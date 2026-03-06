using Domain.Entities;
using Domain.RepositoryAbstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DAL;
internal class Repository : IRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<Repository> _logger;

    public Repository(AppDbContext dbContext, ILogger<Repository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<TEntity> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : BaseEntity
    {
        try
        {
            await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
            _logger.LogInformation("Добавлен новый объект {EntityType} с Id={EntityId}", typeof(TEntity).Name, entity.Id);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении объекта {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(CancellationToken cancellationToken = default)
        where TEntity : BaseEntity
    {
        try
        {
            var result = await _dbContext.Set<TEntity>().ToListAsync(cancellationToken);
            _logger.LogDebug("Получено {Count} объектов {EntityType}", result.Count, typeof(TEntity).Name);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении всех объектов {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    public async Task<TEntity?> GetByIdAsync<TEntity>(int id, CancellationToken cancellationToken = default)
        where TEntity : BaseEntity
    {
        try
        {
            var entity = await _dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (entity is null)
                _logger.LogWarning("{EntityType} с Id={EntityId} не найден", typeof(TEntity).Name, id);
            else
                _logger.LogDebug("{EntityType} с Id={EntityId} найден", typeof(TEntity).Name, id);

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при поиске {EntityType} с Id={EntityId}", typeof(TEntity).Name, id);
            throw;
        }
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
            if (user is null)
                _logger.LogWarning("Пользователь с username={Username} не найден", username);
            else
                _logger.LogDebug("Пользователь с username={Username} найден", username);

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при поиске пользователя с username={Username}", username);
            throw;
        }
    }

    public void Update<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        try
        {
            _dbContext.Set<TEntity>().Update(entity);
            _logger.LogInformation("{EntityType} с Id={EntityId} обновлен", typeof(TEntity).Name, entity.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении {EntityType} с Id={EntityId}", typeof(TEntity).Name, entity.Id);
            throw;
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("Транзакция базы данных начата");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при начале транзакции");
            throw;
        }
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbContext.Database.CommitTransactionAsync(cancellationToken);
            _logger.LogDebug("Транзакция базы данных успешно зафиксирована");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при фиксации транзакции");
            throw;
        }
    }
}