// Infrastructure/Services/FriendService.cs
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class FriendService : IFriendService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public FriendService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task SendFriendRequestAsync(string requesterId, string addresseeId)
    {
        if (requesterId == addresseeId)
            throw new InvalidOperationException("Không thể gửi lời mời cho chính mình");

        if (await IsFriendAsync(requesterId, addresseeId))
            throw new InvalidOperationException("Đã là bạn bè");

        var existing = await _context.FriendRequests
            .AnyAsync(fr =>
                (fr.RequesterId == requesterId && fr.AddresseeId == addresseeId) ||
                (fr.RequesterId == addresseeId && fr.AddresseeId == requesterId));

        if (existing)
            throw new InvalidOperationException("Lời mời đã tồn tại");

        var request = new FriendRequest
        {
            RequesterId = requesterId,
            AddresseeId = addresseeId
        };

        _context.FriendRequests.Add(request);
        await _context.SaveChangesAsync();
    }

    public async Task AcceptFriendRequestAsync(string requestId, string userId)
    {
        var request = await _context.FriendRequests
            .FirstOrDefaultAsync(fr => fr.Id == requestId && fr.AddresseeId == userId && fr.Status == FriendRequestStatus.Pending);

        if (request == null) throw new InvalidOperationException("Không tìm thấy lời mời");

        request.Status = FriendRequestStatus.Accepted;
        request.RespondedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task RejectFriendRequestAsync(string requestId, string userId)
    {
        var request = await _context.FriendRequests
            .FirstOrDefaultAsync(fr => fr.Id == requestId && fr.AddresseeId == userId && fr.Status == FriendRequestStatus.Pending);

        if (request == null) return;

        request.Status = FriendRequestStatus.Rejected;
        request.RespondedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task CancelFriendRequestAsync(string requestId, string userId)
    {
        var request = await _context.FriendRequests
            .FirstOrDefaultAsync(fr => fr.Id == requestId && fr.RequesterId == userId && fr.Status == FriendRequestStatus.Pending);

        if (request == null) return;

        request.Status = FriendRequestStatus.Cancelled;
        request.RespondedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task UnfriendAsync(string currentUserId, string friendUserId)
    {
        if (currentUserId == friendUserId)
            throw new InvalidOperationException("Không thể hủy kết bạn với chính mình");

        var friendship = await _context.FriendRequests
            .FirstOrDefaultAsync(fr =>
                fr.Status == FriendRequestStatus.Accepted &&
                (
                    (fr.RequesterId == currentUserId && fr.AddresseeId == friendUserId) ||
                    (fr.RequesterId == friendUserId && fr.AddresseeId == currentUserId)
                ));

        if (friendship == null)
            throw new KeyNotFoundException("Không tìm thấy mối quan hệ bạn bè");

        _context.FriendRequests.Remove(friendship);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsFriendAsync(string userId1, string userId2)
    {
        return await _context.FriendRequests
            .AnyAsync(fr =>
                (fr.RequesterId == userId1 && fr.AddresseeId == userId2 ||
                 fr.RequesterId == userId2 && fr.AddresseeId == userId1) &&
                fr.Status == FriendRequestStatus.Accepted);
    }

    public async Task<List<ApplicationUser>> GetFriendsAsync(string userId, int page = 1, int pageSize = 20)
    {
        var friends = await _context.FriendRequests
            .Where(fr => fr.Status == FriendRequestStatus.Accepted &&
                (fr.RequesterId == userId || fr.AddresseeId == userId))
            .OrderByDescending(fr => fr.RespondedAt ?? fr.RequestedAt)
            .Select(fr => fr.RequesterId == userId ? fr.Addressee : fr.Requester)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return friends;
    }

    public async Task<List<FriendRequest>> GetPendingRequestsAsync(string userId)
    {
        return await _context.FriendRequests
            .Where(fr => fr.AddresseeId == userId && fr.Status == FriendRequestStatus.Pending)
            .Include(fr => fr.Requester)
            .OrderByDescending(fr => fr.RequestedAt)
            .ToListAsync();
    }

    public async Task<List<object>> GetFriendSuggestionsAsync(string userId, int limit = 20)
    {
        // 1. Lấy danh sách ID bạn bè của user hiện tại
        var friendIds = await _context.FriendRequests
            .Where(fr => fr.Status == FriendRequestStatus.Accepted &&
                         (fr.RequesterId == userId || fr.AddresseeId == userId))
            .Select(fr => fr.RequesterId == userId ? fr.AddresseeId : fr.RequesterId)
            .ToListAsync();

        if (!friendIds.Any())
            return new List<object>();

        // 2. Lấy tất cả người mà bạn bè của mình đang kết bạn (Friends-of-Friends)
        var friendOfFriendIds = await _context.FriendRequests
            .Where(fr => fr.Status == FriendRequestStatus.Accepted &&
                         (friendIds.Contains(fr.RequesterId) || friendIds.Contains(fr.AddresseeId)))
            .Select(fr => fr.RequesterId == userId ? fr.AddresseeId :
                         fr.AddresseeId == userId ? fr.RequesterId :
                         fr.RequesterId != userId ? fr.RequesterId : fr.AddresseeId)
            .Where(id => id != userId)
            .Distinct()
            .ToListAsync();

        // 3. Loại bỏ những người đã là bạn hoặc đã có lời mời qua lại
        var excludedIds = await _context.FriendRequests
            .Where(fr => (fr.RequesterId == userId || fr.AddresseeId == userId))
            .Select(fr => fr.RequesterId == userId ? fr.AddresseeId : fr.RequesterId)
            .ToListAsync();

        excludedIds.AddRange(friendIds); // thêm luôn danh sách bạn bè vào để loại

        var candidateIds = friendOfFriendIds
            .Where(id => !excludedIds.Contains(id))
            .Distinct()
            .Take(100) // giới hạn để không quét cả triệu người
            .ToList();

        var suggestions = new List<object>();

        foreach (var candidateId in candidateIds)
        {
            // Đếm số bạn chung
            int mutualCount = friendIds.Count(friendId =>
                _context.FriendRequests.Any(fr =>
                    fr.Status == FriendRequestStatus.Accepted &&
                    ((fr.RequesterId == friendId && fr.AddresseeId == candidateId) ||
                     (fr.RequesterId == candidateId && fr.AddresseeId == friendId))
            ));

            if (mutualCount == 0) continue;

            var user = await _userManager.FindByIdAsync(candidateId);
            if (user == null) continue;

            // Kiểm tra mình đã gửi lời mời cho người này chưa
            bool hasSentRequest = await _context.FriendRequests
                .AnyAsync(fr => fr.RequesterId == userId &&
                               fr.AddresseeId == candidateId &&
                               fr.Status == FriendRequestStatus.Pending);

            suggestions.Add(new
            {
                userId = user.Id,
                userName = user.UserName,
                displayName = user.DisplayName ?? user.UserName,
                avatar = user.ImageUrl ?? "",
                mutualFriends = mutualCount,
                hasSentRequest
            });
        }

        // Sắp xếp theo số bạn chung giảm dần và lấy limit bản ghi
        return suggestions
            .OrderByDescending(x => (int)x.GetType().GetProperty("mutualFriends")!.GetValue(x)!)
            .Take(limit)
            .ToList();
    }
}