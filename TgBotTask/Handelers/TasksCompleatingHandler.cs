using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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
    public class TasksCompleatingHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Buttons buttons;
        private readonly ShowTask showTask;
        public TasksCompleatingHandler(IServiceProvider serviceProvider, Buttons buttons, ShowTask showTask)
        {
            _serviceProvider = serviceProvider;
            this.buttons = buttons;
            this.showTask = showTask;
        }

        public async Task HandleSessionAsync(ITelegramBotClient client, Message message, UserTaskSession session, ConcurrentDictionary<long, UserTaskSession> sessions) 
        {
            long chatId = message.Chat.Id;

            int taskId = 0;

            switch (session.CurrentStep)
            {
                case "ReturnAllTasks":


                    await showTask.ShowAllTasksWithId(client, chatId);
                    session.CurrentStep = "GetNumberOfTask";
                    await client.SendMessage(chatId, "Введите номер задачи которую хотите выполнить");
                    break;

                case "GetNumberOfTask":

                    if (int.TryParse(message.Text, out taskId))
                    {
                        using (var scop = _serviceProvider.CreateScope())
                        {
                            var getTask = scop.ServiceProvider.GetRequiredService<TaskCrud>();
                            var taskById = getTask.GetTaskById(taskId, chatId).ToString();

                            await client.SendMessage(chatId: chatId, text: taskById);
                        }

                        session.CurrentStep = "CompleatOrUncompleat";

                        await client.SendMessage(chatId: chatId, text: "Выберите стаутус задачи", replyMarkup: buttons.StatusTask());
                    }
                    else
                    {
                        await client.SendMessage(chatId: chatId, text: "Вы не правильно написали номер задачи");
                    }

                    break;

                case "CompleatOrUncompleat":
                    
                    if (message.Text.ToLower() == "задача выполнена")
                    {
                        using (var scop = _serviceProvider.CreateScope())
                        {
                            var getTask = scop.ServiceProvider.GetRequiredService<TaskCrud>();

                            getTask.DeleteTask(taskId, chatId);


                            await client.SendMessage(chatId: chatId, text: "Задача была убрана", replyMarkup: buttons.MainMenu());

                            sessions.TryRemove(chatId, out _);
                        }
                    }
                    else if(message.Text.ToLower() == "задача не выполнена")
                    {
                        session.CurrentStep = "ReturnAllTasks";
                    }
                    else
                    {
                        
                        sessions.TryRemove(chatId, out _);
                        await client.SendMessage(chatId: chatId, text: "Главное меню", replyMarkup: buttons.MainMenu());
                    }
                    break;

            }
        }
    }
}
