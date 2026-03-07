using Domain.Entities;
using Domain.RepositoryAbstraction;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DAL.Repositories;
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

}