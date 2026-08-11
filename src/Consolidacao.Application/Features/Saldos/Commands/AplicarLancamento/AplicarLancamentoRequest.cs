using Consolidacao.Application.Common.Mediator;
using Consolidacao.Application.Common.Results;

namespace Consolidacao.Application.Features.Saldos.Commands.AplicarLancamento;

public sealed record AplicarLancamentoRequest(
    Guid LancamentoId,
    DateOnly Data,
    string Tipo,
    decimal Valor) : ICommand<Result<AplicarLancamentoResponse>>;
