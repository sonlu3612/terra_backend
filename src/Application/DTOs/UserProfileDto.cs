namespace Application.DTOs
{
    /// <summary>
    /// Data transfer object for user profile information
    /// </summary>
    public class UserProfileDto
    {
        public string Id { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string? Bio { get; set; }

        public string Theme { get; set; } = "light";

        public string Accent { get; set; } = "blue";

        public string? Website { get; set; }

        public string? Location { get; set; }

        public string PhotoUrl { get; set; } = "/assets/twitter-avatar.jpg";

        public string? CoverPhotoUrl { get; set; }

        public bool Verified { get; set; }

        public int TotalTweets { get; set; }

        public int TotalPhotos { get; set; }

        public string? PinnedTweetId { get; set; }

        /// <summary>
        /// Empty arrays for now - followers/following populated by separate endpoints
        /// </summary>
        public List<string> Following { get; set; } = new();

        /// <summary>
        /// Empty arrays for now - followers/following populated by separate endpoints
        /// </summary>
        public List<string> Followers { get; set; } = new();

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
