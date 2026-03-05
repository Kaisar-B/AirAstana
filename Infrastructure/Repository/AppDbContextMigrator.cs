using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repository;

/// <summary>
/// Применяет все ожидающие миграции к базе данных при старте приложения.
/// </summary>
public class AppDbContextMigrator
{
    private readonly AppDbContext _context;
    private readonly ILogger<AppDbContextMigrator> _logger;

    public AppDbContextMigrator(
        AppDbContext context,
        ILogger<AppDbContextMigrator> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ApplyMigrationsAsync()
    {
        try
        {
            var pendingMigrations = _context.Database.GetPendingMigrations().ToList();

            if (pendingMigrations.Any())
            {
                _logger.LogInformation("Применение ожидающих миграций к базе данных...");
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Все миграции успешно применены.");
            }
            else
            {
                _logger.LogInformation("Нет ожидающих миграций для применения.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при применении миграций.");
            throw;
        }
    }
}
