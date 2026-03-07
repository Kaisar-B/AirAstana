using Domain.RepositoryAbstraction;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.DAL.Repositories;
internal class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _dbContext;
    private IDbContextTransaction? _dbContextTransaction;
    private readonly IFlightRepository _flightRepository;

    public UnitOfWork(AppDbContext dbContext, IFlightRepository flightRepository)
    {
        _dbContext = dbContext;
        _flightRepository = flightRepository;
    }
    public IFlightRepository Flights => _flightRepository;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _dbContextTransaction =  await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbContextTransaction?.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await RollbackTransactionAsync();
            throw ex;
        }
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _dbContextTransaction?.Dispose();
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _dbContextTransaction?.RollbackAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
