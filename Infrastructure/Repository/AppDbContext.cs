using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

/// <summary>
/// Контекст базы данных приложения.
/// Содержит DbSet для сущностей Flight, Role и User,
/// а также конфигурацию моделей и правил конвенций.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Конструктор контекста базы данных с параметрами конфигурации.
    /// </summary>
    /// <param name="options">Параметры DbContext для конфигурации подключения и поведения.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public DbSet<Flight> Flights { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Конфигурирует модели при создании модели данных.
    /// Автоматически применяет все конфигурации из текущей сборки.
    /// </summary>
    /// <param name="modelBuilder">Builder для конфигурации моделей.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Настраивает конвенции EF Core.
    /// Преобразует все свойства типа Enum в строковое представление в базе данных.
    /// </summary>
    /// <param name="configurationBuilder">Builder для настройки конвенций свойств.</param>
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Enum>().HaveConversion<string>();
    }
}