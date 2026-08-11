namespace Consolidacao.Integration.Tests.Common.Dtos;

internal record SaldoDiarioDto(
    DateOnly Data,
    decimal TotalCreditos,
    decimal TotalDebitos,
    decimal Saldo);
