using Lancamentos.Domain.Lancamentos;
using Lancamentos.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Lancamentos.Infrastructure.Persistence;

public class LancamentosDbContext : DbContext
{
    public LancamentosDbContext(DbContextOptions<LancamentosDbContext> options) : base(options) { }

    public DbSet<Lancamento> Lancamentos => Set<Lancamento>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LancamentosDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
