using Lancamentos.Application.Common.Mediator;
using Lancamentos.Application.Common.Results;

namespace Lancamentos.Application.Features.Lancamentos.Commands.RegistrarLancamento;

public sealed record RegistrarLancamentoRequest(
    DateOnly Data,
    string Tipo,
    decimal Valor,
    string Descricao) : ICommand<Result<RegistrarLancamentoResponse>>;
