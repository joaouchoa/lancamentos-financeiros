using Consolidacao.Domain.Saldos;
using Consolidacao.Domain.Saldos.Repositories;
using Consolidacao.Infrastructure.Idempotencia;
using Consolidacao.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Consolidacao.Infrastructure.Repositories;

public class SaldoDiarioRepository : ISaldoDiarioRepository
{
    private readonly ConsolidacaoDbContext _context;

    public SaldoDiarioRepository(ConsolidacaoDbContext context) => _context = context;

    public async Task<SaldoDiario?> GetByDataAsync(DateOnly data, CancellationToken cancellationToken = default) =>
        await _context.SaldosDiarios.FirstOrDefaultAsync(s => s.Data == data, cancellationToken);

    public async Task<IReadOnlyList<SaldoDiario>> ListAsync(
        DateOnly? dataInicial,
        DateOnly? dataFinal,
        CancellationToken cancellationToken = default)
    {
        var query = _context.SaldosDiarios.AsQueryable();

        if (dataInicial.HasValue)
            query = query.Where(s => s.Data >= dataInicial.Value);

        if (dataFinal.HasValue)
            query = query.Where(s => s.Data <= dataFinal.Value);

        return await query
            .OrderBy(s => s.Data)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SaldoDiario saldoDiario, CancellationToken cancellationToken = default) =>
        await _context.SaldosDiarios.AddAsync(saldoDiario, cancellationToken);

    public async Task<bool> LancamentoJaProcessadoAsync(Guid lancamentoId, CancellationToken cancellationToken = default) =>
        await _context.LancamentosProcessados.AnyAsync(l => l.LancamentoId == lancamentoId, cancellationToken);

    public async Task MarcarLancamentoProcessadoAsync(Guid lancamentoId, CancellationToken cancellationToken = default) =>
        await _context.LancamentosProcessados.AddAsync(LancamentoProcessado.Criar(lancamentoId), cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}
