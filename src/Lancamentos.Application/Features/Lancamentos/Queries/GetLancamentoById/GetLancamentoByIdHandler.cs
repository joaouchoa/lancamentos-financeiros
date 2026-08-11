using Lancamentos.Application.Common.Errors;
using Lancamentos.Application.Common.Mediator;
using Lancamentos.Application.Common.Results;
using Lancamentos.Domain.Lancamentos.Repositories;

namespace Lancamentos.Application.Features.Lancamentos.Queries.GetLancamentoById;

public sealed class GetLancamentoByIdHandler : IQueryHandler<GetLancamentoByIdRequest, Result<LancamentoDto>>
{
    private readonly ILancamentoRepository _repository;

    public GetLancamentoByIdHandler(ILancamentoRepository repository) => _repository = repository;

    public async Task<Result<LancamentoDto>> Handle(GetLancamentoByIdRequest request, CancellationToken cancellationToken)
    {
        var lancamento = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (lancamento is null)
            return Result<LancamentoDto>.Failure(Error.NotFound(ApplicationErrorMessages.Lancamento.LancamentoNaoEncontrado));

        return Result<LancamentoDto>.Success(new LancamentoDto(
            lancamento.Id,
            lancamento.Data,
            lancamento.Tipo.ToString(),
            lancamento.Valor,
            lancamento.Descricao));
    }
}
