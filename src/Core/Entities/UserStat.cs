using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class UserStat
    {
        public string UserId { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string LikesJson { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string RetweetsJson { get; set; }

        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        public virtual ApplicationUser User { get; set; }
    }
}
