using MediatR;

namespace Lancamentos.Application.Common.Mediator;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;
