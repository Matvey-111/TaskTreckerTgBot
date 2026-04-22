using Microsoft.Extensions.DependencyInjection;

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TgBotTask.Models.DtoModels;
using TgBotTask.Services;
using System.Collections.Concurrent;

namespace TgBotTask.Handelers
{
    public class UpdateHandler
    {
        static ConcurrentDictionary<long, UserTaskSession> userSessions = new();

        private readonly IServiceProvider _serviceProvider;

        private readonly Buttons buttons;

        private readonly ShowTask showTask;

        public UpdateHandler(IServiceProvider serviceProvider, Buttons buttons,  ShowTask showTask)
        {
            _serviceProvider = serviceProvider;
            this.buttons = buttons;
            this.showTask = showTask;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var messege = update.Message;


            if (messege != null)
            {
                long chatId = messege.Chat.Id;

                if (userSessions.TryGetValue(chatId, out var addNewTaskSession))
                {
                    var taskHandler = new TaskCreateHandler(_serviceProvider);
                    await taskHandler.HandleSessionAsync(client, messege, addNewTaskSession, userSessions);
                    return;

                }
                
                switch (messege.Text.ToString().ToLower())
                {
                    case "запуск" or "/start":
                        var start = buttons.StartMenu();
                        await client.SendMessage(
                            chatId: messege.Chat.Id,
                            text: "Привет я твой трекер задач",
                            replyMarkup: start);
                        break;


                    case "главное меню":


                        var mainMenu = buttons.MainMenu();

                        await client.SendMessage(
                            chatId: messege.Chat.Id,
                            text: "Выбери дейстивие:",
                            replyMarkup: mainMenu);
                        break;

                    case "добавить задачу":

                        userSessions[chatId] = new UserTaskSession(); // Создаем новую сессию
                        await client.SendMessage(chatId, "Введите название задачи:");

                        break;


                    case "посмотреть все задачи":

                        using (var scope = _serviceProvider.CreateScope())
                        {
                            await showTask.ShowAllTasks(client, chatId);
                        }
                        break;


                    case "перейти к выполнению задач":

                        userSessions[chatId] = new UserTaskSession()
                        {
                            CurrentStep = "ReturnAllTasks"
                        };

                        break;

                    case "Удалить задачу":

                        userSessions[chatId] = new UserTaskSession()
                        {
                            CurrentStep = "AllTasks"
                        };

                        break;

                }

            }
                
        }
        public Task HandleErrorAsync(ITelegramBotClient client, Exception exception, HandleErrorSource source,CancellationToken token)
        {
            Console.WriteLine("Ошибка: " + exception.Message);
            return Task.CompletedTask;

        }
    }
}
