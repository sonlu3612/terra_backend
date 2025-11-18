using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FollowService : IFollowService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FollowService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task FollowAsync(string followerId, string followingId)
        {
            if (followerId == followingId)
            {
                throw new ArgumentException("Cannot follow yourself");
            }
            var existing = await _context.UserFollows
                .AnyAsync(f => f.FollowerId == followingId && f.FollowingId == followingId);

            if (!existing) 
            {
                var follow = new UserFollow
                {
                    FollowerId = followerId,
                    FollowingId = followingId
                };
                _context.UserFollows.Add(follow);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UnfollowAsync(string followerId, string followingId)
        {
            var follow = await _context.UserFollows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

            if(follow == null)
            {
                _context.UserFollows.Remove(follow);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsFollowingAsync(string followerId, string followingId)
        {
            return await _context.UserFollows
                .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
        }


        public async Task<int> GetFollowersCountAsync(string userId)
        
            => await _context.UserFollows.CountAsync(f => f.FollowingId == userId);

        public async Task<int> GetFollowingCountAsync(string userId)

            => await _context.UserFollows.CountAsync(f => f.FollowerId == userId);

        public async Task<List<ApplicationUser>> GetFollowersAsync(string userId, int page = 1, int pageSize = 20)
        {
            return await _context.UserFollows
                .Where(f => f.FollowingId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => f.Follower)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        }

        public async Task<List<ApplicationUser>> GetFollowingAsync(string userId, int page = 1, int pageSize = 20)
        {
            return await _context.UserFollows
                .Where(f => f.FollowerId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => f.Following)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        }



    }
}
