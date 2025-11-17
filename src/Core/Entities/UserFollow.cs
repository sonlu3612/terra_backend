using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class UserFollow
    {
        public string FollowerId { get; set; }

        public string FollowingId { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public virtual ApplicationUser Follower { get; set; }

        public virtual ApplicationUser Following { get; set; }
    }
}
