using InvoiceManager.DTOs.AuthDto;
using InvoiceManager.Models;

namespace InvoiceManager.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto, string? ipAddress = null);
    Task<AuthResponseDto?> RefreshAsync(string refreshToken, string? ipAddress = null);
    Task<bool> RevokeRefreshTokenAsync(string refreshToken);
    Task<User?> GetUserByIdAsync(int id);
    Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    Task<User?> UpdateProfileAsync(int userId, UpdateProfileDto dto);
}
