// Core/Entities/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Core.Entities
{
    public class ApplicationUser : IdentityUser   
    {
        public string FullName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; } = string.Empty;
        public string? Theme { get; set; } = "dark";
        public string? Accent { get; set; } = "blue";
        public string? Website { get; set; } = string.Empty;
        public string? Location { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } = string.Empty;        // avatar
        public string? CoverPhotoURL { get; set; } = string.Empty;   // ảnh bìa
        public bool Verified { get; set; } = false;
        public int FollowingCount { get; set; } = 0;
        public int FollowersCount { get; set; } = 0;
        public int TotalTweets { get; set; } = 0;
        public int TotalPhotos { get; set; } = 0;
        public string? PinnedTweetId { get; set; }   

        public virtual Tweet? PinnedTweet { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<Tweet> Tweets { get; set; } = new List<Tweet>();

        public virtual ICollection<UserFollow> Following { get; set; } = new List<UserFollow>();
        public virtual ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();

        // Like / Retweet / Bookmark
        public virtual ICollection<TweetLike> Likes { get; set; } = new List<TweetLike>();
        public virtual ICollection<TweetRetweet> Retweets { get; set; } = new List<TweetRetweet>();
        public virtual ICollection<TweetBookmark> Bookmarks { get; set; } = new List<TweetBookmark>();

        //Friend Request
        public virtual ICollection<FriendRequest> SentFriendRequests { get; set; } = new List<FriendRequest>();
        public virtual ICollection<FriendRequest> ReceivedFriendRequests { get; set; } = new List<FriendRequest>();

        //Block
        public virtual ICollection<UserBlock> Blocking { get; set; } = new List<UserBlock>();   // Danh sách người mình chặn
        public virtual ICollection<UserBlock> BlockedBy { get; set; } = new List<UserBlock>();   // Danh sách người chặn mình
    }
}