using Domain.AbstractServices;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Identity;
public class SessionUserService : ISessionUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                             ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");

    public string? Username => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name)
                               ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("name");

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }
}