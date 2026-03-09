using Domain.Entities;

namespace Domain.RepositoryAbstraction;
public interface IFlightRepository
{
    Task<Flight?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Flight>> WhereAsync(string? origin, string? destination, bool arrivalAsc = true, CancellationToken cancellationToken = default);
    Task<Flight> AddAsync(Flight flight, CancellationToken cancellationToken = default);
    Task UpdateAsync(Flight flight, CancellationToken cancellationToken = default);
}
