using System;
using System.Collections.Generic;
using cqhttp.Cyan.Messages.CQElements;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

//这个Module有点乱，跳过吧
namespace tg_duxin.Module_CoolQForward {
    class InterfaceSend : Module {
        public InterfaceSend () {
            name = Config.module_name;
            onCommandOnly = false;
            moduleID = Global.cntModules++;
            Config.module_ID = moduleID;
            Config.sub_module_ID = Global.cntModules++;
            //cqhttp.Cyan.Logger.verbosity_level = cqhttp.Cyan.Enums.Verbosity.DEBUG;

            required = new List<MessageType> { MessageType.Text /*, MessageType.Photo*/ };
        }
        public override void submitCommands () {
            Global.commandsPool[moduleID] = (new List<string> { "/start_send", "/start_recv", "/stop_send", "/stop_recv", "/send", "/setnick", "/getlast", "/ping" });

            Global.commandsPool[Config.sub_module_ID] = new List<string> (new string[] { "/teach", "/force", "/reply", "/delete", "/hitokoto", "/一言", "/listen", "/点歌" });
            //Config.CQrecv.Add(new Sisters.WudiLib.Posts.PrivateEndpoint(745679136));//send all to me! for debug
        }
        public override void Stop () {

        }
        public override string GetResult (Telegram.Bot.Types.Message msg) {
            bool isInList = false;
            /*if (msg.Type == MessageType.Photo) {
                foreach (Chat i in Config.TGsend)
                    if (msg.Chat.Id == i.Id) {
                        isInList = true;
                        break;
                    }
                if (!isInList) return "";
                string ms = $"[{NicknameLookup.GetTGNickname(msg.From)}] 发了一张图片";
                foreach (var i in Config.CQrecv) {
                    Config.CoolQClient.SendTextAsync (i.Item1, i.Item2, ms);
                    Config.CoolQClient.SendMessageAsync (i.Item1, i.Item2,
                        //new cqhttp.Cyan.Messages.Message (new ElementImage (msg.NewChatPhoto))
                    );
                }
            }*/
            try {
                Command command = Parser.ParseCommand (msg.Text, moduleID);
                switch (command.operation) {
                    case 0: //start_send
                        foreach (Chat i in Config.TGsend)
                            if (i.Id == msg.Chat.Id)
                                return string.Format (Config.alreadySendingMessage, "QQ");
                        Config.TGsend.Add (msg.Chat);
                        return string.Format (Config.startSendMessage, "QQ");
                    case 1: //start_recv
                        foreach (Chat i in Config.TGrecv) {
                            if (i.Id == msg.Chat.Id)
                                return string.Format (Config.alreadyRecvingMessage, "QQ");
                        }
                        Config.TGrecv.Add (msg.Chat);
                        return string.Format (Config.startRecvMessage, "QQ");
                    case 2: //stop_send
                        foreach (Chat i in Config.TGsend)
                            if (i.Id == msg.Chat.Id) {
                                Config.TGsend.Remove (i);
                                return string.Format (Config.stopSendMessage, "QQ");
                            }
                        return string.Format (Config.notSendingMessage, "QQ");
                    case 3: //stop_recv
                        foreach (Chat i in Config.TGrecv) {
                            if (i.Id == msg.Chat.Id) {
                                Config.TGrecv.Remove (i);
                                return string.Format (Config.stopRecvMessage, "QQ");
                            }
                        }
                        return string.Format (Config.notRecvingMessage, "QQ");
                    case 4: //send
                        try {
                            foreach (var i in Config.CQrecv)
                                Config.CoolQClient.SendTextAsync (i.Item1, i.Item2, $"[{NicknameLookup.GetTGNickname(msg.From)}] {msg.Text.Substring(6)}");
                        } catch (IndexOutOfRangeException) {
                            return Config.forceForwardFaliure;
                        }
                        return Config.forceForwardSuccess;
                    case 5: //setnick
                        if (command.parameters.Count != 2)
                            return Config.setNickFormatError;
                        NicknameLookup.SetCQNickname (command.parameters[0], command.parameters[1]);
                        //throw new NotImplementedException();
                        return Config.setNickSuccess;
                }
            } catch (CommandErrorException) {

            }
            foreach (Chat i in Config.TGsend)
                if (msg.Chat.Id == i.Id) {
                    isInList = true;
                    break;
                }
            if (!isInList) return "";
            string message_send = $"[{NicknameLookup.GetTGNickname(msg.From)}] {msg.Text}";
            foreach (var i in Config.CQrecv)
                Config.CoolQClient.SendTextAsync (i.Item1, i.Item2, message_send);
            return "";
        }
    }
    class InterfaceRecv : Module {
        public InterfaceRecv () {
            name = Config.module_name;
            moduleID = Global.cntModules++;
            CoolQHandle.InitCoolQ ();
            // nickname连接数据库时在此进行初始化！
        }
        public override void Stop () {
            //should i do anything here?
            Config.CQrecv.Clear ();
            Config.CQsend.Clear ();
            Config.TGrecv.Clear ();
            Config.TGsend.Clear ();

            //ok
            return;
        }
    }
}