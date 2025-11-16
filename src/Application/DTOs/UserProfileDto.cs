using Core.Common.Enums;

namespace Application.DTOs;

/// <summary>
/// Complete user profile information - contains all profile details
/// </summary>
public class UserProfileDto
{
    public int Id { get; set; }
    
    public string UserId { get; set; } = null!;
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public DateTime DateOfBirth { get; set; }
    
    public string? Bio { get; set; }
    
    public string? ProfilePictureUrl { get; set; }
    
    public Gender Gender { get; set; }
}
