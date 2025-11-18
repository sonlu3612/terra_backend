using Application.DTOs;
using MediatR;

namespace Application.Features.Queries.Users
{
    /// <summary>
    /// Query to search for a user by username
    /// </summary>
    public class SearchUserByUsernameQuery : IRequest<UserProfileDto>
    {
        public string Username { get; set; } = string.Empty;
    }
}
