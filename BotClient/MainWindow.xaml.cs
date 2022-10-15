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
    }
}
