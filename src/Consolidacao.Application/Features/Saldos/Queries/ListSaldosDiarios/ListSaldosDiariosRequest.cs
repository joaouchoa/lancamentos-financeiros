using Consolidacao.Application.Common.Mediator;
using Consolidacao.Application.Common.Results;

namespace Consolidacao.Application.Features.Saldos.Queries.ListSaldosDiarios;

public sealed record ListSaldosDiariosRequest(
    DateOnly? DataInicial = null,
    DateOnly? DataFinal = null) : IQuery<Result<List<SaldoDiarioDto>>>;
