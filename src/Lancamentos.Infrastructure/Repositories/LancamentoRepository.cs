using Lancamentos.Domain.Lancamentos;
using Lancamentos.Domain.Lancamentos.Repositories;
using Lancamentos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lancamentos.Infrastructure.Repositories;

public class LancamentoRepository : ILancamentoRepository
{
    private readonly LancamentosDbContext _context;

    public LancamentoRepository(LancamentosDbContext context) => _context = context;

    public async Task<Lancamento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Lancamentos.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Lancamento>> ListAsync(LancamentoFilter filter, CancellationToken cancellationToken = default) =>
        await ApplyFilters(filter)
            .OrderByDescending(l => l.Data)
            .Skip((filter.Pagina - 1) * filter.TamanhoPagina)
            .Take(filter.TamanhoPagina)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<int> CountAsync(LancamentoFilter filter, CancellationToken cancellationToken = default) =>
        await ApplyFilters(filter).CountAsync(cancellationToken);

    public async Task AddAsync(Lancamento lancamento, CancellationToken cancellationToken = default) =>
        await _context.Lancamentos.AddAsync(lancamento, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);

    private IQueryable<Lancamento> ApplyFilters(LancamentoFilter filter)
    {
        var query = _context.Lancamentos.AsQueryable();

        if (filter.Data.HasValue)
            query = query.Where(l => l.Data == filter.Data.Value);

        if (filter.Tipo.HasValue)
            query = query.Where(l => l.Tipo == filter.Tipo.Value);

        return query;
    }
}
