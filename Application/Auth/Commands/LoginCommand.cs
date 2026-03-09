using Application.Auth.Models;
using MediatR;

namespace Application.Auth.Commands;
public class LoginCommand : IRequest<AuthResponse>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
