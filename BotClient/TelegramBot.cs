using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace BotClient
{
    public class TelegramBot
    {
        private MainWindow mainWindow;
        private TelegramBotClient bot;        
        public ObservableCollection<UserLog> BotLog { get; set; }

        public Dictionary<long, List<string>> documentList = new Dictionary<long, List<string>>();
        static bool uploadFlag = false;
        public string destinationPreset = $@"{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads")}\TrainingBot\";
        public TelegramBot(MainWindow main)
        {           
            BotLog = new ObservableCollection<UserLog>();            
            mainWindow = main;
            string token = "PLACEHOLDER";
            bot = new TelegramBotClient(token);
            bot.StartReceiving(Update, Error);                
        }

        async Task Update(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;

            if (message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                long id = message.Chat.Id;
                                          
                mainWindow.Dispatcher.Invoke(() =>
                    {
                        BotLog.Add(new UserLog(message.Chat.FirstName, message.Text, DateTime.Now.ToLongTimeString(), message.Chat.Id));                                           
                    });
            }

            if (uploadFlag)
            {
                uploadFlag = !uploadFlag;
                if (IsNumber(message.Text))
                {
                    int number = int.Parse(message.Text);
                    if (number >= 1 && number <= documentList.Count)
                    {
                        string fileName = documentList[message.Chat.Id][number - 1];
                        Stream stream = System.IO.File.OpenRead(destinationPreset + $"{message.Chat.Id}\\" + fileName);
                        await botClient.SendDocumentAsync(message.Chat.Id, new InputOnlineFile(stream, fileName));
                        stream.Close();
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Документа с таким номером не существует.");
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Для скачивания документа необходимо отправить его числовой номер.");
                }
            }
            else
            {
                switch (message.Type)
                {
                    case Telegram.Bot.Types.Enums.MessageType.Text:
                        {
                            switch (message.Text)
                            {
                                case "/start":
                                    {
                                        await botClient.SendTextMessageAsync(message.Chat.Id, "Отправьте файл для сохранения в облаке.");
                                        return;
                                    }
                                case "/documentslist":
                                    {
                                        if (documentList.Count != 0)
                                        {
                                            string list = "";
                                            await botClient.SendTextMessageAsync(message.Chat.Id, "Cписок всех сохранённых документов:\n");
                                            for (int index = 0; index < documentList[message.Chat.Id].Count; index++)
                                            {
                                                list += $"{documentList[message.Chat.Id][index]}\n";
                                            }
                                            await botClient.SendTextMessageAsync(message.Chat.Id, $"{list}\n");
                                        }
                                        else
                                            await botClient.SendTextMessageAsync(message.Chat.Id, "Список документов пуст.");
                                        return;
                                    }
                                case "/downloaddocument":
                                    {
                                        if (documentList.Count != 0)
                                        {
                                            uploadFlag = true;
                                            string list = "";
                                            await botClient.SendTextMessageAsync(message.Chat.Id, "Введите номер документа для скачивания:\n");
                                            for (int index = 0; index < documentList[message.Chat.Id].Count; index++)
                                            {
                                                list += $"{index + 1}.  {documentList[message.Chat.Id][index]}\n";
                                            }
                                            await botClient.SendTextMessageAsync(message.Chat.Id, $"{list}\n");
                                        }
                                        else
                                            await botClient.SendTextMessageAsync(message.Chat.Id, "Список документов пуст.");
                                        return;
                                    }
                                default:
                                    {
                                        await botClient.SendTextMessageAsync(message.Chat.Id, "Неизвестная команда.");
                                        return;
                                    }
                            }
                        }
                    case Telegram.Bot.Types.Enums.MessageType.Document:
                        {
                            string userFolder = destinationPreset + $"{message.Chat.Id}\\";
                            string fileName = $"{message.Document.FileName}";
                            CheckID(message.Chat.Id);
                            documentList[message.Chat.Id].Add(fileName);
                            CheckMainFolder();
                            CheckUserFolder(userFolder);
                            string destinationFilePath = userFolder + fileName;
                            FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
                            var file = await botClient.GetInfoAndDownloadFileAsync(message.Document.FileId, fileStream);
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"Файл {message.Document.FileName} сохранён.");
                            fileStream.Close();
                            return;
                        }
                    case Telegram.Bot.Types.Enums.MessageType.Photo:
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Отправьте фото документом для сохранения.");
                            return;
                        }
                    default:
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Неизвестная команда.");
                            return;
                        }
                }

            }            
        }

        public void SendMessage(string text, string stringID)
        {
            long id = Convert.ToInt64(stringID);
            bot.SendTextMessageAsync(id, text);
        }

        private static Task Error(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public bool IsNumber(string number)
        {
            bool result = true;
            foreach (char digit in number)
                if (!char.IsDigit(digit))
                    result = false;
            return result;
        }

        private void CheckID(long ID)
        {
            if (!documentList.ContainsKey(ID))
                documentList.Add(ID, new List<string>());
        }

        private void CheckMainFolder()
        {
            if (!Directory.Exists(destinationPreset))
            {
                Directory.CreateDirectory(destinationPreset);
            }
        }

        private void CheckUserFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
    }
}
