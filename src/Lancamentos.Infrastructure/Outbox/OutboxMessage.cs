namespace Lancamentos.Infrastructure.Outbox;

public class OutboxMessage
{
    public Guid Id { get; private set; }
    public string Tipo { get; private set; } = null!;
    public string Payload { get; private set; } = null!;
    public DateTime CriadoEm { get; private set; }
    public DateTime? ProcessadoEm { get; private set; }

    private OutboxMessage() { }

    private OutboxMessage(string tipo, string payload)
    {
        Id = Guid.NewGuid();
        Tipo = tipo;
        Payload = payload;
        CriadoEm = DateTime.UtcNow;
    }

    public static OutboxMessage Criar(string tipo, string payload) => new(tipo, payload);

    public void MarcarComoProcessada() => ProcessadoEm = DateTime.UtcNow;
}
