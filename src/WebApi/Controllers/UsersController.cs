using Application.DTOs;
using Application.Features.Queries.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{
    /// <summary>
    /// Controller for user profile operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get the current authenticated user's profile
        /// GET /api/users/me
        /// </summary>
        [HttpGet("me")]
        [Produces(typeof(UserProfileDto))]
        public async Task<ActionResult<UserProfileDto>> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User is not authenticated" });
            }

            var query = new GetCurrentUserQuery { UserId = userId };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Search for a user by username
        /// GET /api/users?username=john_doe
        /// </summary>
        [HttpGet]
        [Produces(typeof(UserProfileDto))]
        public async Task<ActionResult<UserProfileDto>> SearchUserByUsername([FromQuery] string? username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { message = "Username query parameter is required" });
            }

            var query = new SearchUserByUsernameQuery { Username = username.Trim() };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Get a user profile by ID
        /// GET /api/users/{id}
        /// </summary>
        [HttpGet("{id}")]
        [Produces(typeof(UserProfileDto))]
        public async Task<ActionResult<UserProfileDto>> GetUserById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { message = "User ID is required" });
            }

            var query = new GetUserByIdQuery { UserId = id.Trim() };
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
