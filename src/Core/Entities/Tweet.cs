// Core/Entities/Tweet.cs
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Core.Entities
{
    public class Tweet
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string UserId { get; set; } = string.Empty;

        [StringLength(280, ErrorMessage = "Tweet cannot exceed 280 characters")]
        public string Text { get; set; } = string.Empty;

        public string? Images { get; set; } 

        public string? ParentTweetId { get; set; }

        public int LikeCount { get; set; } = 0;
        public int RetweetCount { get; set; } = 0;
        public int ReplyCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public virtual Tweet? ParentTweet { get; set; }
        public virtual ICollection<Tweet> Replies { get; set; } = new List<Tweet>();
        public virtual ICollection<TweetMedia> Media { get; set; } = new List<TweetMedia>();
        public virtual ICollection<TweetLike> Likes { get; set; } = new List<TweetLike>();
        public virtual ICollection<TweetRetweet> Retweets { get; set; } = new List<TweetRetweet>();
        public virtual ICollection<TweetBookmark> Bookmarks { get; set; } = new List<TweetBookmark>();
    }
}