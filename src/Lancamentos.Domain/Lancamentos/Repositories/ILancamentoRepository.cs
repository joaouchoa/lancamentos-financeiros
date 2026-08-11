namespace Lancamentos.Domain.Lancamentos.Repositories;

public interface ILancamentoRepository
{
    Task<Lancamento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Lancamento>> ListAsync(LancamentoFilter filter, CancellationToken cancellationToken = default);

    Task<int> CountAsync(LancamentoFilter filter, CancellationToken cancellationToken = default);

    Task AddAsync(Lancamento lancamento, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
