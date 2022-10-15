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
        public string LastMessage { get; set; }
        public string Time { get; set; }
        public long ID { get; set; }        
        public UserLog(string name, string lastMessage, string time, long id)
        {
            Name = name;
            LastMessage = lastMessage;
            Time = time;
            ID = id;            
        }
    }
}
