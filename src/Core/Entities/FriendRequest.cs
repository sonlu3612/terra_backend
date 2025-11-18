using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public enum FriendRequestStatus
    {
        Pending,    // Đang chờ
        Accepted,   // Đã chấp nhận
        Rejected,   // Từ chối
        Cancelled   // Người gửi hủy
    }
    public class FriendRequest
    {
        [Key]
        public string Id { get; set; }

        public string RequesterId { get; set; } = null!;
        public string AddresseeId { get; set; } = null!;

        public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedAt { get; set; }

        public virtual ApplicationUser Requester { get; set; } = null!;
        public virtual ApplicationUser Addressee { get; set; } = null!;
    }
}
