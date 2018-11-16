using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;

namespace tg_duxin.Module_QQForwarding {
    class InterfaceListener : Module {
        //private Reciever_D server_;
        private Reciever server;
        private bool isStarted = false;
        
        public InterfaceListener() {
            name = Config.module_name;
            moduleID = Global.cntModules++;
            if (isStarted) return;
            server = new Reciever($"http://127.0.0.1:{Config.listenPort}");
            isStarted = true;
        }
        public override void Stop() => server.Stop();

        public static string OnMessageRecieved(string request) {
            string toSend = "";
            if (request.Length == 0) return Config.APIHelp;
            try {
                toSend = Protocal.Deserialize(request);
            }
            catch (Exception e) {
                if (e is NotImplementedException) {
                    return "暂不支持发送此格式";
                }
                else if (e is FormatException) {
                    return "json格式错误\n" + Config.APIHelp;
                }
                else
                    Console.WriteLine($"[QQForwarding]发生了异常\n{e.ToString()}\n继续执行");
            }
            try {
                foreach (ChatId id in Config.chatIdConfirmed)
                    OptimisticModuleManager.SendText(id, toSend, true);
            }
            catch (ApiRequestException) {
                return Config.APIHelp;
            }
            return "ok";
        }
    }
    class InterfaceCaller : Module {
        public override void submitCommands() {
            Global.commandsPool[moduleID] = new List<string>(new string[] { "/get", "/activate", "/deactivated" });
        }
        public InterfaceCaller() {
            name = Config.module_name;
            moduleID = Global.cntModules++;
            onCommandOnly = false;
            required = MessageType.Text;
            Config.chatIdToSendTo = new List<ChatId>();
            Config.chatNameToSendTo = new List<string>();
            Config.chatIdConfirmed = new List<ChatId>();
        }
        private string getName(Message msg) {
            switch (msg.Chat.Type) {
                case ChatType.Supergroup:
                case ChatType.Group:
                    return msg.Chat.Title;
                case ChatType.Private:
                    return msg.Chat.Username;
                default:
                    return "";//throw exception?
            }
        }
        public override string GetResult(Message msg) {
            if (Global.commandsPool[moduleID].IndexOf(msg.Text.Split(' ')[0]) == -1) {
                bool flag = false;
                foreach (ChatId i in Config.chatIdToSendTo)
                    if (i.Identifier == msg.Chat.Id)
                        flag = true;
                if (flag) return "";
                Config.chatIdToSendTo.Add(msg.Chat.Id);
                Config.chatNameToSendTo.Add(getName(msg));
                return "";
            }
            string ret = "";
            switch(msg.Text.Split(' ')[0]) {
                case "/get":
                    ret = "当前接收到了来自\"";
                    foreach (string name in Config.chatNameToSendTo)
                        ret += name + "\" , \"";
                    ret += "\"的消息";return ret;
                case "/activate":
                    try {
                        for (int i = 0; i < Config.chatIdToSendTo.Count; i++) {
                            if (msg.Text.Substring(9).Trim() == Config.chatNameToSendTo[i]) {
                                Config.chatIdConfirmed.Add(Config.chatIdToSendTo[i]);
                                break;
                            }
                        }
                    }
                    catch(Exception e) {
                        Console.WriteLine($"[QQForwarding]添加发送对象过程中出现异常{e.ToString()}");
                        return $"激活中出现了一点问题";
                    }
                    return "成功激活";
                case "/deactivate":
                    foreach (ChatId i in Config.chatIdConfirmed)
                        if (i.Username == msg.Text.Substring(11).Trim()) {
                            Config.chatIdConfirmed.Remove(i);
                            return "成功关闭";
                        }
                    return "未找到";
            }
            return "貌似进行了不可能的操作。。。你大概是get了我的shell? tqdl";
        }
    }
}