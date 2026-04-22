using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgBotTask.Models.DtoModels;
using TgBotTask.Services;

namespace TgBotTask.Handelers
{
    public class TaskCreateHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public TaskCreateHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task HandleSessionAsync(ITelegramBotClient client, Message message, UserTaskSession session, ConcurrentDictionary<long, UserTaskSession> sessions)
        {

            long chatId = message.Chat.Id;

            switch (session.CurrentStep)
            {
                case "Name":
                    session.Task.TaskName = message.Text;
                    session.CurrentStep = "Description";
                    await client.SendMessage(chatId, "Введите описание задачи:");
                    break;

                case "Description":
                    session.Task.TaskDescription = message.Text;
                    session.CurrentStep = "Rating";
                    await client.SendMessage(chatId, "Введите рейтинг задачи (число от 1 до 10):");
                    break;

                case "Rating":
                    if (int.TryParse(message.Text, out int rating))
                    {
                        session.Task.Rating = rating;
                        session.CurrentStep = "Time";
                        await client.SendMessage(chatId, "Сколько времени нужно (в минутах)?");
                    }
                    else
                    {
                        await client.SendMessage(chatId, "Пожалуйста, введите число для рейтинга.");
                    }
                    break;

                case "Time":
                    if (int.TryParse(message.Text, out int time))
                    {
                        session.Task.TimeForDo = time;
                        await SaveTaskAndFinish(client, chatId, session, sessions);
                    }
                    else
                    {
                        await client.SendMessage(chatId, "Введите время числом.");
                    }
                    break;
            }

        }

        private async Task SaveTaskAndFinish(ITelegramBotClient client, long chatId, UserTaskSession session, ConcurrentDictionary<long, UserTaskSession> sessions)
        {
            try
            {
                using(var scoped = _serviceProvider.CreateScope())
                {
                    var crud = scoped.ServiceProvider.GetRequiredService<TaskCrud>();
                    await crud.AddTask(session.Task);
                }


                await client.SendMessage(
                    chatId: chatId,
                    text: $"Задача успешно сохранена!\n\n" +
                          $"Название: {session.Task.TaskName}\n" +
                          $"Описание: {session.Task.TaskDescription}\n" +
                          $"Рейтинг: {session.Task.Rating}\n" +
                          $"Время: {session.Task.TimeForDo} мин.");

            }
            catch
            {
                await client.SendMessage(chatId, "Ошибка при сохранении в базу данных.");
            }
            finally
            {
                sessions.TryRemove(chatId, out _);
            }
        }
    }
}
