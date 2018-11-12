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
        private static Dictionary<long, string> lastMessage = new Dictionary<long, string>();
        private static bool repeated = false;
        public static TelegramBotClient repeater = new TelegramBotClient(Global.bot_key);

        //本质功能，绝对不是module//连start都是module
        private static string Repeate(string message, long id) {
            try {
                if (message == lastMessage[id]) {
                    if (repeated == false) {
                        repeated = true;
                        return lastMessage[id];
                    }
                    else return "";
                }
            }
            catch (KeyNotFoundException) { }
            lastMessage[id] = message;
            repeated = false;
            return "";
        }
        private static async void OnMessageRecv(object sender,MessageEventArgs msg) {
            Message message = msg.Message;
            switch (message.Type) {
                case MessageType.Text:
                    string s = Repeate(message.Text,message.Chat.Id);
                    if (s.Any())
                        await repeater.SendTextMessageAsync(message.Chat.Id, s);
                    break;
                default:
                    break;
            }
        }
        
        static void Main(string[] args) {
            repeater.OnMessage += OnMessageRecv;
            repeater.OnMessage += PassiveModuleManager.OnTrigger;

            PassiveModuleManager.LoadModule();
            OptimisticModuleManager.LoadModule();
            Global.commandsPool = new List<List<string>>();
            for (int i = 0; i < Global.cntModules; i++)
                Global.commandsPool.Add(new List<string>());
            PassiveModuleManager.InitModule();
            OptimisticModuleManager.InitModule();
            

            repeater.StartReceiving();
            Console.WriteLine("已启动");
            Console.ReadLine();
            repeater.StopReceiving();
            OptimisticModuleManager.StopModule();
        }
    }
}
