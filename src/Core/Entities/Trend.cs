using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Trend
    {
        public string Id { get; set; }

        public int Woeid { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Url { get; set; }

        [StringLength(100)]
        public string Query { get; set; }

        public int? TweetVolume { get; set; }

        public DateTimeOffset CapturedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
