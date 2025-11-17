using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// User's display name (full name)
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// User bio/description
        /// </summary>
        public string? Bio { get; set; }

        /// <summary>
        /// Theme preference: light, dim, or lights-out
        /// </summary>
        public string Theme { get; set; } = "light";

        /// <summary>
        /// Accent color: blue, yellow, pink, purple, orange, green
        /// </summary>
        public string Accent { get; set; } = "blue";

        /// <summary>
        /// User's website URL
        /// </summary>
        public string? Website { get; set; }

        /// <summary>
        /// User's location
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// User's profile photo URL
        /// </summary>
        public string PhotoUrl { get; set; } = "/assets/twitter-avatar.jpg";

        /// <summary>
        /// User's cover photo URL
        /// </summary>
        public string? CoverPhotoUrl { get; set; }

        /// <summary>
        /// Whether the user is verified
        /// </summary>
        public bool Verified { get; set; } = false;

        /// <summary>
        /// Total number of tweets created by this user
        /// </summary>
        public int TotalTweets { get; set; } = 0;

        /// <summary>
        /// Total number of tweets with media created by this user
        /// </summary>
        public int TotalPhotos { get; set; } = 0;

        /// <summary>
        /// ID of the pinned tweet (if any)
        /// </summary>
        public Guid? PinnedTweetId { get; set; }

        /// <summary>
        /// Timestamp of when the account was created
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Timestamp of the last update to the account
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }

        /// <summary>
        /// Email verification timestamp
        /// </summary>
        public DateTimeOffset? EmailVerifiedAt { get; set; }
    }
}