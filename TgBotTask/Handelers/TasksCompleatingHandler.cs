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

            switch (session.CurrentStep)
            {
                case "ReturnAllTasks":
                    await showTask.ShowAllTasksWithId(client, chatId);
                    session.CurrentStep = "GetNumberOfTask";
                    await client.SendMessage(chatId, "Введите номер задачи которую хотите выполнить");
                    break;

                case "GetNumberOfTask":
                    if (int.TryParse(message.Text, out int inputId))
                    {
                        
                        session.SelectedTaskId = inputId;

                        using (var scop = _serviceProvider.CreateScope())
                        {
                            var crud = scop.ServiceProvider.GetRequiredService<TaskCrud>();
                            var task = await crud.GetTaskById(inputId, chatId);

                            if (task != null)
                            {
                                await client.SendMessage(chatId, task.ToString());
                                session.CurrentStep = "CompleatOrUncompleat";
                                await client.SendMessage(chatId, "Выберите статус задачи", replyMarkup: buttons.StatusTask());
                            }
                            else
                            {
                                await client.SendMessage(chatId, "Задача не найдена. Введите другой номер:");
                            }
                        }
                    }
                    else
                    {
                        await client.SendMessage(chatId, "Вы неправильно написали номер задачи");
                    }
                    break;

                case "CompleatOrUncompleat":
                    // ДОСТАЕМ ID ИЗ СЕССИИ
                    int currentId = session.SelectedTaskId;

                    if (message.Text.ToLower() == "задача выполнена")
                    {
                        using (var scop = _serviceProvider.CreateScope())
                        {
                            var crud = scop.ServiceProvider.GetRequiredService<TaskCrud>();
                            crud.DeleteTask(currentId, chatId); // Обязательно await

                            await client.SendMessage(chatId, "✅ Задача выполнена и удалена!", replyMarkup: buttons.MainMenu());
                            sessions.TryRemove(chatId, out _);
                        }
                    }
                    else if (message.Text.ToLower() == "задача не выполнена")
                    {
                        // Чтобы вернуться к списку, нужно либо отправить сообщение, либо вручную вызвать шаг
                        await client.SendMessage(chatId, "Хорошо, выберите другую задачу или введите ID заново:");
                        session.CurrentStep = "GetNumberOfTask";
                    }
                    else
                    {
                        sessions.TryRemove(chatId, out _);
                        await client.SendMessage(chatId, "Выход в главное меню", replyMarkup: buttons.MainMenu());
                    }
                    break;
            }
        }
    }
}
