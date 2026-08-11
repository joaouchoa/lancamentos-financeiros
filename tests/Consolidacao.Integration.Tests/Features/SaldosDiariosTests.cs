using System.Net;
using System.Net.Http.Json;
using Consolidacao.Integration.Tests.Common.Dtos;
using Consolidacao.Integration.Tests.Infrastructure;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Shared.Contracts.IntegrationEvents;

namespace Consolidacao.Integration.Tests.Features;

[Collection(ConsolidacaoCollection.Name)]
public class SaldosDiariosTests
{
    private readonly HttpClient _client;
    private readonly IntegrationWebApplicationFactory _factory;

    public SaldosDiariosTests(IntegrationWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetByData_DeveRetornarZerado_QuandoNaoHaLancamentos()
    {
        var data = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-1);

        var response = await _client.GetAsync($"/api/saldos-diarios/{data:yyyy-MM-dd}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<SaldoDiarioDto>();
        dto!.Saldo.Should().Be(0m);
    }

    [Fact]
    public async Task Consumer_DeveAplicarLancamento_QuandoEventoPublicadoNoBarramento()
    {
        // Arrange
        var data = DateOnly.FromDateTime(DateTime.UtcNow);
        var evento = new LancamentoRegistradoIntegrationEvent(Guid.NewGuid(), data, "Credito", 250m);

        // Act — simula outro serviço (Lançamentos) publicando o evento
        using (var scope = _factory.Services.CreateScope())
        {
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
            await publishEndpoint.Publish(evento);
        }

        // Assert — aguarda o consumer processar de forma assíncrona
        var saldo = await AguardarSaldoAsync(data, esperado => esperado.TotalCreditos >= 250m, TimeSpan.FromSeconds(20));
        saldo.Should().NotBeNull();
        saldo!.TotalCreditos.Should().BeGreaterThanOrEqualTo(250m);
    }

    [Fact]
    public async Task Consumer_DeveSerIdempotente_QuandoMensagemReentregue()
    {
        // Arrange — captura o saldo base antes de publicar, já que o dia "hoje"
        // pode conter dados de seed; a idempotência é validada pela variação
        // (delta), não pelo valor absoluto.
        var data = DateOnly.FromDateTime(DateTime.UtcNow);
        var lancamentoId = Guid.NewGuid();
        var evento = new LancamentoRegistradoIntegrationEvent(lancamentoId, data, "Debito", 77m);

        var totalDebitosAntes = await ObterTotalDebitosAsync(data);

        // Act — publica o MESMO evento duas vezes (simula reentrega at-least-once)
        using (var scope = _factory.Services.CreateScope())
        {
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
            await publishEndpoint.Publish(evento);
            await publishEndpoint.Publish(evento);
        }

        // Assert — espera estabilizar e confirma que só foi contado uma vez
        await Task.Delay(TimeSpan.FromSeconds(8));

        var totalDebitosDepois = await ObterTotalDebitosAsync(data);

        (totalDebitosDepois - totalDebitosAntes).Should().Be(77m,
            "a segunda entrega do mesmo LancamentoId não deve ser contada novamente");
    }

    private async Task<decimal> ObterTotalDebitosAsync(DateOnly data)
    {
        var response = await _client.GetAsync($"/api/saldos-diarios/{data:yyyy-MM-dd}");
        var dto = await response.Content.ReadFromJsonAsync<SaldoDiarioDto>();
        return dto!.TotalDebitos;
    }

    private async Task<SaldoDiarioDto?> AguardarSaldoAsync(DateOnly data, Func<SaldoDiarioDto, bool> condicao, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow.Add(timeout);

        while (DateTime.UtcNow < deadline)
        {
            var response = await _client.GetAsync($"/api/saldos-diarios/{data:yyyy-MM-dd}");
            var dto = await response.Content.ReadFromJsonAsync<SaldoDiarioDto>();

            if (dto is not null && condicao(dto))
                return dto;

            await Task.Delay(500);
        }

        return null;
    }
}
