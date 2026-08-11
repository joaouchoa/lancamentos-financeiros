namespace Consolidacao.Application.Features.Saldos;

public sealed record SaldoDiarioDto(
    DateOnly Data,
    decimal TotalCreditos,
    decimal TotalDebitos,
    decimal Saldo);
