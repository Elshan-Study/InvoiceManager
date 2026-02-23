using InvoiceManager.Data;
using InvoiceManager.DTOs.AuthDto;
using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManager.Services;

public class AuthService : IAuthService
{
    private readonly TaskFlowDbContext _db;
    private readonly TokenService _tokenService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(TaskFlowDbContext db, TokenService tokenService, IPasswordHasher<User> passwordHasher)
    {
        _db = db;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            throw new System.Exception("Email already taken");

        var user = new User
        {
            Email = dto.Email,
            Name = dto.Name,
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
        user.CreatedAt = DateTimeOffset.UtcNow;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // create tokens
        var access = _tokenService.CreateAccessToken(user, out var accessExpiresAt);
        var refresh = _tokenService.CreateRefreshToken();
        refresh.UserId = user.Id;
        _db.RefreshTokens.Add(refresh);
        await _db.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = access,
            RefreshToken = refresh.Token,
            AccessTokenExpiresAt = accessExpiresAt
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto, string? ipAddress = null)
    {
        var user = await _db.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null) return null;

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed) return null;

        var access = _tokenService.CreateAccessToken(user, out var accessExpiresAt);
        var refresh = _tokenService.CreateRefreshToken(ipAddress);
        refresh.UserId = user.Id;

        _db.RefreshTokens.Add(refresh);
        await _db.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = access,
            RefreshToken = refresh.Token,
            AccessTokenExpiresAt = accessExpiresAt
        };
    }

    public async Task<AuthResponseDto?> RefreshAsync(string refreshToken, string? ipAddress = null)
    {
        var rt = await _db.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(r => r.Token == refreshToken);
        if (rt == null || rt.RevokedAt != null || rt.Expires <= DateTimeOffset.UtcNow) return null;

        // revoke old
        rt.RevokedAt = DateTimeOffset.UtcNow;

        // create new refresh
        var newRt = _tokenService.CreateRefreshToken(ipAddress);
        newRt.UserId = rt.UserId;
        rt.ReplacedByToken = newRt.Token;

        _db.RefreshTokens.Add(newRt);
        await _db.SaveChangesAsync();

        var access = _tokenService.CreateAccessToken(rt.User!, out var accessExpiresAt);

        return new AuthResponseDto
        {
            AccessToken = access,
            RefreshToken = newRt.Token,
            AccessTokenExpiresAt = accessExpiresAt
        };
    }

    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
    {
        var rt = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
        if (rt == null || rt.RevokedAt != null) return false;
        rt.RevokedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<User?> GetUserByIdAsync(int id) => await _db.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return false;

        var res = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, oldPassword);
        if (res == PasswordVerificationResult.Failed) return false;

        user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
        user.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<User?> UpdateProfileAsync(int userId, UpdateProfileDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return null;
        user.Name = dto.Name;
        user.Address = dto.Address;
        user.PhoneNumber = dto.PhoneNumber;
        user.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync();
        return user;
    }
}

