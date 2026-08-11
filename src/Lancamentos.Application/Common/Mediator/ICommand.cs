using MediatR;

namespace Lancamentos.Application.Common.Mediator;

public interface ICommand<TResponse> : IRequest<TResponse>;
