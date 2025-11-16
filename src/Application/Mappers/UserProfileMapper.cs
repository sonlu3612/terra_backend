using Application.DTOs;
using Core.Entities;

namespace Application.Mappers;

public class UserProfileMapper
{
    /// <summary>
    /// Maps UserProfile entity to UserProfileBasicDto (firstName + profilePictureUrl only)
    /// </summary>
    public static UserProfileBasicDto ToBasicDto(UserProfile userProfile)
    {
        return new UserProfileBasicDto
        {
            FirstName = userProfile.FirstName,
            ProfilePictureUrl = userProfile.ProfilePictureUrl
        };
    }
    
    /// <summary>
    /// Maps UserProfile entity to UserProfileDto (complete profile)
    /// </summary>
    public static UserProfileDto ToDto(UserProfile userProfile)
    {
        return new UserProfileDto
        {
            Id = userProfile.Id,
            UserId = userProfile.UserId,
            FirstName = userProfile.FirstName,
            LastName = userProfile.LastName,
            DateOfBirth = userProfile.DateOfBirth,
            Bio = userProfile.Bio,
            ProfilePictureUrl = userProfile.ProfilePictureUrl,
            Gender = userProfile.Gender
        };
    }
    
    /// <summary>
    /// Maps CreateUserProfileDto to UserProfile entity
    /// </summary>
    public static UserProfile ToEntity(CreateUserProfileDto dto, string userId)
    {
        return new UserProfile
        {
            UserId = userId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            Bio = dto.Bio,
            ProfilePictureUrl = dto.ProfilePictureUrl,
            Gender = dto.Gender
        };
    }
    
    /// <summary>
    /// Maps UpdateUserProfileDto to existing UserProfile entity
    /// </summary>
    public static void UpdateEntity(UserProfile userProfile, UpdateUserProfileDto dto)
    {
        if (!string.IsNullOrEmpty(dto.FirstName))
            userProfile.FirstName = dto.FirstName;
            
        if (!string.IsNullOrEmpty(dto.LastName))
            userProfile.LastName = dto.LastName;
            
        if (dto.DateOfBirth.HasValue)
            userProfile.DateOfBirth = dto.DateOfBirth.Value;
            
        if (!string.IsNullOrEmpty(dto.Bio))
            userProfile.Bio = dto.Bio;
            
        if (!string.IsNullOrEmpty(dto.ProfilePictureUrl))
            userProfile.ProfilePictureUrl = dto.ProfilePictureUrl;
            
        if (dto.Gender.HasValue)
            userProfile.Gender = dto.Gender.Value;
    }
}
