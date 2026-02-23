using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace InvoiceManager.Services;

public class TokenService
{
    private readonly IConfiguration _config;
    public TokenService(IConfiguration config) => _config = config;

    public string CreateAccessToken(User user, out DateTimeOffset expiresAt)
    {
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not set"));
        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        var accessMinutes = int.Parse(_config["Jwt:AccessTokenMinutes"] ?? "15");
        expiresAt = DateTimeOffset.UtcNow.AddMinutes(accessMinutes);

        var claims = new[]
        {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email)
            };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken CreateRefreshToken(string? createdByIp = null)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        var refresh = new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            CreatedAt = DateTimeOffset.UtcNow,
            Expires = DateTimeOffset.UtcNow.AddDays(int.Parse(_config["Jwt:RefreshTokenDays"] ?? "30")),
            CreatedByIp = createdByIp
        };
        return refresh;
    }
}
