using Lancamentos.Application.Common.Mediator;
using Lancamentos.Application.Common.Results;
using Lancamentos.Domain.Lancamentos;
using Lancamentos.Domain.Lancamentos.Repositories;

namespace Lancamentos.Application.Features.Lancamentos.Queries.ListLancamentos;

public sealed class ListLancamentosHandler
    : IQueryHandler<ListLancamentosRequest, Result<ListLancamentosPagedResponse>>
{
    private readonly ILancamentoRepository _repository;

    public ListLancamentosHandler(ILancamentoRepository repository) => _repository = repository;

    public async Task<Result<ListLancamentosPagedResponse>> Handle(
        ListLancamentosRequest request,
        CancellationToken cancellationToken)
    {
        TipoLancamento? tipo = null;
        if (!string.IsNullOrWhiteSpace(request.Tipo) && Enum.TryParse<TipoLancamento>(request.Tipo, ignoreCase: true, out var tipoParsed))
            tipo = tipoParsed;

        var filter = new LancamentoFilter(request.Data, tipo, request.Pagina, request.TamanhoPagina);

        var lancamentos = await _repository.ListAsync(filter, cancellationToken);
        var total = await _repository.CountAsync(filter, cancellationToken);

        var itens = lancamentos
            .Select(l => new LancamentoDto(l.Id, l.Data, l.Tipo.ToString(), l.Valor, l.Descricao))
            .ToList();

        var totalPaginas = (int)Math.Ceiling(total / (double)request.TamanhoPagina);

        return Result<ListLancamentosPagedResponse>.Success(new ListLancamentosPagedResponse(
            itens,
            total,
            request.Pagina,
            request.TamanhoPagina,
            totalPaginas));
    }
}
