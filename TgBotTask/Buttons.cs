using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace TgBotTask
{
    public class Buttons
    {

        public ReplyKeyboardMarkup MainMenu()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton("Добавить задачу"),
                new KeyboardButton("Посмотреть все задачи"),
                new KeyboardButton("Перейти к выполнению задач"),
                new KeyboardButton("Удалить задачу")
            })
            {
                ResizeKeyboard = true
            };
        }


        public ReplyKeyboardMarkup StartMenu()
        {
             return new ReplyKeyboardMarkup(new[]
             {
                 new KeyboardButton("Главное меню"),
                 new KeyboardButton("Инструкция")
             })
             {
                 ResizeKeyboard = true
             };
        }
        public ReplyKeyboardMarkup MenuAfterViewingAllTasks()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton("Добавить задачу"),
                new KeyboardButton("Перейти к выполнению задач"),
                new KeyboardButton("Удалить задачу")
            })
            {
                ResizeKeyboard = true
            };
        }

        public ReplyKeyboardMarkup StatusTask()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton("Задача выполнена "),
                new KeyboardButton("Задача не выполнена"),
                new KeyboardButton("Не выполнил(выйти в глаыное меню ")
            }
            );
           
        }
    }
}
