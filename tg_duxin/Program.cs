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
        private static async void ExecModule(Message message) {
            string x = "";
            try {
                if (message.Type == MessageType.Text)
                    x = ReplyerBot.GetResult(message.Text, message.From.Username);
                await repeater.SendTextMessageAsync(message.Chat.Id, x);
            }
            catch (Exception e) {
                if (e is NotImplementedException) { }
                else Console.WriteLine($"执行回复模块时遇到了异常：\n{e.ToString()}\n暂时忽略并继续执行\n");
            }

            //add module operations here
            if (message.Type == MessageType.Text && message.Text == "/start")
                await repeater.SendTextMessageAsync(message.Chat.Id, Start.GetResult());
        }

        //本质功能，绝对不是module//连start都是module
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
            Console.WriteLine("已启动");
            Console.ReadLine();
            repeater.StopReceiving();
        }
    }
}
