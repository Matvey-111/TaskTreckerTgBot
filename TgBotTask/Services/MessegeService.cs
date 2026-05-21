using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgBotTask.Data;
using TgBotTask.Models;

namespace TgBotTask.Services
{
    public class MessegeService : BackgroundService
    {
        private readonly Logger<MessegeService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITelegramBotClient _botClient;
        private readonly ShowTask showTask;
        public MessegeService(Logger<MessegeService> logger, IServiceProvider serviceProvider, ITelegramBotClient botClient, ShowTask showTask)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _botClient = botClient;
            this.showTask = showTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(message: "Сервис Уведомлений запущен");

            while (!stoppingToken.IsCancellationRequested)
            {
                
                var now = DateTime.Now;
                var nextRun = now.Date.AddHours(9); 

                if (now > nextRun)
                {
                    
                    nextRun = nextRun.AddDays(1);
                }

                var delay = nextRun - now;
                _logger.LogInformation($"Следующее уведомление через {delay.TotalHours:F2} ч. в {nextRun:HH:mm}");

                try
                {
                    
                    await Task.Delay(delay, stoppingToken);
 
                    await BroadcastToAllUsers(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private async Task BroadcastToAllUsers(CancellationToken ct)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userIds = db.Users.Select(u => u.ChatId).ToList();

            foreach (var chatId in userIds)
            {
                try
                {
                    await _botClient.SendMessage(chatId, "Доброе утро! Сейчас 9:00, самое лучшее время для работы", cancellationToken: ct);
                    await Task.Delay(50, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Ошибка отправки {chatId}: {ex.Message}");
                }
            }
        }
    }
}