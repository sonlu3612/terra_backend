using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Core.Entities;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ====================== USER FOLLOW (bạn dùng class UserFollow) ======================
            builder.Entity<UserFollow>(entity =>
            {
                entity.HasKey(uf => new { uf.FollowerId, uf.FollowingId });

                entity.HasOne(uf => uf.Follower)
                      .WithMany(u => u.Following)
                      .HasForeignKey(uf => uf.FollowerId)
                      .OnDelete(DeleteBehavior.Cascade);    

                entity.HasOne(uf => uf.Following)
                      .WithMany(u => u.Followers)
                      .HasForeignKey(uf => uf.FollowingId)
                      .OnDelete(DeleteBehavior.NoAction); 

                entity.HasIndex(uf => uf.FollowingId);
            });

            // ====================== TWEET ======================
            builder.Entity<Tweet>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Id).HasMaxLength(50).ValueGeneratedOnAdd();
                entity.Property(t => t.Text).IsRequired().HasMaxLength(280);
                entity.Property(t => t.Images).HasColumnType("nvarchar(max)");

                entity.HasOne(t => t.User)
                      .WithMany(u => u.Tweets)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.NoAction); 

                // Reply / Thread
                entity.HasOne(t => t.ParentTweet)
                      .WithMany(t => t.Replies)
                      .HasForeignKey(t => t.ParentTweetId)
                      .OnDelete(DeleteBehavior.NoAction);
            });
            // ====================== TWEET LIKE ======================
            builder.Entity<TweetLike>(entity =>
            {
                entity.HasKey(l => new { l.UserId, l.TweetId });

                entity.HasOne(l => l.User)
                      .WithMany(u => u.Likes)
                      .HasForeignKey(l => l.UserId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(l => l.Tweet)
                      .WithMany(t => t.Likes)
                      .HasForeignKey(l => l.TweetId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ====================== TWEET RETWEET ======================
            builder.Entity<TweetRetweet>(entity =>
            {
                entity.HasKey(r => new { r.UserId, r.TweetId });

                entity.HasOne(r => r.User)
                      .WithMany(u => u.Retweets)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(r => r.Tweet)
                      .WithMany(t => t.Retweets)
                      .HasForeignKey(r => r.TweetId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ====================== TWEET BOOKMARK ======================
            builder.Entity<TweetBookmark>(entity =>
            {
                entity.HasKey(b => new { b.UserId, b.TweetId });

                entity.HasOne(b => b.User)
                      .WithMany(u => u.Bookmarks)
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(b => b.Tweet)
                      .WithMany(t => t.Bookmarks)
                      .HasForeignKey(b => b.TweetId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            // ====================== TÙY CHỈNH TÊN BẢNG  ======================
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<Tweet>().ToTable("Tweets");
            builder.Entity<UserFollow>().ToTable("UserFollows");
            builder.Entity<TweetLike>().ToTable("TweetLikes");
            builder.Entity<TweetRetweet>().ToTable("TweetRetweets");
            builder.Entity<TweetBookmark>().ToTable("TweetBookmarks");
        }
    }
}