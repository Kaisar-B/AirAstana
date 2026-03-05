namespace Infrastructure.Identity.Authentication;
/// <summary>
/// Сервис для работы с аутентификацией и JWT-токенами.
/// Предоставляет методы для генерации и валидации токенов пользователя.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Генерирует JWT-токен для пользователя с указанным идентификатором, именем и ролью.
    /// </summary>
    /// <param name="userId">Уникальный идентификатор пользователя.</param>
    /// <param name="username">Имя пользователя.</param>
    /// <param name="role">Роль пользователя.</param>
    /// <returns>Сформированный JWT-токен в виде строки.</returns>
    string GenerateJwtToken(string userId, string username, string role);

    /// <summary>
    /// Проверяет корректность и срок действия JWT-токена.
    /// </summary>
    /// <param name="token">JWT-токен, который необходимо проверить.</param>
    /// <returns>
    /// true — если токен валиден; false — если токен недействителен или просрочен.
    /// </returns>
    bool ValidateToken(string token);
}