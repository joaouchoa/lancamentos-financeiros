namespace Lancamentos.Integration.Tests.Common.Dtos;

internal record LancamentoDto(
    Guid Id,
    DateOnly Data,
    string Tipo,
    decimal Valor,
    string Descricao);

internal record PagedLancamentosDto(
    List<LancamentoDto> Itens,
    int TotalRegistros,
    int Pagina,
    int TamanhoPagina,
    int TotalPaginas);
