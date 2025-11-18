using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFollowService
    {
        Task FollowAsync(string followerId, string followingId);
        Task UnfollowAsync(string followerId, string followingId);
        Task<bool> IsFollowingAsync(string followerId, string followingId);

        Task<int> GetFollowersCountAsync(string userId);
        Task<int> GetFollowingCountAsync(string userId);
        Task<List<ApplicationUser>> GetFollowersAsync(string userId, int page = 1, int pageSize = 20);
        Task<List<ApplicationUser>> GetFollowingAsync(string userId, int page = 1, int pageSize = 20);


    }
}
