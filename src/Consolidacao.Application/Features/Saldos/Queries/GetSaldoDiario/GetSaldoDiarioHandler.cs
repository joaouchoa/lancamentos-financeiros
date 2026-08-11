using Consolidacao.Application.Common.Mediator;
using Consolidacao.Application.Common.Results;
using Consolidacao.Domain.Saldos.Repositories;

namespace Consolidacao.Application.Features.Saldos.Queries.GetSaldoDiario;

public sealed class GetSaldoDiarioHandler : IQueryHandler<GetSaldoDiarioRequest, Result<SaldoDiarioDto>>
{
    private readonly ISaldoDiarioRepository _repository;

    public GetSaldoDiarioHandler(ISaldoDiarioRepository repository) => _repository = repository;

    public async Task<Result<SaldoDiarioDto>> Handle(GetSaldoDiarioRequest request, CancellationToken cancellationToken)
    {
        var saldoDiario = await _repository.GetByDataAsync(request.Data, cancellationToken);

        // Nenhum lançamento naquele dia é um estado válido, não um erro — retorna zeros.
        var dto = saldoDiario is null
            ? new SaldoDiarioDto(request.Data, 0m, 0m, 0m)
            : new SaldoDiarioDto(saldoDiario.Data, saldoDiario.TotalCreditos, saldoDiario.TotalDebitos, saldoDiario.Saldo);

        return Result<SaldoDiarioDto>.Success(dto);
    }
}
