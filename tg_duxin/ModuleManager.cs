using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace tg_duxin {
    public class Module {
        public readonly string name = "";
        public readonly MessageType required = MessageType.Unknown;
        public readonly int moduleID = -1;

        public virtual void Init() {
            throw new NotImplementedException();
        }
        public virtual string GetResult(Message msg) {
            throw new NotImplementedException();
        }
        public virtual void Stop() {
            throw new NotImplementedException();
        }
    }

    class OptimisticModuleManager {//主动式mod
        private static List<Module> pool = new List<Module>();
        public static void LoadModule() {
            pool.Add(new Module_QQForwarding.Interface());
        }
        public static async void SendText(Chat contact, string message) {
            await Program.repeater.SendTextMessageAsync(
                contact.Id,
                message
                );
        }
        public static void InitModule() {
            foreach (Module i in pool)
                i.Init();
        }

        internal static void StopModule() {
            foreach (Module i in pool)
                i.Stop();
        }
    }
    class PassiveModuleManager {//被动式mod
        public static List<Module> pool = new List<Module>();
        public static void LoadModule() {
            pool.Add(new Module_ReplyerBot.Interface());
            pool.Add(new Module_Start.Interface());
        }
        public static void InitModule() {
            foreach (Module i in pool)
                i.Init();
        }
        public static async void OnTrigger(object sender, MessageEventArgs msg) {
            Message message = msg.Message;
            foreach (Module i in pool) {
                try {
                    if (message.Type == i.required &&
                    Global.commandsPool[i.moduleID].Contains(message.Text.Split(' ')[0])
                    )
                        //TODO:非文字消息如何处理？
                        await Program.repeater.SendTextMessageAsync(
                            message.Chat.Id,
                            i.GetResult(message)
                           );
                }
                catch (Exception e) {
                    Console.WriteLine($"执行模块{i.name}时遇到异常\n{e.ToString()}\n继续执行");
                }
            }
        }
    }
}
