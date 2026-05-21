using System;
using System.Collections.Generic;
using System.Text;

namespace TgBotTask.Models.DtoModels
{
    public class UserTaskSession
    {
        public string CurrentStep { get; set; }
        public TaskModel Task { get; set; } = new();

        public int SelectedTaskId { get; set; }
    }
}
