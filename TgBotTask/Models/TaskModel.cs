using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Text;

namespace TgBotTask.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string TaskName { get; set; } ="";
        public string TaskDescription { get; set; } ="";
        public int Rating { get; set; }
        public int TimeForDo { get; set; }
        public long UserChatId { get; set; }
    }
}
