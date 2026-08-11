using System.Text.Json;
using Lancamentos.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Contracts.IntegrationEvents;

namespace Lancamentos.Infrastructure.Outbox;

public class OutboxPublisher : BackgroundService
{
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxPublisher> _logger;

    public OutboxPublisher(IServiceScopeFactory scopeFactory, ILogger<OutboxPublisher> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PublicarPendentesAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Falha ao publicar mensagens pendentes do outbox. Nova tentativa no próximo ciclo.");
            }

            try
            {
                await Task.Delay(PollingInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // encerramento normal do host
            }
        }
    }

    private async Task PublicarPendentesAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LancamentosDbContext>();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        var pendentes = await context.OutboxMessages
            .Where(m => m.ProcessadoEm == null)
            .OrderBy(m => m.CriadoEm)
            .Take(50)
            .ToListAsync(cancellationToken);

        if (pendentes.Count == 0)
            return;

        foreach (var mensagem in pendentes)
        {
            await PublicarAsync(publishEndpoint, mensagem, cancellationToken);
            mensagem.MarcarComoProcessada();
        }

        await context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Quantidade} mensagem(ns) do outbox publicada(s) com sucesso.", pendentes.Count);
    }

    private static async Task PublicarAsync(IPublishEndpoint publishEndpoint, OutboxMessage mensagem, CancellationToken cancellationToken)
    {
        if (mensagem.Tipo != nameof(LancamentoRegistradoIntegrationEvent))
            throw new InvalidOperationException($"Tipo de mensagem de outbox desconhecido: {mensagem.Tipo}.");

        var evento = JsonSerializer.Deserialize<LancamentoRegistradoIntegrationEvent>(mensagem.Payload)
            ?? throw new InvalidOperationException($"Falha ao desserializar a mensagem de outbox {mensagem.Id}.");

        await publishEndpoint.Publish(evento, cancellationToken);
    }
}
