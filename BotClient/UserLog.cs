using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotClient
{
    public class UserLog
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }
        public long ID { get; set; }        
        public UserLog(string name, string message, string time, long id)
        {
            Name = name;
            Message = message;
            Time = time;
            ID = id;            
        }
    }
}
