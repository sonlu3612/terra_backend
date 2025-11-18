using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class MediaAsset
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(50)]
        public string MimeType { get; set; }

        public long SizeBytes { get; set; }

        [StringLength(500)]
        public string Url { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public virtual ApplicationUser User { get; set; }
    }
}
