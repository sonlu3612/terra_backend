using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IBlockService
    {
        Task BlockUserAsync(string blockerId, string blockedId);
        Task UnblockUserAsync(string blockerId, string blockedId);
        Task<bool> IsBlockedAsync(string userId1, string userId2); // userId1 chặn userId2 HOẶC ngược lại
        Task<List<ApplicationUser>> GetBlockedUsersAsync(string userId, int page = 1, int pageSize = 50);
    }
}
