using Consolidacao.Infrastructure.Idempotencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consolidacao.Infrastructure.Persistence.Configurations;

public class LancamentoProcessadoConfiguration : IEntityTypeConfiguration<LancamentoProcessado>
{
    public void Configure(EntityTypeBuilder<LancamentoProcessado> builder)
    {
        builder.ToTable("lancamentos_processados");

        builder.HasKey(l => l.LancamentoId);
        builder.Property(l => l.LancamentoId).HasColumnName("lancamento_id");

        builder.Property(l => l.ProcessadoEm)
            .HasColumnName("processado_em")
            .IsRequired();
    }
}
