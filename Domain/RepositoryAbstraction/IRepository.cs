using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoryAbstraction;
/// <summary>
/// Абстракция над Data Access Layer.
///
/// Expression<>; не используется, так как является абстракцией,
/// тесно связанной с ORM, Entity Framework Core,
/// и не подходит для использования с Dapper.
///
/// Code smell:
/// Метод "GetByUsernameAsync" не являются
/// гарантированной частью TEntity. В более корректной архитектуре
/// их следует выносить в специализированные репозитории
/// (например IUserRepository и тп).
///
/// Так как задание тестовое и время ограничено,
/// данные методы были добавлены в базовый репозиторий
/// для упрощения реализации.
/// </summary>
/// <typeparam name="TEntity">Тип сущности</typeparam>
public interface IRepository
{
    Task<TEntity?> GetByIdAsync<TEntity>(int id, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity;
    Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(CancellationToken cancellationToken = default)
        where TEntity : BaseEntity;
    Task<TEntity> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : BaseEntity;
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    void Update<TEntity>(TEntity entity) where TEntity : BaseEntity;
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
}