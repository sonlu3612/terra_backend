using Core.Common.Enums;

namespace Application.DTOs;

/// <summary>
/// Basic user profile information - contains only firstName and profilePictureUrl
/// </summary>
public class UserProfileBasicDto
{
    public string FirstName { get; set; } = null!;
    
    public string? ProfilePictureUrl { get; set; }
}
