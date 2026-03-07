using Domain.AbstractServices;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.PipelineBehaviors;
public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
    private readonly ISessionUserService _sessionUserService;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger, ISessionUserService sessionUserService)
    {
        _logger = logger;
        _sessionUserService = sessionUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _sessionUserService.UserId ?? "Аноним";
        var username = _sessionUserService.Username ?? "Аноним";

        _logger.LogInformation("Выполнение запроса: {Name} Пользователь: {Username} ({UserId}) {@Request}", requestName, username, userId, request);

        try
        {
            var response = await next();
            _logger.LogInformation("Запрос выполнен успешно: {Name} Пользователь: {Username}", requestName, username);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка выполнения запроса: {Name} Пользователь: {Username} Ошибка: {Error}", requestName, username, ex.Message);
            throw;
        }
    }

}