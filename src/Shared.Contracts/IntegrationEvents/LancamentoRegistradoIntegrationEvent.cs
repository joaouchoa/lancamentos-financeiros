namespace Shared.Contracts.IntegrationEvents;

public sealed record LancamentoRegistradoIntegrationEvent(
    Guid LancamentoId,
    DateOnly Data,
    string Tipo,
    decimal Valor);
