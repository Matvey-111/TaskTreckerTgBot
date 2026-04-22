using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TgBotTask.Services
{
    public class MessegeService : BackgroundService
    {
        private readonly Logger<MessegeService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITelegramBotClient _botClient;
        public MessegeService(Logger<MessegeService> logger, IServiceProvider serviceProvider, ITelegramBotClient botClient)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _botClient = botClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(message: "Сервис Уведомлений запущен");

            try
            {
                while(!stoppingToken.IsCancellationRequested)
                {
                    var now = DateTime.Now;

                    if (now.Hour == 9 || now.Minute == 0)
                    {
                        
                    }

                }
            }
            catch
            {

            }
        }

        private async Task SendDailyMessege(CancellationToken token)
        {
            using var scope = _serviceProvider.CreateScope();
            var crud = scope.ServiceProvider.GetRequiredService<TaskCrud>();
            //var userIds = await crud.GetAllUserIds();

        }
    }
}
