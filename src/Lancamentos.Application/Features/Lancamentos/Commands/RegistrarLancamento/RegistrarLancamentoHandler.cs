using Lancamentos.Application.Common.Mediator;
using Lancamentos.Application.Common.Outbox;
using Lancamentos.Application.Common.Results;
using Lancamentos.Domain.Lancamentos;
using Lancamentos.Domain.Lancamentos.Repositories;

namespace Lancamentos.Application.Features.Lancamentos.Commands.RegistrarLancamento;

public sealed class RegistrarLancamentoHandler
    : ICommandHandler<RegistrarLancamentoRequest, Result<RegistrarLancamentoResponse>>
{
    private readonly ILancamentoRepository _repository;
    private readonly IOutboxRepository _outboxRepository;

    public RegistrarLancamentoHandler(ILancamentoRepository repository, IOutboxRepository outboxRepository)
    {
        _repository = repository;
        _outboxRepository = outboxRepository;
    }

    public async Task<Result<RegistrarLancamentoResponse>> Handle(
        RegistrarLancamentoRequest request,
        CancellationToken cancellationToken)
    {
        var tipo = Enum.Parse<TipoLancamento>(request.Tipo, ignoreCase: true);
        var lancamento = Lancamento.Criar(request.Data, tipo, request.Valor, request.Descricao);

        await _repository.AddAsync(lancamento, cancellationToken);
        await _outboxRepository.AddAsync(lancamento, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result<RegistrarLancamentoResponse>.Success(new RegistrarLancamentoResponse(
            lancamento.Id,
            lancamento.Data,
            lancamento.Tipo.ToString(),
            lancamento.Valor,
            lancamento.Descricao));
    }
}
