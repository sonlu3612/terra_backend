using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Core.Interfaces;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;
    
    public UserProfileController(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }
    
    /// <summary>
    /// Get basic profile information for a specific user
    /// </summary>
    [HttpGet("basic/{userId}")]
    public async Task<ActionResult<UserProfileBasicDto>> GetBasicProfile(string userId)
    {
        try
        {
            var profile = await _userProfileService.GetBasicProfileAsync(userId);
            if (profile == null)
            {
                return NotFound(new { message = "User profile not found" });
            }
            
            return Ok(profile);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }
    
    /// <summary>
    /// Get complete profile information for a specific user (requires authentication)
    /// </summary>
    [Authorize]
    [HttpGet("{userId}")]
    public async Task<ActionResult<UserProfileDto>> GetProfile(string userId)
    {
        try
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            // Allow users to view their own profile or admins to view all profiles
            if (currentUserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            
            var profile = await _userProfileService.GetProfileAsync(userId);
            if (profile == null)
            {
                return NotFound(new { message = "User profile not found" });
            }
            
            return Ok(profile);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }
    
    /// <summary>
    /// Get all user profiles with pagination (requires authentication and admin role)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserProfileDto>>> GetAllProfiles([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest(new { message = "Page number and page size must be greater than 0" });
            }
            
            var profiles = await _userProfileService.GetAllProfilesAsync(pageNumber, pageSize);
            return Ok(profiles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }
    
    /// <summary>
    /// Create a new user profile (requires authentication)
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<UserProfileDto>> CreateProfile([FromBody] CreateUserProfileDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            // Users can only create their own profile
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }
            
            var profile = await _userProfileService.CreateProfileAsync(currentUserId, dto);
            return CreatedAtAction(nameof(GetProfile), new { userId = currentUserId }, profile);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }
    
    /// <summary>
    /// Update an existing user profile (requires authentication)
    /// </summary>
    [Authorize]
    [HttpPut("{userId}")]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile(string userId, [FromBody] UpdateUserProfileDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            // Users can only update their own profile
            if (currentUserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            
            var profile = await _userProfileService.UpdateProfileAsync(userId, dto);
            return Ok(profile);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }
    
    /// <summary>
    /// Delete a user profile (requires authentication)
    /// </summary>
    [Authorize]
    [HttpDelete("{userId}")]
    public async Task<ActionResult> DeleteProfile(string userId)
    {
        try
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            // Users can only delete their own profile
            if (currentUserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            
            await _userProfileService.DeleteProfileAsync(userId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }
}
