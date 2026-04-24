using System;
using System.Collections.Generic;
using System.Text;
using TgBotTask.Data;
using TgBotTask.Models;
using Microsoft.EntityFrameworkCore;

namespace TgBotTask.Services
{
    public class TaskCrud
    {
        private readonly AppDbContext _dbContext;

        public TaskCrud(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<TaskModel>> GetAllTask(long chatId)
        {
            return await _dbContext.Tasks.Where(u => u.UserChatId == chatId).ToListAsync();
        }

        public async Task<TaskModel> GetTaskById(int id, long chatId)
        {
            var task = await _dbContext.Tasks.Where(u => u.UserChatId == chatId).FirstOrDefaultAsync(t => t.Id == id);
            return task;
        }


        public async Task AddTask(TaskModel task, long chatId)
        {
            var user = await _dbContext.Users.Include(u => u.Tasks).FirstOrDefaultAsync(u => u.ChatId == chatId);

            if(user != null)
            {
                user.Tasks.Add(task);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("Пользователь не найден");
            }
            
        }

        public async Task UpdateTask(TaskModel task, int id, long chatId)
        {
            var taskModel = await _dbContext.Tasks.Where(u => u.UserChatId == chatId).FirstOrDefaultAsync(t => t.Id == id);

            taskModel = task;

            await _dbContext.SaveChangesAsync();
           

        }

        public async void DeleteTask(int id, long chatId)
        {
            var taskToDelete = await _dbContext.Tasks.Where(u => u.UserChatId == chatId).FirstOrDefaultAsync(t => t.Id == id);

            if (taskToDelete != null)
            {
                _dbContext.Remove(taskToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}
