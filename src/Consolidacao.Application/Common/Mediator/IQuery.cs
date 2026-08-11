using MediatR;

namespace Consolidacao.Application.Common.Mediator;

public interface IQuery<TResponse> : IRequest<TResponse>;
