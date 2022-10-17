using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BotClient
{ 
    public partial class MainWindow : Window
    {
        TelegramBot bot;      
        public MainWindow()
        {
            InitializeComponent();
            bot = new TelegramBot(this);
            UsersList.ItemsSource = bot.BotLog;            
        }

        private void SendBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SendBox.Text.Length > 0)
                SendButton.IsEnabled = true;
            else
                SendButton.IsEnabled = false;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserID.Text.Length == 9)
            { 
                bot.SendMessage(SendBox.Text, UserID.Text);
                SendBox.Text = "";
            }
        }

        private void GitHubButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com");
        }

        private void TelegramButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://t.me/TrainingBot2022_bot");
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void DocumentItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(bot.destinationPreset);
        }

        private void SaveItem_Click(object sender, RoutedEventArgs e)
        {
            CheckLogFolder();
            JObject main = new JObject();
            JArray messages = new JArray();
            foreach (UserLog message in bot.BotLog)
            {
                JObject messageInfo = new JObject();
                messageInfo["First_name"] = message.Name;
                messageInfo["ID"] = message.ID;
                messageInfo["Text"] = message.Message;
                messageInfo["Time"] = message.Time;
                messages.Add(messageInfo);
            }
            main["Messages"] = messages;
            WriteJSON(main);            
        }

        private void CheckLogFolder()
        {
            if (!Directory.Exists("Messages Logs"))
                Directory.CreateDirectory("Messages Logs");
        }

        private void WriteJSON(JObject json)
        {
            string fileName = $"MessageLog_{DateTime.Now.ToFileTime()}.json";
            string path = "Messages Logs\\" + fileName;
            File.WriteAllText(path, json.ToString());
            Process.Start(path);
        }
    }
}
