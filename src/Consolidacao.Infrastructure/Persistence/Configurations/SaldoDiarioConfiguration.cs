using Consolidacao.Domain.Saldos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consolidacao.Infrastructure.Persistence.Configurations;

public class SaldoDiarioConfiguration : IEntityTypeConfiguration<SaldoDiario>
{
    public void Configure(EntityTypeBuilder<SaldoDiario> builder)
    {
        builder.ToTable("saldos_diarios");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id");

        builder.Property(s => s.Data)
            .HasColumnName("data")
            .HasColumnType("date")
            .IsRequired();

        builder.HasIndex(s => s.Data).IsUnique();

        builder.Property(s => s.TotalCreditos)
            .HasColumnName("total_creditos")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.TotalDebitos)
            .HasColumnName("total_debitos")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.Saldo)
            .HasColumnName("saldo")
            .HasPrecision(18, 2)
            .IsRequired();
    }
}
