using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgBotTask.Models.DtoModels;
using TgBotTask.Services;

namespace TgBotTask.Handelers
{
    internal class TaskDeleteHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Buttons buttons;

        public TaskDeleteHandler(IServiceProvider serviceProvider, Buttons buttons)
        {
            _serviceProvider = serviceProvider;
            this.buttons = buttons;
        }

        public async Task HandleSessionAsync(ITelegramBotClient client, Message message, UserTaskSession session, ConcurrentDictionary<long, UserTaskSession> sessions)
        {
            long chatId = message.Chat.Id;

            int taskId = 0;

            switch (session.CurrentStep)
            {
                case "AllTasks":


                    using (var scop = _serviceProvider.CreateScope())
                    {
                        var getTasks = scop.ServiceProvider.GetRequiredService<ShowTask>();
                        var allTasksWithId = getTasks.ShowAllTasksWithId(client, chatId).ToString();

                        await client.SendMessage(chatId: chatId, text: allTasksWithId);
                    }
                    session.CurrentStep = "NumberOfTask";
                    await client.SendMessage(chatId, "Введите номер задачи которую хотите удалить");
                    break;

                case "NumberOfTask":

                    if (int.TryParse(message.Text, out taskId))
                    {
                        using (var scop = _serviceProvider.CreateScope())
                        {
                            var getTask = scop.ServiceProvider.GetRequiredService<TaskCrud>();
                            getTask.DeleteTask(taskId,chatId);
                        }
                        sessions.TryRemove(chatId, out _);
                        

                        await client.SendMessage(chatId: chatId, text: "Задача была удлалена", replyMarkup: buttons.MainMenu());
                    }
                    else
                    {
                        await client.SendMessage(chatId: chatId, text: "Вы не правильно написали номер задачи");
                    }

                    break;

               
            }
        }
    }
}
