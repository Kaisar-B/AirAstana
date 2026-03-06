namespace Application.Common.Interfaces;

/// <summary>
/// Интерфейс сервиса для хэширования и проверки паролей.
/// Предоставляет методы для безопасного преобразования пароля в хэш
/// и последующей проверки введённого пароля на соответствие сохранённому хэшу.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Создаёт хэш из переданного пароля.
    /// </summary>
    /// <param name="password">Исходный пароль в открытом виде.</param>
    /// <returns>Строка, содержащая хэш пароля.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Проверяет соответствие введённого пароля сохранённому хэшу.
    /// </summary>
    /// <param name="password">Пароль, введённый пользователем.</param>
    /// <param name="passwordHash">Сохранённый хэш пароля.</param>
    /// <returns>
    /// true — если пароль соответствует хэшу;  
    /// false — если пароль неверный.
    /// </returns>
    bool VerifyPassword(string password, string passwordHash);
}