namespace Lancamentos.Application.Features.Lancamentos.Queries.ListLancamentos;

public sealed record ListLancamentosPagedResponse(
    List<LancamentoDto> Itens,
    int TotalRegistros,
    int Pagina,
    int TamanhoPagina,
    int TotalPaginas);
