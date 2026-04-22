using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;

namespace TgBotTask.Services
{
    public class ShowTask
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Buttons buttons;

        public ShowTask(IServiceProvider serviceProvider, Buttons buttons)
        {
            _serviceProvider = serviceProvider;
            this.buttons = buttons;
        }
        public async Task ShowAllTasks(ITelegramBotClient client, long chatId )
        {
            using (var scope = _serviceProvider.CreateScope())
            {

                var crud = scope.ServiceProvider.GetRequiredService<TaskCrud>();

                var tasks = await crud.GetAllTask();

                string text = tasks.Any() ? string.Join("\n", tasks.Select(t => $"• {t.TaskName}")) : "Список пуст";


                await client.SendMessage(chatId, text, replyMarkup: buttons.MenuAfterViewingAllTasks());
            }
        }

        public async Task ShowAllTasksWithId(ITelegramBotClient client, long chatId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {

                var crud = scope.ServiceProvider.GetRequiredService<TaskCrud>();

                var tasks = await crud.GetAllTask();

                string text = tasks.Any() ? string.Join("\n", tasks.Select(t => $"Номер:{t.Id} Название: {t.TaskName}")) : "На даный момент список пуст";


                await client.SendMessage(chatId, text, replyMarkup: buttons.MenuAfterViewingAllTasks());
            }
        }

    }
}
