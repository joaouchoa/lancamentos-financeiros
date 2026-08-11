using Consolidacao.Application.Common.Mediator;
using Consolidacao.Application.Common.Results;
using Consolidacao.Domain.Saldos;
using Consolidacao.Domain.Saldos.Repositories;

namespace Consolidacao.Application.Features.Saldos.Commands.AplicarLancamento;

public sealed class AplicarLancamentoHandler
    : ICommandHandler<AplicarLancamentoRequest, Result<AplicarLancamentoResponse>>
{
    private readonly ISaldoDiarioRepository _repository;

    public AplicarLancamentoHandler(ISaldoDiarioRepository repository) => _repository = repository;

    public async Task<Result<AplicarLancamentoResponse>> Handle(
        AplicarLancamentoRequest request,
        CancellationToken cancellationToken)
    {
        // Idempotência: reentrega do RabbitMQ não pode contar o lançamento duas vezes.
        var jaProcessado = await _repository.LancamentoJaProcessadoAsync(request.LancamentoId, cancellationToken);
        if (jaProcessado)
            return Result<AplicarLancamentoResponse>.Success(new AplicarLancamentoResponse(Aplicado: false));

        var tipo = Enum.Parse<TipoLancamento>(request.Tipo, ignoreCase: true);

        var saldoDiario = await _repository.GetByDataAsync(request.Data, cancellationToken);
        if (saldoDiario is null)
        {
            saldoDiario = SaldoDiario.Criar(request.Data);
            await _repository.AddAsync(saldoDiario, cancellationToken);
        }

        saldoDiario.Aplicar(tipo, request.Valor);

        await _repository.MarcarLancamentoProcessadoAsync(request.LancamentoId, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result<AplicarLancamentoResponse>.Success(new AplicarLancamentoResponse(Aplicado: true));
    }
}
