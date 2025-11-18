using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class UserBlock
    {
        public string BlockerId { get; set; } = null!;   // Người chặn
        public string BlockedId { get; set; } = null!;   // Người bị chặn

        public DateTime BlockedAt { get; set; } = DateTime.UtcNow;

        public virtual ApplicationUser Blocker { get; set; } = null!;
        public virtual ApplicationUser Blocked { get; set; } = null!;
    }
}
