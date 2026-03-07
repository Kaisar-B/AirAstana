using Domain.Entities;

namespace Domain.RepositoryAbstraction;
public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

}
