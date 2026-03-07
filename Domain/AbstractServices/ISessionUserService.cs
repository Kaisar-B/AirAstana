namespace Domain.AbstractServices;
public interface ISessionUserService
{
    string? UserId { get; }
    string? Username { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}