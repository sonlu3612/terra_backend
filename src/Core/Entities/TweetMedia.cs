using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class TweetMedia
    {
        public string TweetId { get; set; }

        public string MediaId { get; set; }

        public int Position { get; set; }

        public virtual Tweet Tweet { get; set; }

        public virtual MediaAsset Media { get; set; }
    }
}
