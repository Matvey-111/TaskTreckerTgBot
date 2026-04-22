using System;
using System.Collections.Generic;
using System.Text;

namespace TgBotTask.Models.DtoModels
{
    public class UserTaskSession
    {
        public string CurrentStep { get; set; } = "Name"; // Name, Description, Rating, Time
        public TaskModel Task { get; set; } = new();
    }
}
