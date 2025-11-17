using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AuthSession
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        [StringLength(500)]
        public string RefreshToken { get; set; }

        public DateTimeOffset ExpiresAt { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public virtual ApplicationUser User { get; set; }
    }
}
