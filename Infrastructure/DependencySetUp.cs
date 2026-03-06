using Application.Common.Interfaces;
using Domain.RepositoryAbstraction;
using Infrastructure.Cache;
using Infrastructure.DAL;
using Infrastructure.Identity.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Domain.AbstractServices;
using Serilog.Core;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace Infrastructure;

/// <summary>
/// Статический класс для настройки зависимостей инфраструктурного слоя.
/// </summary>
public static class DependencySetUp
{
    /// <summary>
    /// Добавляет все необходимые сервисы инфраструктурного слоя в <see cref="IServiceCollection"/>.
    /// Настраивает:
    /// - HttpContextAccessor для доступа к HttpContext
    /// - Serilog логирование
    /// - DbContext для SQL Server
    /// - Миграции и заполнение базы данных начальными данными
    /// - Redis кэш
    /// - Сервисы кэша и аутентификации
    /// </summary>
    /// <param name="services">Коллекция сервисов для добавления зависимостей.</param>
    /// <param name="configuration">Конфигурация приложения для чтения строк подключения и настроек.</param>
    /// <exception cref="InvalidOperationException">
    /// Генерируется, если строки подключения к базе данных или Redis не настроены.
    /// </exception>
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Доступ к HttpContext через DI
        services.AddHttpContextAccessor();

        // Настройка Serilog по конфигурации
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        // Добавление Serilog в логирование .NET
        services.AddLogging(logBuilder => logBuilder.AddSerilog(dispose: true));

        // Настройка подключения к SQL Server
        var connectionStringSql = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionStringSql))
            throw new InvalidOperationException(
                "Строка подключения к базе данных 'DefaultConnection' не настроена.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionStringSql));

        // Сервисы для миграции и заполнения базы данных начальными данными
        services.AddScoped<AppDbContextMigrator>();
        services.AddScoped<DataSeed>();

        // Настройка подключения к Redis
        var connectionRedis = configuration.GetConnectionString("Redis");
        if (string.IsNullOrWhiteSpace(connectionRedis))
            throw new InvalidOperationException(
                "Строка подключения к Redis 'DefaultConnection:Redis' не настроена.");

        services.AddStackExchangeRedisCache(opts =>
        {
            opts.Configuration = connectionRedis;
            opts.InstanceName = "TestAirAstana_";
        });

        // Сервис для работы с кэшем
        services.AddScoped<ICacheService, CacheService>();

        // Сервис аутентификации
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRepository, Infrastructure.DAL.Repository>();

        services.AddScoped<ISessionUser, SessionUserService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        var jwtSettings = configuration.GetSection(JwtConfigs.SectionName).Get<JwtConfigs>()
            ?? throw new InvalidOperationException("Настройки JWT не найдены");

        services.Configure<JwtConfigs>(configuration.GetSection(JwtConfigs.SectionName));

        services.AddAuthentication(options =>
        {
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),       
            };
        });
    }
}