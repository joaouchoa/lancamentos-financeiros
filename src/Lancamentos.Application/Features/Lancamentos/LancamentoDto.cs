namespace Lancamentos.Application.Features.Lancamentos;

public sealed record LancamentoDto(
    Guid Id,
    DateOnly Data,
    string Tipo,
    decimal Valor,
    string Descricao);
