using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Entities;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/block")]
    [ApiController]
    public class BlockController : ControllerBase
    {
        private readonly IBlockService _blockService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BlockController(IBlockService blockService, UserManager<ApplicationUser> userManager)
        {
            _blockService = blockService;
            _userManager = userManager;
        }

        private string CurrentUserId => _userManager.GetUserId(User)!;

        [HttpPost("{userId}")]
        public async Task<IActionResult> Block(string userId)
        {
            await _blockService.BlockUserAsync(CurrentUserId, userId);
            return Ok(new { message = "Đã chặn người dùng" });
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Unblock(string userId)
        {
            await _blockService.UnblockUserAsync(CurrentUserId, userId);
            return Ok(new { message = "Đã bỏ chặn" });
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetBlockedList([FromQuery] int page = 1)
            => Ok(await _blockService.GetBlockedUsersAsync(CurrentUserId, page));

        [HttpGet("check/{userId}")]
        public async Task<IActionResult> Check(string userId)
            => Ok(new { isBlocked = await _blockService.IsBlockedAsync(CurrentUserId, userId) });
    }
}
