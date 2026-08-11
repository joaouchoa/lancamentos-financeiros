namespace Consolidacao.Application.Features.Saldos.Commands.AplicarLancamento;

/// <summary>
/// <paramref name="Aplicado"/> é <c>false</c> quando o lançamento já havia sido
/// processado anteriormente — reentrega idempotente do RabbitMQ, tratada como
/// no-op, não como erro.
/// </summary>
public sealed record AplicarLancamentoResponse(bool Aplicado);
