using System.Net.Http.Json;
using FluentAssertions;
using Lancamentos.Integration.Tests.Common.Dtos;
using Lancamentos.Integration.Tests.Infrastructure;
using Lancamentos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lancamentos.Integration.Tests.Features;

[Collection(LancamentosCollection.Name)]
public class OutboxTests
{
    private readonly HttpClient _client;
    private readonly IntegrationWebApplicationFactory _factory;

    public OutboxTests(IntegrationWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Registrar_DeveGravarEPublicarMensagemNoOutbox()
    {
        // Arrange
        var body = new
        {
            Data = DateOnly.FromDateTime(DateTime.UtcNow),
            Tipo = "Debito",
            Valor = 42m,
            Descricao = "Teste de publicação via outbox"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/lancamentos", body);
        var lancamento = await response.Content.ReadFromJsonAsync<LancamentoDto>();

        // Assert — aguarda o OutboxPublisher em background drenar a mensagem
        var processada = await AguardarMensagemProcessadaAsync(lancamento!.Id, TimeSpan.FromSeconds(40));
        processada.Should().BeTrue("a mensagem do outbox deve ser publicada assim que o RabbitMQ estiver disponível");
    }

    private async Task<bool> AguardarMensagemProcessadaAsync(Guid lancamentoId, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow.Add(timeout);

        while (DateTime.UtcNow < deadline)
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LancamentosDbContext>();

            var existeProcessada = await context.OutboxMessages
                .AnyAsync(m => m.Payload.Contains(lancamentoId.ToString()) && m.ProcessadoEm != null);

            if (existeProcessada)
                return true;

            await Task.Delay(500);
        }

        return false;
    }
}
