using Consolidacao.Application.Features.Saldos.Commands.AplicarLancamento;
using MassTransit;
using MediatR;
using Shared.Contracts.IntegrationEvents;

namespace Consolidacao.Infrastructure.Messaging.Consumers;

/// <summary>
/// Consome o evento publicado pelo serviço de Lançamentos e aplica ao saldo
/// diário via o mesmo pipeline MediatR usado pela API (Command + Validation +
/// Logging behaviors). Se a aplicação falhar, a exceção sobe e o MassTransit
/// aciona a política de retry configurada; após esgotar as tentativas, a
/// mensagem é movida automaticamente para a fila de erro (_error).
/// </summary>
public class LancamentoRegistradoConsumer : IConsumer<LancamentoRegistradoIntegrationEvent>
{
    private readonly ISender _sender;

    public LancamentoRegistradoConsumer(ISender sender) => _sender = sender;

    public async Task Consume(ConsumeContext<LancamentoRegistradoIntegrationEvent> context)
    {
        var evento = context.Message;

        var command = new AplicarLancamentoRequest(evento.LancamentoId, evento.Data, evento.Tipo, evento.Valor);

        var result = await _sender.Send(command, context.CancellationToken);

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Falha ao aplicar o lançamento {evento.LancamentoId} ao saldo diário: {result.Error.Message}");
    }
}
