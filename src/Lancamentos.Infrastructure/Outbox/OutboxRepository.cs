using System.Text.Json;
using Lancamentos.Application.Common.Outbox;
using Lancamentos.Domain.Lancamentos;
using Lancamentos.Infrastructure.Persistence;
using Shared.Contracts.IntegrationEvents;

namespace Lancamentos.Infrastructure.Outbox;

public class OutboxRepository : IOutboxRepository
{
    private readonly LancamentosDbContext _context;

    public OutboxRepository(LancamentosDbContext context) => _context = context;

    public async Task AddAsync(Lancamento lancamento, CancellationToken cancellationToken = default)
    {
        var evento = new LancamentoRegistradoIntegrationEvent(
            lancamento.Id,
            lancamento.Data,
            lancamento.Tipo.ToString(),
            lancamento.Valor);

        var mensagem = OutboxMessage.Criar(
            nameof(LancamentoRegistradoIntegrationEvent),
            JsonSerializer.Serialize(evento));

        await _context.OutboxMessages.AddAsync(mensagem, cancellationToken);
    }
}
