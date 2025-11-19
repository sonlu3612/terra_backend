using Application.DTOs;
using Application.Exceptions;
using Application.Mappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Core.Entities;

namespace Application.Features.Queries.Users
{
    /// <summary>
    /// Handler for GetCurrentUserQuery
    /// </summary>
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserProfileDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetCurrentUserQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserProfileDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                throw new NotFoundException("User", "current");
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
