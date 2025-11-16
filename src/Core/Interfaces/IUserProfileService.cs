namespace Core.Interfaces;

public interface IUserProfileService
{
    /// <summary>
    /// Get basic user profile information (firstName + profilePictureUrl)
    /// </summary>
    Task<object?> GetBasicProfileAsync(string userId);
    
    /// <summary>
    /// Get complete user profile information
    /// </summary>
    Task<object?> GetProfileAsync(string userId);
    
    /// <summary>
    /// Get all user profiles with pagination
    /// </summary>
    Task<IEnumerable<object>> GetAllProfilesAsync(int pageNumber = 1, int pageSize = 10);
    
    /// <summary>
    /// Create a new user profile
    /// </summary>
    Task<object> CreateProfileAsync(string userId, object dto);
    
    /// <summary>
    /// Update an existing user profile
    /// </summary>
    Task<object> UpdateProfileAsync(string userId, object dto);
    
    /// <summary>
    /// Delete a user profile
    /// </summary>
    Task DeleteProfileAsync(string userId);
    
    /// <summary>
    /// Check if a user profile exists
    /// </summary>
    Task<bool> ProfileExistsAsync(string userId);
}
