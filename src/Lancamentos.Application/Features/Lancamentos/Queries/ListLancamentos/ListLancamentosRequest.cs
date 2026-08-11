using Lancamentos.Application.Common.Mediator;
using Lancamentos.Application.Common.Results;

namespace Lancamentos.Application.Features.Lancamentos.Queries.ListLancamentos;

public sealed record ListLancamentosRequest(
    DateOnly? Data = null,
    string? Tipo = null,
    int Pagina = 1,
    int TamanhoPagina = 10) : IQuery<Result<ListLancamentosPagedResponse>>;
