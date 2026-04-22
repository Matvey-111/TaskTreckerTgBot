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

        public TasksCompleatingHandler(IServiceProvider serviceProvider, Buttons buttons)
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
                case "ReturnAllTasks":
                  

                    using(var scop = _serviceProvider.CreateScope())
                    {
                        var getTasks = scop.ServiceProvider.GetRequiredService<ShowTask>();
                        var allTasksWithId = getTasks.ShowAllTasksWithId(client, chatId).ToString();

                        await client.SendMessage(chatId: chatId, text: allTasksWithId);
                    }
                    session.CurrentStep = "GetNumberOfTask";
                    await client.SendMessage(chatId, "Введите номер задачи которую хотите выполнить");
                    break;

                case "GetNumberOfTask":

                    if (int.TryParse(message.Text, out taskId))
                    {
                        using (var scop = _serviceProvider.CreateScope())
                        {
                            var getTask = scop.ServiceProvider.GetRequiredService<TaskCrud>();
                            var taskById = getTask.GetTaskById(taskId).ToString();

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

                            getTask.DeleteTask(taskId);


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
