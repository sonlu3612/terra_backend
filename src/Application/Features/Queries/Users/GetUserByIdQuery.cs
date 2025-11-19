using Application.DTOs;
using MediatR;

namespace Application.Features.Queries.Users
{
    /// <summary>
    /// Query to get a user by ID
    /// </summary>
    public class GetUserByIdQuery : IRequest<UserProfileDto>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
