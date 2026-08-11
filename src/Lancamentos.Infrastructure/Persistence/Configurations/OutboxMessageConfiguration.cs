using Lancamentos.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lancamentos.Infrastructure.Persistence.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasColumnName("id");

        builder.Property(m => m.Tipo)
            .HasColumnName("tipo")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(m => m.Payload)
            .HasColumnName("payload")
            .IsRequired();

        builder.Property(m => m.CriadoEm)
            .HasColumnName("criado_em")
            .IsRequired();

        builder.Property(m => m.ProcessadoEm)
            .HasColumnName("processado_em");
    }
}
