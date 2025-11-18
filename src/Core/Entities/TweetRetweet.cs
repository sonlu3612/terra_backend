using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class TweetRetweet
    {
        public string TweetId { get; set; }

        public string UserId { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public virtual Tweet Tweet { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
