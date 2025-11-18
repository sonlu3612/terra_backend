using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFriendService
    {
        Task SendFriendRequestAsync(string requesterId, string addresseeId);
        Task AcceptFriendRequestAsync(string requestId, string userId);
        Task RejectFriendRequestAsync(string requestId, string userId);
        Task CancelFriendRequestAsync(string requestId, string userId);
        Task UnfriendAsync(string currentUserId, string friendUserId);
        Task<bool> IsFriendAsync(string userId1, string userId2);
        Task<List<ApplicationUser>> GetFriendsAsync(string userId, int page = 1, int pageSize = 20);
        Task<List<FriendRequest>> GetPendingRequestsAsync(string userId);
        Task<List<object>> GetFriendSuggestionsAsync(string userId, int limit = 20);


    }
}
