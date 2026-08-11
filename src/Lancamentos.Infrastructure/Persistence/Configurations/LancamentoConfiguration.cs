using Lancamentos.Domain.Lancamentos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lancamentos.Infrastructure.Persistence.Configurations;

public class LancamentoConfiguration : IEntityTypeConfiguration<Lancamento>
{
    public void Configure(EntityTypeBuilder<Lancamento> builder)
    {
        builder.ToTable("lancamentos");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasColumnName("id");

        builder.Property(l => l.Data)
            .HasColumnName("data")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(l => l.Tipo)
            .HasColumnName("tipo")
            .IsRequired();

        builder.Property(l => l.Valor)
            .HasColumnName("valor")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(l => l.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(500)
            .IsRequired();
    }
}
