using Core.Entities;
using Application.DTOs;

namespace Application.Mappers
{
    /// <summary>
    /// Mapper for converting ApplicationUser entities to DTOs
    /// </summary>
    public static class UserMapper
    {
        /// <summary>
        /// Maps an ApplicationUser to a UserProfileDto
        /// </summary>
        public static UserProfileDto ToProfileDto(this ApplicationUser user)
        {
            ArgumentNullException.ThrowIfNull(user);

            return new UserProfileDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                DisplayName = user.DisplayName ?? string.Empty,
                Bio = user.Bio,
                Theme = user.Theme ?? "light",
                Accent = user.Accent ?? "blue",
                Website = user.Website,
                Location = user.Location,
                PhotoUrl = user.PhotoUrl ?? "/assets/twitter-avatar.jpg",
                CoverPhotoUrl = user.CoverPhotoUrl,
                Verified = user.Verified,
                TotalTweets = user.TotalTweets,
                TotalPhotos = user.TotalPhotos,
                PinnedTweetId = user.PinnedTweetId?.ToString(),
                Following = new(), // Populated by separate endpoints (3.3)
                Followers = new(), // Populated by separate endpoints (3.3)
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}
