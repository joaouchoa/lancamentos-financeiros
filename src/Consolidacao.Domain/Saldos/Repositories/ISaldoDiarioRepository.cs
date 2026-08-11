namespace Consolidacao.Domain.Saldos.Repositories;

public interface ISaldoDiarioRepository
{
    Task<SaldoDiario?> GetByDataAsync(DateOnly data, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SaldoDiario>> ListAsync(
        DateOnly? dataInicial,
        DateOnly? dataFinal,
        CancellationToken cancellationToken = default);

    Task AddAsync(SaldoDiario saldoDiario, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se um lançamento já foi aplicado ao saldo diário — base da
    /// idempotência do consumidor contra reentrega do RabbitMQ.
    /// </summary>
    Task<bool> LancamentoJaProcessadoAsync(Guid lancamentoId, CancellationToken cancellationToken = default);

    Task MarcarLancamentoProcessadoAsync(Guid lancamentoId, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
