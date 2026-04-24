using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using TgBotTask.Data;
using TgBotTask.Models;

namespace TgBotTask.Services
{
    public class UserCrud
    {
        private readonly AppDbContext _dbContext;

        public UserCrud(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> UserExists(long chatId)
        {
            return await _dbContext.Users.AnyAsync(u => u.ChatId == chatId);
        }
        public async Task<List<long>> GetAllUserChatIds()
        {
            return await _dbContext.Users.Select(u => u.ChatId).ToListAsync();
        }
        public async Task AddUser(long chatId, string name)
        {
            var user = new UserModel 
            { 
                ChatId = chatId, 
                Username =  name 
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }
        public async Task RemoveUser(long chatId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);
            if (user != null)
            {
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
