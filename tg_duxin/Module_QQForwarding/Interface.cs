using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace tg_duxin.Module_QQForwarding {
    class InterfaceListener : Module {
        private Reciever server;
        private bool isStarted = false;
        
        public InterfaceListener() {
            name = "qq_tg互联bot";
            moduleID = Global.cntModules++;
            if (isStarted) return;
            server = new Reciever(OnMessageRecieved, $"http://127.0.0.1:{Config.listenPort}/");
            server.Run();
            isStarted = true;
        }
        public override void Stop() {
            server.Stop();
        }
        public static string OnMessageRecieved(HttpListenerRequest request) {
            if (request.HasEntityBody == false) return Config.APIHelp;
            using (Stream bodyRecv = request.InputStream) {
                StreamReader bodyRead = new StreamReader(bodyRecv, request.ContentEncoding);
                string toSend = "";
                try {
                    toSend = Protocal.Deserialize(bodyRead.ReadToEnd());
                }
                catch(Exception e) {
                    if(e is NotImplementedException) {
                        return "暂不支持发送此格式";
                    }
                    else if(e is FormatException) {
                        return "json格式错误";
                    }
                }
                foreach (ChatId id in Config.chatIdToSendTo)
                    OptimisticModuleManager.SendText(id, toSend, true);
                return "ok";
            }
        }
    }
    class InterfaceCaller : Module {
        public override void submitCommands() {
            Global.commandsPool[moduleID] = new List<string>(new string[] { "/get", "/activate" });
        }
        public InterfaceCaller() {
            name = "tg_qq互联bot";
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
            if (msg.Text.Split(' ')[0] != "/activate" &&
                msg.Text.Split(' ')[0] != "/get") {
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
                            if (msg.Text.Substring(10).Trim() == Config.chatNameToSendTo[i]) {
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
            }
            return "貌似进行了不可能的操作。。。你大概是get了我的shell? tqdl";
        }
    }
}