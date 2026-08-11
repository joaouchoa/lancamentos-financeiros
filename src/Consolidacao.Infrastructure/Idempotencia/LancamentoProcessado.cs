namespace Consolidacao.Infrastructure.Idempotencia;

/// <summary>
/// Guarda de idempotência: registra que um lançamento já foi aplicado ao
/// saldo diário, para que reentregas do RabbitMQ (at-least-once) não sejam
/// contadas mais de uma vez.
/// </summary>
public class LancamentoProcessado
{
    public Guid LancamentoId { get; private set; }
    public DateTime ProcessadoEm { get; private set; }

    private LancamentoProcessado() { }

    private LancamentoProcessado(Guid lancamentoId)
    {
        LancamentoId = lancamentoId;
        ProcessadoEm = DateTime.UtcNow;
    }

    public static LancamentoProcessado Criar(Guid lancamentoId) => new(lancamentoId);
}
