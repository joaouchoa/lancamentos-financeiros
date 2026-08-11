using Consolidacao.Domain.Saldos;
using Consolidacao.Infrastructure.Idempotencia;
using Microsoft.EntityFrameworkCore;

namespace Consolidacao.Infrastructure.Persistence;

public class ConsolidacaoDbContext : DbContext
{
    public ConsolidacaoDbContext(DbContextOptions<ConsolidacaoDbContext> options) : base(options) { }

    public DbSet<SaldoDiario> SaldosDiarios => Set<SaldoDiario>();

    public DbSet<LancamentoProcessado> LancamentosProcessados => Set<LancamentoProcessado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConsolidacaoDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
