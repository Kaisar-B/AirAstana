namespace Domain.AbstractServices;
/// <summary>
/// Сервис для работы с кэшированными данными.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Получает объект из кэша по указанному ключу.
    /// </summary>
    /// <typeparam name="T">Тип объекта, который ожидается из кэша.</typeparam>
    /// <param name="key">Ключ для поиска объекта в кэше.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>Объект типа <typeparamref name="T"/>, если он найден в кэше; иначе null.</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Сохраняет объект в кэш с указанным ключом и временем жизни.
    /// </summary>
    /// <typeparam name="T">Тип сохраняемого объекта.</typeparam>
    /// <param name="key">Ключ для сохранения объекта в кэше.</param>
    /// <param name="value">Объект, который необходимо сохранить в кэше.</param>
    /// <param name="expiration">Время жизни объекта в кэше. Если null, используется значение по умолчанию.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Удаляет объект из кэша по указанному ключу.
    /// </summary>
    /// <param name="key">Ключ объекта, который необходимо удалить.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}