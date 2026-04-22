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

        public async Task<IEnumerable<TaskModel>> GetAllTask()
        {
            return await _dbContext.Tasks.ToListAsync();
        }

        public async Task<TaskModel> GetTaskById(int id)
        {
            var task = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            return task;
        }


        public async Task AddTask(TaskModel task)
        {

            await _dbContext.Tasks.AddAsync(task);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateTask(TaskModel task, int id)
        {
            var taskModel = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);

            taskModel = task;

            await _dbContext.SaveChangesAsync();
           

        }

        public async void DeleteTask(int id)
        {
            var taskToDelete = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);

            if (taskToDelete != null)
            {
                _dbContext.Remove(taskToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}
