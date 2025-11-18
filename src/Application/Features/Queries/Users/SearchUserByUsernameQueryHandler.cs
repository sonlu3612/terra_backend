using Application.DTOs;
using Application.Exceptions;
using Application.Mappers;
using Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Queries.Users
{
    /// <summary>
    /// Handler for SearchUserByUsernameQuery
    /// </summary>
    public class SearchUserByUsernameQueryHandler : IRequestHandler<SearchUserByUsernameQuery, UserProfileDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public SearchUserByUsernameQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserProfileDto> Handle(SearchUserByUsernameQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Username))
            {
                throw new NotFoundException("User", "username is required");
            }

            var user = await _userManager.FindByNameAsync(request.Username);

            if (user == null)
            {
                throw new NotFoundException("User", request.Username);
            }

            return user.ToProfileDto();
        }
    }
}
