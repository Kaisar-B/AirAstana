using Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Identity.Authentication;
public class AuthService : IAuthService
{
    private readonly JwtConfigs _jwtConfigs;
    private readonly SymmetricSecurityKey _signingKey;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public AuthService(IOptions<JwtConfigs> jwtSettings)
    {
        _jwtConfigs = jwtSettings.Value;
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfigs.Secret));
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public string GenerateJwtToken(string userId, string username, string role)
    {
        var claims = CreateClaims(userId, username, role);

        var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtConfigs.Issuer,
            audience: _jwtConfigs.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtConfigs.ExpirationInHours),
            signingCredentials: credentials
        );

        return _tokenHandler.WriteToken(token);
    }

    public bool ValidateToken(string token)
    {
        try
        {
            _tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                ValidateIssuer = true,
                ValidIssuer = _jwtConfigs.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtConfigs.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private static Claim[] CreateClaims(string userId, string username, string role) => new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, userId),
        new Claim(JwtRegisteredClaimNames.Name, username),
        new Claim(ClaimTypes.Role, role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
}
