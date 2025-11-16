using Core.Common.Enums;

namespace Core.Entities;

public class UserProfile
{
    public int Id { get; set; }
    
    /// <summary>
    /// Foreign key to ApplicationUser
    /// </summary>
    public string UserId { get; set; } = null!;
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public DateTime DateOfBirth { get; set; }
    
    public string? Bio { get; set; }
    
    public string? ProfilePictureUrl { get; set; }
    
    public Gender Gender { get; set; } = Gender.Other;
}
