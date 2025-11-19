using Application.DTOs;
using Application.Exceptions;
using Application.Mappers;
using Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Queries.Users
{
    /// <summary>
    /// Handler for GetUserByIdQuery
    /// </summary>
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserProfileDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserProfileDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                throw new NotFoundException("User", "invalid");
            }

            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                throw new NotFoundException("User", request.UserId);
            }

            return user.ToProfileDto();
        }
    }
}
