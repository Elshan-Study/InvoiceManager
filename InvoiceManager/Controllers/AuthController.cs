using InvoiceManager.Common;
using InvoiceManager.DTOs.AuthDto;
using InvoiceManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="dto">User registration data.</param>
    /// <returns>Access and refresh tokens for the newly created user.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var res = await _authService.RegisterAsync(dto);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(res, "Registered"));
        }
        catch (System.Exception ex)
        {
            return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Authenticates a user and generates access and refresh tokens.
    /// </summary>
    /// <param name="dto">User login credentials.</param>
    /// <returns>Access and refresh tokens if credentials are valid.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var res = await _authService.LoginAsync(dto, ip);

        if (res == null)
            return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid credentials"));

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(res, "Logged in"));
    }

    /// <summary>
    /// Generates a new access token using a valid refresh token.
    /// </summary>
    /// <param name="dto">Refresh token payload.</param>
    /// <returns>New access and refresh tokens.</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto dto)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var res = await _authService.RefreshAsync(dto.RefreshToken, ip);

        if (res == null)
            return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid refresh token"));

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(res, "Token refreshed"));
    }

    /// <summary>
    /// Revokes an existing refresh token.
    /// </summary>
    /// <param name="dto">Refresh token to revoke.</param>
    /// <returns>Confirmation of revocation.</returns>
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RefreshDto dto)
    {
        var ok = await _authService.RevokeRefreshTokenAsync(dto.RefreshToken);

        if (!ok)
            return BadRequest(ApiResponse<object>.ErrorResponse("Token not found or already revoked"));

        return Ok(ApiResponse<object>.SuccessResponse(null, "Token revoked"));
    }
}
