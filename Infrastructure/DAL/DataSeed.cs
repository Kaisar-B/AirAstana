using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.DAL;

/// <summary>
/// Класс для инициализации базы данных начальными данными.
/// Выполняет применение всех ожидающих миграций и заполнение таблиц Flights, Roles и Users.
/// </summary>
public class DataSeed
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<DataSeed> _logger;
    private readonly AppDbContextMigrator _initializer;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Конструктор класса DataSeed.
    /// </summary>
    /// <param name="dbContext">Контекст базы данных приложения.</param>
    /// <param name="passwordHasher">Сервис хэширования паролей.</param>
    /// <param name="logger">Логгер для вывода сообщений.</param>
    /// <param name="initializer">Сервис для применения миграций к базе данных.</param>
    public DataSeed(AppDbContext dbContext, IPasswordHasher passwordHasher, ILogger<DataSeed> logger, AppDbContextMigrator initializer, TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _initializer = initializer;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Применяет все ожидающие миграции и заполняет базу данных начальными данными.
    /// </summary>
    /// <param name="builder">ModelBuilder для EF Core (может использоваться для конфигурации моделей при необходимости).</param>
    public async Task SetDataBaseAsync()
    {
        try
        {
            // Применяем все ожидающие миграции
            await _initializer.ApplyMigrationsAsync();

            // Заполняем таблицы начальными данными
            await InitFlightsAsync();
            await InitRolesAsync();
            await InitUsersAsync();

            _logger.LogInformation("Инициализация базы данных завершена успешно.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при заполнении базы данных начальными данными.");
            throw;
        }
    }

    /// <summary>
    /// Инициализация таблицы рейсов начальными данными.
    /// Пропускается, если таблица уже содержит данные.
    /// </summary>
    private async Task InitFlightsAsync()
    {
        if (await _dbContext.Flights.AnyAsync())
        {
            _logger.LogInformation("Рейсы уже присутствуют в базе, пропускаем создание.");
            return;
        }

        var flights = new List<Flight>
        {
            new Flight { Origin = "Тараз", Destination = "Актау", Departure = _timeProvider.GetLocalNow().AddDays(1).AddHours(7), Arrival = _timeProvider.GetLocalNow().AddDays(1).AddHours(9), Status = Status.InTime, Created = _timeProvider.GetLocalNow(), CreatedBy = "System" },
            new Flight { Origin = "Актау", Destination = "Актобе", Departure = _timeProvider.GetLocalNow().AddDays(1).AddHours(12), Arrival = _timeProvider.GetLocalNow().AddDays(1).AddHours(14), Status = Status.Delayed, Created = _timeProvider.GetLocalNow(), CreatedBy = "System" },
            new Flight { Origin = "Актобе", Destination = "Кокшетау", Departure = _timeProvider.GetLocalNow().AddDays(2).AddHours(6), Arrival = _timeProvider.GetLocalNow().AddDays(2).AddHours(8), Status = Status.Cancelled, Created = _timeProvider.GetLocalNow(), CreatedBy = "System" },
            new Flight { Origin = "Кокшетау", Destination = "Семей", Departure = _timeProvider.GetLocalNow().AddDays(2).AddHours(15), Arrival = _timeProvider.GetLocalNow().AddDays(2).AddHours(17), Status = Status.InTime, Created = _timeProvider.GetLocalNow(), CreatedBy = "System" },
            new Flight { Origin = "Семей", Destination = "Туркестан", Departure = _timeProvider.GetLocalNow().AddDays(3).AddHours(10), Arrival = _timeProvider.GetLocalNow().AddDays(3).AddHours(13), Status = Status.InTime, Created = _timeProvider.GetLocalNow(), CreatedBy = "System" }
        };

        // Сохраняем новые рейсы в базу
        await _dbContext.Flights.AddRangeAsync(flights);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Добавлено {Count} новых рейсов в базу данных.", flights.Count);
    }

    /// <summary>
    /// Инициализация таблицы ролей начальными данными.
    /// Пропускается, если таблица уже содержит данные.
    /// </summary>
    private async Task InitRolesAsync()
    {
        if (await _dbContext.Roles.AnyAsync())
        {
            _logger.LogInformation("Роли уже существуют, пропускаем их создание.");
            return;
        }

        var roles = new List<Role>
        {
            new Role { Code = "User", Created = _timeProvider.GetLocalNow(), CreatedBy="System"},
            new Role { Code = "Admin", Created = _timeProvider.GetLocalNow(), CreatedBy="System"}
        };

        await _dbContext.Roles.AddRangeAsync(roles);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Добавлено {Count} ролей в базу данных.", roles.Count);
    }

    /// <summary>
    /// Инициализация таблицы пользователей начальными данными.
    /// Пропускается, если таблица уже содержит данные.
    /// </summary>
    private async Task InitUsersAsync()
    {
        if (await _dbContext.Users.AnyAsync())
        {
            _logger.LogInformation("Пользователи уже существуют, пропускаем создание.");
            return;
        }

        var adminRole = await _dbContext.Roles.FirstAsync(r => r.Code == "Admin");
        var userRole = await _dbContext.Roles.FirstAsync(r => r.Code == "User");

        var users = new List<User>
        {
            new User { Username = "systemadmin", PasswordHashed = _passwordHasher.HashPassword("admin!234"), RoleId = adminRole.Id, Created = _timeProvider.GetLocalNow(), CreatedBy = "System" },
            new User { Username = "standarduser", PasswordHashed = _passwordHasher.HashPassword("user!234"), RoleId = userRole.Id, Created = _timeProvider.GetLocalNow(), CreatedBy = "System" }
        };

        // Сохраняем новых пользователей
        await _dbContext.Users.AddRangeAsync(users);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Добавлено {Count} пользователей в базу данных.", users.Count);
        _logger.LogInformation("Данные для входа по умолчанию:");
        _logger.LogInformation("Admin: systemadmin / admin!234");
        _logger.LogInformation("User: standarduser / user!234");
    }
}