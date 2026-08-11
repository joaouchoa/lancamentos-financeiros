using Consolidacao.Application.Common.Mediator;
using Consolidacao.Application.Common.Results;

namespace Consolidacao.Application.Features.Saldos.Queries.GetSaldoDiario;

public sealed record GetSaldoDiarioRequest(DateOnly Data) : IQuery<Result<SaldoDiarioDto>>;
