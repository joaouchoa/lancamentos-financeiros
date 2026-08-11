using Consolidacao.Application.Common.Mediator;
using Consolidacao.Application.Common.Results;
using Consolidacao.Domain.Saldos.Repositories;

namespace Consolidacao.Application.Features.Saldos.Queries.ListSaldosDiarios;

public sealed class ListSaldosDiariosHandler
    : IQueryHandler<ListSaldosDiariosRequest, Result<List<SaldoDiarioDto>>>
{
    private readonly ISaldoDiarioRepository _repository;

    public ListSaldosDiariosHandler(ISaldoDiarioRepository repository) => _repository = repository;

    public async Task<Result<List<SaldoDiarioDto>>> Handle(
        ListSaldosDiariosRequest request,
        CancellationToken cancellationToken)
    {
        var saldos = await _repository.ListAsync(request.DataInicial, request.DataFinal, cancellationToken);

        var dtos = saldos
            .Select(s => new SaldoDiarioDto(s.Data, s.TotalCreditos, s.TotalDebitos, s.Saldo))
            .ToList();

        return Result<List<SaldoDiarioDto>>.Success(dtos);
    }
}
