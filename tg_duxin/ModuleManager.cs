using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace tg_duxin {
    public class Module {
        public string name = "";
        public List<MessageType> required = new List<MessageType>{ MessageType.Unknown };
        public int moduleID = -1;
        public bool onCommandOnly = true;
        public virtual void submitCommands() 
            => throw new NotImplementedException();
        public virtual string GetResult(Message msg) 
            => throw new NotImplementedException();
        public virtual void Stop() => throw new NotImplementedException();
    }

    class OptimisticModuleManager {//主动式mod
        private static List<Module> pool = new List<Module>();
        public static void LoadModule() {
            //pool.Add(new Module_QQForwarding.InterfaceListener());
            pool.Add(new Module_CoolQForward.InterfaceRecv());
        }
        public static async void SendText(ChatId chatID, string message, bool isMD = false) {
            if (isMD)
                await Program.repeater.SendTextMessageAsync(
                    chatID,
                    message,
                    ParseMode.Markdown
                );
            else
                await Program.repeater.SendTextMessageAsync(
                    chatID,
                    message
                );
        }
        public static async void SendPhoto(ChatId chatId, string url) {
            try{
                await Program.repeater.SendPhotoAsync (
                    chatId,
                    url
                );
            }
            catch(ApiRequestException) {
                await Program.repeater.SendTextMessageAsync (
                    chatId,
                    $"获取图片失败，url为:{url}"
                );
            }
        }
        public static void InitModule() {
        //    foreach (Module i in pool)
        //        i.submitCommands();
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
            //pool.Add(new Module_QQForwarding.InterfaceCaller());
            pool.Add(new Module_CoolQForward.InterfaceSend());
        }
        public static void InitModule() {
            foreach (Module i in pool)
                i.submitCommands();
        }
        public static async void OnTrigger(object sender, MessageEventArgs msg) {
            Message message = msg.Message;
            foreach (Module i in pool) {
                try {
                    if (i.required.IndexOf(message.Type) != -1 &&
                    (
                        (!i.onCommandOnly)||
                        Global.commandsPool[i.moduleID].
                            Contains(message.Text.Split(' ')[0]))
                    ) {
                        string sendBack = i.GetResult(message);
                        if (sendBack == "") continue;
                        //TODO:非文字消息如何处理？
                        try {
                            await Program.repeater.SendTextMessageAsync(
                                message.Chat.Id,
                                sendBack
                               );
                        }
                        catch (Exception e) {
                            if (e is NotImplementedException) { }
                            else if(e is FormatException) {
                                await Program.repeater.SendTextMessageAsync(
                                    message.Chat.Id,
                                    "命令格式错误"
                                );
                            }
                        }
                    }
                }
                catch (Exception e) {
                    Console.WriteLine($"执行模块{i.name}时遇到异常\n{e.ToString()}\n继续执行");
                }
            }
        }
    }
}
