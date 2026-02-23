using InvoiceManager.Common;
using InvoiceManager.DTOs.AuthDto;
using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InvoiceManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;
    public UsersController(IAuthService authService) => _authService = authService;

    private int GetCurrentUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Returns the profile information of the currently authenticated user.
    /// </summary>
    /// <returns>User profile data.</returns>
    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _authService.GetUserByIdAsync(GetCurrentUserId());
        if (user == null)
            return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

        var profile = new UserProfileDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Address = user.Address,
            PhoneNumber = user.PhoneNumber,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return Ok(ApiResponse<UserProfileDto>.SuccessResponse(profile, "Profile retrieved successfully"));
    }

    /// <summary>
    /// Updates profile information of the currently authenticated user.
    /// </summary>
    /// <param name="dto">Updated profile data.</param>
    /// <returns>Updated profile information.</returns>
    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var user = await _authService.UpdateProfileAsync(GetCurrentUserId(), dto);
        if (user == null)
            return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

        var profile = new UserProfileDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Address = user.Address,
            PhoneNumber = user.PhoneNumber,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return Ok(ApiResponse<UserProfileDto>.SuccessResponse(profile, "Profile updated successfully"));
    }

    /// <summary>
    /// Changes the password of the currently authenticated user.
    /// </summary>
    /// <param name="dto">Old and new password.</param>
    /// <returns>Confirmation of password change.</returns>
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var ok = await _authService.ChangePasswordAsync(GetCurrentUserId(), dto.OldPassword, dto.NewPassword);
        if (!ok) return BadRequest(ApiResponse<object>.ErrorResponse("Old password is incorrect"));
        return Ok(ApiResponse<object>.SuccessResponse(null, "Password changed"));
    }
}
