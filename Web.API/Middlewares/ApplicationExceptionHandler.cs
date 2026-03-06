using Domain.AppExceptions;
using System.Net;
using System.Text.Json;
using Web.API.Models;

namespace Web.API.Middlewares;

/// <summary>
/// Middleware для глобальной обработки исключений приложения.
/// Перехватывает все необработанные исключения, логирует их и возвращает
/// клиенту стандартизированный JSON-ответ с кодом статуса и сообщением.
/// </summary>
public class ApplicationExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApplicationExceptionHandler> _logger;

    public ApplicationExceptionHandler(RequestDelegate next, ILogger<ApplicationExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Обрабатывает HTTP-запрос, перехватывая исключения на уровне конвейера.
    /// </summary>
    /// <param name="context">Контекст текущего HTTP-запроса</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Продолжаем обработку запроса дальше по конвейеру
            await _next(context);
        }
        catch (Exception ex)
        {
            // Перехватываем все исключения и формируем корректный ответ
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Формирует и отправляет клиенту JSON-ответ с информацией об ошибке.
    /// </summary>
    /// <param name="context">Контекст HTTP-запроса</param>
    /// <param name="exception">Возникшее исключение</param>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        // Определяем тип исключения и формируем стандартизированный ответ
        var errorResponse = exception switch
        {
            InputValidationException validationEx => ApiResult<int>.Fail(
                (int)HttpStatusCode.BadRequest,
                string.Join("; ", validationEx.Errors.Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value)}"))
            ),

            DomainException domainEx => ApiResult<int>.Fail(
                (int)HttpStatusCode.BadRequest,
                domainEx.Message
            ),

            UnauthorizedAccessException => ApiResult<int>.Fail(
                (int)HttpStatusCode.Forbidden,
                "Нет доступа к данному ресурсу"
            ),

            _ => ApiResult<int>.Fail(
                (int)HttpStatusCode.InternalServerError,
                "Внутренняя ошибка сервера"
            )
        };

        response.StatusCode = errorResponse.Data; // Код HTTP соответствует статусу ошибки

        // Логирование полной информации об ошибке
        _logger.LogError(exception,
            "Ошибка обработки запроса. Путь: {Path}, Метод: {Method}, StatusCode: {StatusCode}, Сообщение: {Message}",
            context.Request.Path,
            context.Request.Method,
            response.StatusCode,
            exception.Message);

        // Сериализация ответа в JSON с camelCase для фронтенда
        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }
}