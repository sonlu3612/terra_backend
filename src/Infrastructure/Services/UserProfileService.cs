using Microsoft.EntityFrameworkCore;
using Application.DTOs;
using Application.Mappers;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Services;

public class UserProfileService : IUserProfileService
{
    private readonly ApplicationDbContext _dbContext;
    
    public UserProfileService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<object?> GetBasicProfileAsync(string userId)
    {
        var profile = await _dbContext.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId);
        
        return profile == null ? null : UserProfileMapper.ToBasicDto(profile);
    }
    
    public async Task<object?> GetProfileAsync(string userId)
    {
        var profile = await _dbContext.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId);
        
        return profile == null ? null : UserProfileMapper.ToDto(profile);
    }
    
    public async Task<IEnumerable<object>> GetAllProfilesAsync(int pageNumber = 1, int pageSize = 10)
    {
        var profiles = await _dbContext.UserProfiles
            .AsNoTracking()
            .OrderBy(p => p.FirstName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return profiles.Select(UserProfileMapper.ToDto).Cast<object>();
    }
    
    public async Task<object> CreateProfileAsync(string userId, object dto)
    {
        if (dto is not CreateUserProfileDto createDto)
        {
            throw new ArgumentException("Invalid DTO type");
        }
        
        // Validate DTO
        createDto.Validate();
        
        // Check if user already has a profile
        var existingProfile = await _dbContext.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);
        
        if (existingProfile != null)
        {
            throw new InvalidOperationException("User already has a profile");
        }
        
        // Check if user exists
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with id '{userId}' not found");
        }
        
        // Create entity and save
        var userProfile = UserProfileMapper.ToEntity(createDto, userId);
        _dbContext.UserProfiles.Add(userProfile);
        await _dbContext.SaveChangesAsync();
        
        return UserProfileMapper.ToDto(userProfile);
    }
    
    public async Task<object> UpdateProfileAsync(string userId, object dto)
    {
        if (dto is not UpdateUserProfileDto updateDto)
        {
            throw new ArgumentException("Invalid DTO type");
        }
        
        // Validate DTO
        updateDto.Validate();
        
        // Get existing profile
        var profile = await _dbContext.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);
        
        if (profile == null)
        {
            throw new KeyNotFoundException($"Profile for user '{userId}' not found");
        }
        
        // Update entity
        UserProfileMapper.UpdateEntity(profile, updateDto);
        _dbContext.UserProfiles.Update(profile);
        await _dbContext.SaveChangesAsync();
        
        return UserProfileMapper.ToDto(profile);
    }
    
    public async Task DeleteProfileAsync(string userId)
    {
        var profile = await _dbContext.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);
        
        if (profile == null)
        {
            throw new KeyNotFoundException($"Profile for user '{userId}' not found");
        }
        
        _dbContext.UserProfiles.Remove(profile);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<bool> ProfileExistsAsync(string userId)
    {
        return await _dbContext.UserProfiles
            .AnyAsync(p => p.UserId == userId);
    }
}
