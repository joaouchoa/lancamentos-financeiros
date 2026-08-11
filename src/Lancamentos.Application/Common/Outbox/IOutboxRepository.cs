using Lancamentos.Domain.Lancamentos;

namespace Lancamentos.Application.Common.Outbox;

/// <summary>
/// Porta de saída para o padrão Outbox. A Application só sabe que precisa
/// "garantir a publicação eventual" de um lançamento — a Infrastructure decide
/// como serializar e efetivamente publicar a mensagem no barramento.
/// </summary>
public interface IOutboxRepository
{
    Task AddAsync(Lancamento lancamento, CancellationToken cancellationToken = default);
}
