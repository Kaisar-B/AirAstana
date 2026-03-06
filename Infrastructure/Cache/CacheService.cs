using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using Domain.AbstractServices;

namespace Infrastructure.Cache;

/// <summary>
///     Сервис для работы с распределнном кэшом.
/// </summary>
internal class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheService> _logger;
    private static readonly TimeSpan DefaultCacheExpiration = TimeSpan.FromHours(1);

    public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    ///     Получение данных из кэша по ключу.
    ///     Можно использовать ValueTask для эффективности.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var cachedData = await _cache.GetAsync(key, cancellationToken);

            if (cachedData is null)
            {
                _logger.LogDebug("Кэш не существует для ключа: {Key}", key);
                return null;
            }

            var result = DeserializeFromUtf8<T>(cachedData);

            if (result is null)
            {
                _logger.LogWarning("Десериализация кэша вернула null для ключа: {Key}", key);
            }
            else
            {
                _logger.LogDebug("Кэш для ключа: {Key}", key);
            }

            return result;
        }
        catch (JsonException jex)
        {
            _logger.LogError(jex, "Ошибка десериализации кэша. Ключ: {Key}", key);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при чтении из кэша. Ключ: {Key}", key);
            return null;
        }
    }

    /// <summary>
    ///     Запись данных в кэш по ключу.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiration"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var dataBytes = SerializeToUtf8Bytes(value);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? DefaultCacheExpiration
            };

            await _cache.SetAsync(key, dataBytes, options, cancellationToken);

            _logger.LogDebug(
                "Данные сохранены в Redis cache. Ключ: {Key}, Expiration: {Expiration}",
                key,
                options.AbsoluteExpirationRelativeToNow
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при записи в Redis. Ключ: {Key}", key);
        }
    }

    /// <summary>
    ///     Удаление кжша из Redis по определенному ключу.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
            _logger.LogDebug("Удалено из Redis. Ключ: {Key}", key);
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogError(ex, "Ошибка соединения с Redis при удалении ключа: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неизвестная ошибка при удалении из Redis. Ключ: {Key}", key);
        }
    }

    private static T? DeserializeFromUtf8<T>(byte[] data) where T : class
    {
        var json = Encoding.UTF8.GetString(data);
        return JsonSerializer.Deserialize<T>(json);
    }

    private static byte[] SerializeToUtf8Bytes<T>(T value) where T : class
    {
        var jsonString = JsonSerializer.Serialize(value);
        return Encoding.UTF8.GetBytes(jsonString);
    }
}
