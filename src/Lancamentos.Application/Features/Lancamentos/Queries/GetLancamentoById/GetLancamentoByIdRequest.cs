using Lancamentos.Application.Common.Mediator;
using Lancamentos.Application.Common.Results;

namespace Lancamentos.Application.Features.Lancamentos.Queries.GetLancamentoById;

public sealed record GetLancamentoByIdRequest(Guid Id) : IQuery<Result<LancamentoDto>>;
