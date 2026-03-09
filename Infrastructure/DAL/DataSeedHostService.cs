using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.DAL;
internal class DataSeedHostService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DataSeedHostService> _logger;

    public DataSeedHostService(
        IServiceScopeFactory scopeFactory,
        ILogger<DataSeedHostService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    // Метод вызывается при запуске приложения
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запуск инициализации базы данных (DataSeed).");

        try
        {
            // Создаем scope чтобы получить Scoped сервисы (DbContext, репозитории и т.д.)
            using var scope = _scopeFactory.CreateScope();

            // Получаем сервис DataSeed из DI
            var dataSeed = scope.ServiceProvider.GetRequiredService<DataSeed>();

            // Запускаем наполнение базы
            await dataSeed.SetDataBaseAsync();

            _logger.LogInformation("Инициализация базы данных успешно завершена.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при инициализации базы данных.");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("DataSeedHostedService остановлен.");
        return Task.CompletedTask;
    }
}