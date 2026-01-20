using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserService.Models;
using UserService.Services;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IProfileService _profileService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, IProfileService profileService, ILogger<UserController> logger)
    {
        _userService = userService;
        _profileService = profileService;
        _logger = logger;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    [HttpGet("profile")]
    public async Task<ActionResult<ProfileResponse>> GetProfile()
    {
        var userId = GetUserId();
        var profile = await _userService.GetProfileByUserIdAsync(userId);

        if (profile == null)
            return NotFound(new { message = "Profile not found" });

        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<ActionResult<ProfileResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = GetUserId();

        try
        {
            var profile = await _userService.UpdateProfileAsync(userId, request);
            return Ok(profile);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("profile/image")]
    public async Task<ActionResult<string>> UploadProfileImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded" });

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            return BadRequest(new { message = "Invalid file type. Only images are allowed." });

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(new { message = "File size must be less than 5MB" });

        var userId = GetUserId();

        try
        {
            using var stream = file.OpenReadStream();
            var imageUrl = await _profileService.UploadProfileImageAsync(userId, stream, file.FileName);
            return Ok(new { imageUrl });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("notifications")]
    public async Task<ActionResult<List<NotificationDto>>> GetNotifications([FromQuery] bool unreadOnly = false)
    {
        var userId = GetUserId();
        var notifications = await _userService.GetNotificationsAsync(userId, unreadOnly);
        return Ok(notifications);
    }

    [HttpPut("notifications/{notificationId}/read")]
    public async Task<ActionResult> MarkNotificationAsRead(Guid notificationId)
    {
        var userId = GetUserId();
        var success = await _userService.MarkNotificationAsReadAsync(notificationId, userId);

        if (!success)
            return NotFound(new { message = "Notification not found" });

        return NoContent();
    }

    [HttpPost("addresses")]
    public async Task<ActionResult<AddressDto>> AddAddress([FromBody] AddressDto addressDto)
    {
        var userId = GetUserId();

        try
        {
            var address = await _profileService.AddAddressAsync(userId, addressDto);
            return CreatedAtAction(nameof(GetProfile), new { }, address);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("addresses/{addressId}")]
    public async Task<ActionResult<AddressDto>> UpdateAddress(Guid addressId, [FromBody] AddressDto addressDto)
    {
        var userId = GetUserId();

        try
        {
            var address = await _profileService.UpdateAddressAsync(userId, addressId, addressDto);
            return Ok(address);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("addresses/{addressId}")]
    public async Task<ActionResult> DeleteAddress(Guid addressId)
    {
        var userId = GetUserId();
        var success = await _profileService.DeleteAddressAsync(userId, addressId);

        if (!success)
            return NotFound(new { message = "Address not found" });

        return NoContent();
    }
}
