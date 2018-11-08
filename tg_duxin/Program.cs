using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace tg_duxin {
    class Program {
        private static string lastMessage = "";
        private static bool repeated = false;
        private static TelegramBotClient repeater = new TelegramBotClient(Global.bot_key);
        private static void InitModule() {
            ReplyerBot.Init();
        }
        private static void ExecModule(Message message) {
            if (message.Type == MessageType.Text)
                ReplyerBot.GetResult(message.Text);
            //add modules here
        }

        //本质功能
        private static string Repeate(string message) {
            if (message == lastMessage) {
                if (repeated == false) {
                    repeated = true;
                    return lastMessage;
                }
                else return "";
            }
            lastMessage = message;
            repeated = false;
            return "";
        }
        private static async void OnMessageRecv(object sender,MessageEventArgs msg) {
            Message message = msg.Message;
            switch (message.Type) {
                case MessageType.Text:
                    string s = Repeate(message.Text);
                    if (s.Any())
                        await repeater.SendTextMessageAsync(message.Chat.Id, s);
                    break;
                default:
                    break;
            }
            ExecModule(message);
        }
        
        static void Main(string[] args) {
            repeater.OnMessage += OnMessageRecv;
            InitModule();
            repeater.StartReceiving();
        }
    }
}
