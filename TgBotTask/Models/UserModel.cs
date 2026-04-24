using System;
using System.Collections.Generic;
using System.Text;

namespace TgBotTask.Models
{
    public class UserModel
    {
        public int Id { get; set; } 
        public long ChatId { get; set; } 
        public string Username { get; set; }    
        public List<TaskModel> Tasks { get; set; } = new();
    }
}
