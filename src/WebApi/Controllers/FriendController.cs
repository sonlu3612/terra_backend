using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Entities;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/friends")]
    public class FriendController : ControllerBase
    {
        private readonly IFriendService _friendService;
        private readonly UserManager<ApplicationUser> _userManager;

        public FriendController(IFriendService friendService, UserManager<ApplicationUser> userManager)
        {
            _friendService = friendService;
            _userManager = userManager;
        }

        private string CurrentUserId => _userManager.GetUserId(User)!;

        [HttpPost("request/{userId}")]
        public async Task<IActionResult> SendRequest(string userId)
        {
            await _friendService.SendFriendRequestAsync(CurrentUserId, userId);
            return Ok(new { message = "Đã gửi lời mời kết bạn" });
        }

        [HttpPost("accept/{requestId}")]
        public async Task<IActionResult> Accept(string requestId)
        {
            await _friendService.AcceptFriendRequestAsync(requestId, CurrentUserId);
            return Ok(new { message = "Đã chấp nhận kết bạn" });
        }

        [HttpPost("reject/{requestId}")]
        public async Task<IActionResult> Reject(string requestId)
        {
            await _friendService.RejectFriendRequestAsync(requestId, CurrentUserId);
            return Ok();
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
            => Ok(await _friendService.GetPendingRequestsAsync(CurrentUserId));

        [HttpGet("list")]
        public async Task<IActionResult> GetFriends([FromQuery] int page = 1)
            => Ok(await _friendService.GetFriendsAsync(CurrentUserId, page));

        [HttpDelete("unfriend/{friendUserId}")]
        public async Task<IActionResult> Unfriend(string friendUserId)
        {
            await _friendService.UnfriendAsync(CurrentUserId, friendUserId);
            return Ok(new { message = "Đã hủy kết bạn thành công" });
        }

        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions([FromQuery] int limit = 20)
        {
            var data = await _friendService.GetFriendSuggestionsAsync(CurrentUserId, limit);
            return Ok(data);
        }
    }
}
