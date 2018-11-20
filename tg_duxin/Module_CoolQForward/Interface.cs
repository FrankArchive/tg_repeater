using Sisters.WudiLib;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace tg_duxin.Module_CoolQForward {
    class InterfaceSend : Module {
        public InterfaceSend() {
            name = Config.module_name;
            onCommandOnly = false;
            moduleID = Global.cntModules++;
            required = new List<MessageType>{ MessageType.Text/*, MessageType.Photo*/ };
            if (Config.isStarted) return;
            Config.CoolQClient = new HttpApiClient();
            Config.CoolQClient.ApiAddress = Config.apiAddr;
            Config.isStarted = true;
        }
        public override void submitCommands() {
            Global.commandsPool.Add(new List<string> { "/start_send", "/start_recv", "/stop_send", "/stop_recv", "/send" });
            Config.CQsend.Add(new Sisters.WudiLib.Posts.PrivateEndpoint(745679136));//send all to me! for debug
            //Global.commandsPool[moduleID] = new List<string>(new string[] { "/get", "/activate", "/deactivated" });
        }
        public override void Stop() {
            
        }
        public override string GetResult(Telegram.Bot.Types.Message msg) {
            if (msg.Type == MessageType.Text) {
                switch (msg.Text.Split(' ')[0]) {
                    case "/start_send":
                        foreach (Chat i in Config.TGsend)
                            if (i.Id == msg.Chat.Id) return "本来就在向QQ转发消息";
                        Config.TGsend.Add(msg.Chat);
                        return "开始向QQ转发消息";
                    case "/stop_send":
                        foreach (Chat i in Config.TGsend)
                            if (i.Id == msg.Chat.Id) {
                                Config.TGsend.Remove(i);
                                return "停止向QQ转发消息";
                            }
                        return "本来就没有在向QQ转发消息";
                    case "/start_recv":
                        foreach (Chat i in Config.TGrecv) {
                            if (i.Id == msg.Chat.Id)
                                return "本来就在接收来自QQ的消息";
                        }
                        Config.TGrecv.Add(msg.Chat);
                        return "开始接收来自QQ的消息";
                    case "/stop_recv":
                        foreach (Chat i in Config.TGrecv) {
                            if (i.Id == msg.Chat.Id) {
                                Config.TGrecv.Remove(i);
                                return "停止接收QQ消息";
                            }
                        }
                        return "并没有在接收来自QQ的消息";
                    case "/send":
                        foreach (Sisters.WudiLib.Posts.Endpoint i in Config.CQrecv)
                            Config.CoolQClient.SendMessageAsync(i, $"[{msg.From.Username}] {msg.Text}");
                        return "强制转发成功";
                }
            }
            bool isInList = false;
            foreach (Chat i in Config.TGsend)
                if (msg.Chat.Id == i.Id) {
                    isInList = true;
                    break;
                }
            if (!isInList) return "";
            string message_send = "[" + msg.From.Username + "] " + msg.Text;
            foreach (Sisters.WudiLib.Posts.Endpoint i in Config.CQrecv)
                Config.CoolQClient.SendMessageAsync(i, message_send);
            return "";
        }
    }
    class InterfaceRecv : Module {
        public InterfaceRecv() {
            name = Config.module_name;
            moduleID = Global.cntModules++;
            if (Config.isStarted == false) {
                Config.CoolQClient = new HttpApiClient();
                Config.CoolQClient.ApiAddress = Config.apiAddr;
                Config.isStarted = true;
            }
            Config.CoolQListener = new Sisters.WudiLib.Posts.ApiPostListener(Config.listenAddr);
            Config.CoolQListener.ApiClient = Config.CoolQClient;
            Config.CoolQListener.MessageEvent += onMessageRecieved;
            Config.CoolQListener.StartListen();
        }
        private static void onMessageRecieved(HttpApiClient api, Sisters.WudiLib.Posts.Message message) {
            if (Config.CQinSight.IndexOf(message.Endpoint) == -1)
                Config.CQinSight.Add(message.Endpoint);
            switch(message.RawMessage.Split(' ')[0]) {
                case "/start_send":
                    if (Config.CQsend.Contains(message.Endpoint)) {
                        api.SendMessageAsync(
                                message.Endpoint,
                                "本来就在向TG转发消息"
                                );
                        return;
                    }
                    Config.CQsend.Add(message.Endpoint);
                    api.SendMessageAsync(
                                message.Endpoint,
                                "开始向TG转发消息"
                                );
                    return;
                case "/stop_send":
                    if (Config.CQsend.Contains(message.Endpoint)) {
                        Config.CQsend.Remove(message.Endpoint);
                        api.SendMessageAsync(message.Endpoint, "停止向TG转发消息");
                        return;
                    }
                    api.SendMessageAsync(message.Endpoint,"本来就没有向TG转发消息");
                    return;
                case "/start_recv":
                    if (Config.CQrecv.Contains(message.Endpoint)) {
                        api.SendMessageAsync(message.Endpoint, "本来就在接收TG消息");
                        return;
                    }
                    Config.CQrecv.Add(message.Endpoint);
                    api.SendMessageAsync(message.Endpoint, "开始接收TG消息");
                    return;
                case "/stop_recv":
                    if (Config.CQrecv.Contains(message.Endpoint)) {
                        Config.CQrecv.Remove(message.Endpoint);
                        api.SendMessageAsync(message.Endpoint, "停止接收TG消息");
                        return;
                    }
                    api.SendMessageAsync(message.Endpoint, "本来就没有接收TG消息");
                    return;
                case "/send":
                    foreach (Chat i in Config.TGrecv)
                        OptimisticModuleManager.SendText(i, $"[{message.Sender["nickname"].ToString()}] {message.RawMessage}");
                    api.SendMessageAsync(message.Endpoint, "强制转发成功");
                    return;
            }
            if (Config.CQsend.Contains(message.Endpoint) == false) return;
            foreach (Chat i in Config.TGrecv)
                OptimisticModuleManager.SendText(i, $"[{message.Sender["nickname"].ToString()}] {message.RawMessage}");
            //Console.WriteLine(message.Content.Raw);
        }
    }
}
