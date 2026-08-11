namespace Lancamentos.Domain.Lancamentos.Repositories;

public record LancamentoFilter(
    DateOnly? Data = null,
    TipoLancamento? Tipo = null,
    int Pagina = 1,
    int TamanhoPagina = 10);
