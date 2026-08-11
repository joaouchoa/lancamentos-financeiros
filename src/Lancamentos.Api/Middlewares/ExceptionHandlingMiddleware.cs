using System.Text.Json;
using Lancamentos.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using ValidationException = FluentValidation.ValidationException;

namespace Lancamentos.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (DomainException ex)
        {
            await HandleDomainExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado: {Message}", ex.Message);
            await HandleUnexpectedExceptionAsync(context);
        }
    }

    private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        var problem = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Erro de validação",
            Type = "https://tools.ietf.org/html/rfc7807"
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }

    private static Task HandleDomainExceptionAsync(HttpContext context, DomainException exception)
    {
        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = "Regra de negócio violada",
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc4918#section-11.2"
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        return context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }

    private static Task HandleUnexpectedExceptionAsync(HttpContext context)
    {
        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Erro interno inesperado",
            Type = "https://tools.ietf.org/html/rfc7807"
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
