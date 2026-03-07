using Domain.Entities;
using Domain.RepositoryAbstraction;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DAL.Repositories;
public class FlightRepository : IFlightRepository
{
    private readonly AppDbContext _context;

    public FlightRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Flight?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _context.Flights.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public async Task<Flight> AddAsync(Flight flight, CancellationToken cancellationToken = default)
    {
        await _context.Flights.AddAsync(flight, cancellationToken);
        return flight;
    }

    public Task UpdateAsync(Flight flight, CancellationToken cancellationToken = default)
    {
        _context.Flights.Update(flight);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Flight>> WhereAsync(string? origin, string? destination, bool arrivalAsc = true, CancellationToken cancellationToken = default)
    {
        IQueryable<Flight> queryable = _context.Flights;

        if (!string.IsNullOrEmpty(origin))
        {
            queryable = queryable.Where(x => EF.Functions.Like(x.Origin, $"%{origin}%"));
        }
        if (!string.IsNullOrEmpty(destination))
        {
            queryable = queryable.Where(x => EF.Functions.Like(x.Destination, $"%{destination}%"));
        }
        if (arrivalAsc)
        {
            queryable = queryable.OrderBy(x => x.Arrival);
        }

        return await queryable.ToListAsync();
    }
}