using System.ComponentModel.DataAnnotations;
using Core.Common.Enums;

namespace Application.DTOs;

/// <summary>
/// Request DTO for creating a new user profile
/// </summary>
public class CreateUserProfileDto
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 100 characters")]
    public string FirstName { get; set; } = null!;
    
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 100 characters")]
    public string LastName { get; set; } = null!;
    
    [Required(ErrorMessage = "Date of birth is required")]
    public DateTime DateOfBirth { get; set; }
    
    [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
    public string? Bio { get; set; }
    
    [Url(ErrorMessage = "Profile picture URL must be a valid URL")]
    public string? ProfilePictureUrl { get; set; }
    
    [Required(ErrorMessage = "Gender is required")]
    public Gender Gender { get; set; }
    
    public void Validate()
    {
        if (DateOfBirth > DateTime.UtcNow.Date)
        {
            throw new ArgumentException("Date of birth cannot be in the future");
        }
    }
}
