namespace Lancamentos.Application.Features.Lancamentos.Commands.RegistrarLancamento;

public sealed record RegistrarLancamentoResponse(
    Guid Id,
    DateOnly Data,
    string Tipo,
    decimal Valor,
    string Descricao);
