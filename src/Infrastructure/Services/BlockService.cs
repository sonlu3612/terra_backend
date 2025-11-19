using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    // Infrastructure/Services/BlockService.cs
    using Core.Entities;
    using Core.Interfaces;
    using Infrastructure.Persistence;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Core.Entities;

    public class BlockService : IBlockService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BlockService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task BlockUserAsync(string blockerId, string blockedId)
        {
            if (blockerId == blockedId)
                throw new InvalidOperationException("Không thể tự chặn chính mình");

            var exists = await _context.UserBlocks
                .AnyAsync(b => b.BlockerId == blockerId && b.BlockedId == blockedId);

            if (!exists)
            {
                _context.UserBlocks.Add(new UserBlock
                {
                    BlockerId = blockerId,
                    BlockedId = blockedId
                });
                await _context.SaveChangesAsync();
            }
        }

        public async Task UnblockUserAsync(string blockerId, string blockedId)
        {
            var block = await _context.UserBlocks
                .FirstOrDefaultAsync(b => b.BlockerId == blockerId && b.BlockedId == blockedId);

            if (block != null)
            {
                _context.UserBlocks.Remove(block);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsBlockedAsync(string userId1, string userId2)
        {
            return await _context.UserBlocks
                .AnyAsync(b =>
                    (b.BlockerId == userId1 && b.BlockedId == userId2) ||
                    (b.BlockerId == userId2 && b.BlockedId == userId1));
        }

        public async Task<List<ApplicationUser>> GetBlockedUsersAsync(string userId, int page = 1, int pageSize = 50)
        {
            return await _context.UserBlocks
                .Where(b => b.BlockerId == userId)
                .OrderByDescending(b => b.BlockedAt)
                .Select(b => b.Blocked)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
