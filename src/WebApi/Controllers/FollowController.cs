using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Entities;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private readonly IFollowService _followService;
        private readonly UserManager<ApplicationUser> _userManager;

        public FollowController(IFollowService followService, UserManager<ApplicationUser> userManager)
        {
            _followService = followService;
            _userManager = userManager;
        }

        private string CurrentUserId => _userManager.GetUserId(User);

        [HttpPost("{userId}")]
        public async Task<IActionResult> Follow(string userId)
        {
            await _followService.FollowAsync(CurrentUserId, userId);
            return Ok(new { message = "Followed successfully" });
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Unfollow(string userId)
        {
            await _followService.UnfollowAsync(CurrentUserId, userId);
            return Ok(new { message = "Unfollowed successfully" });
        }

        [HttpGet("is-following/{userId}")]
        public async Task<IActionResult> IsFollowing(string userId)
        {
            var isFollowing = await _followService.IsFollowingAsync(CurrentUserId, userId);
            return Ok(new { isFollowing });
        }

        [HttpGet("followers/count")]
        public async Task<IActionResult> FollowersCount()
            => Ok(await _followService.GetFollowersCountAsync(CurrentUserId));

        [HttpGet("following/count")]
        public async Task<IActionResult> FollowingCount()
            => Ok(await _followService.GetFollowingCountAsync(CurrentUserId));
    }
}

