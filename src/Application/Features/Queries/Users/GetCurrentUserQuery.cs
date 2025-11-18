using Application.DTOs;
using MediatR;

namespace Application.Features.Queries.Users
{
    /// <summary>
    /// Query to get the current user profile (requires authentication)
    /// </summary>
    public class GetCurrentUserQuery : IRequest<UserProfileDto>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
