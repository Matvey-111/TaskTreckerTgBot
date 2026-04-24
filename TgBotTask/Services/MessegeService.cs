using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
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


                if (now.Hour == 10 && now.Minute == 0)
                {
                    await SendDailyReminders(stoppingToken);
                }


                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
        private async Task SendDailyReminders(CancellationToken ct)
        {
            using var scope = _serviceProvider.CreateScope();
            var crud = scope.ServiceProvider.GetRequiredService<UserCrud>();
            var userIds = await crud.GetAllUserChatIds();

            foreach (var chatId in userIds)
            {
                try
                {
                    await _botClient.SendMessage(chatId, "🔔 Напоминание: Не забудьте проверить свои задачи на сегодня!", cancellationToken: ct);

                    // Задержка, чтобы не превысить лимиты Telegram (30 сообщений в секунду)
                    await Task.Delay(100, ct);
                }
                catch (Exception ex)
                {
                    // Если пользователь заблокировал бота, здесь будет ошибка 403
                    Console.WriteLine($"Не удалось отправить сообщение {chatId}: {ex.Message}");
                }
            }
        }
    }
}
