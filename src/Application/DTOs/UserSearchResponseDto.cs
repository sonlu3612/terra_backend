namespace Application.DTOs
{
    /// <summary>
    /// Response wrapper for user search results (404 handling)
    /// </summary>
    public class UserSearchResponseDto
    {
        public UserProfileDto? User { get; set; }

        public bool Found { get; set; }

        public string? Message { get; set; }
    }
}
