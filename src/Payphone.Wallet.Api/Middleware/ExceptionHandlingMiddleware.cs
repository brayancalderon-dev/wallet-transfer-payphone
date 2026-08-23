using System.Net;
using System.Text.Json;
using Payphone.Wallet.Application;
using Payphone.Wallet.Domain.Exceptions;

namespace Payphone.Wallet.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            WalletDomainException domainEx => domainEx.ErrorCode switch
            {
                DomainErrorCode.InsufficientBalance => (HttpStatusCode.UnprocessableEntity, domainEx.Message),
                DomainErrorCode.CannotDeleteWalletWithBalance => (HttpStatusCode.UnprocessableEntity, domainEx.Message), // NUEVO
                DomainErrorCode.NegativeAmount => (HttpStatusCode.BadRequest, domainEx.Message),
                DomainErrorCode.InvalidDocumentId => (HttpStatusCode.BadRequest, domainEx.Message),
                DomainErrorCode.InvalidName => (HttpStatusCode.BadRequest, domainEx.Message),
                DomainErrorCode.SameWalletTransfer => (HttpStatusCode.BadRequest, domainEx.Message),
                _ => (HttpStatusCode.BadRequest, domainEx.Message)
            },
            NotFoundException notFoundEx => (HttpStatusCode.NotFound, notFoundEx.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exception occurred");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var problemDetails = new
        {
            status = (int)statusCode,
            title,
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}