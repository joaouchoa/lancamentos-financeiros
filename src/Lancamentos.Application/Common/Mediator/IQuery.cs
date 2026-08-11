using MediatR;

namespace Lancamentos.Application.Common.Mediator;

public interface IQuery<TResponse> : IRequest<TResponse>;
