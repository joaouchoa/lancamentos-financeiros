using MediatR;

namespace Consolidacao.Application.Common.Mediator;

public interface ICommand<TResponse> : IRequest<TResponse>;
