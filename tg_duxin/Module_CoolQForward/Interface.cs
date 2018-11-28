using Sisters.WudiLib;
using Sisters.WudiLib.Posts;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

//这个Module有点乱，跳过吧
namespace tg_duxin.Module_CoolQForward {
    class InterfaceSend : Module {
        public InterfaceSend() {
            name = Config.module_name;
            onCommandOnly = false;
            moduleID = Global.cntModules++;
            Config.module_ID = moduleID;
            Config.sub_module_ID = Global.cntModules++;

            required = new List<MessageType>{ MessageType.Text/*, MessageType.Photo*/ };
        }
        public override void submitCommands() {
            Global.commandsPool[moduleID] = (new List<string> { "/start_send", "/start_recv", "/stop_send", "/stop_recv", "/send", "/setnick", "/getlast", "/ping" });

            Global.commandsPool[Config.sub_module_ID]
                = new List<string> (new string[] { "/teach", "/force", "/reply", "/delete", "/hitokoto", "/一言" });
            //Config.CQrecv.Add(new Sisters.WudiLib.Posts.PrivateEndpoint(745679136));//send all to me! for debug
        }
        public override void Stop() {
            
        }
        public override string GetResult(Telegram.Bot.Types.Message msg) {
            try {
                Command command = Parser.ParseCommand(msg.Text, moduleID);
                switch(command.operation){
                    case 0://start_send
                        foreach (Chat i in Config.TGsend)
                            if (i.Id == msg.Chat.Id) 
                                return string.Format(Config.alreadySendingMessage, "QQ");
                        Config.TGsend.Add(msg.Chat);
                        return string.Format(Config.startSendMessage, "QQ");
                    case 1://start_recv
                        foreach (Chat i in Config.TGrecv) {
                            if (i.Id == msg.Chat.Id)
                                return string.Format(Config.alreadyRecvingMessage, "QQ");
                        }
                        Config.TGrecv.Add(msg.Chat);
                        return string.Format(Config.startRecvMessage, "QQ");
                    case 2://stop_send
                        foreach (Chat i in Config.TGsend)
                            if (i.Id == msg.Chat.Id) {
                                Config.TGsend.Remove(i);
                                return string.Format(Config.stopSendMessage, "QQ");
                            }
                        return string.Format(Config.notSendingMessage, "QQ");
                    case 3://stop_recv
                        foreach (Chat i in Config.TGrecv) {
                            if (i.Id == msg.Chat.Id) {
                                Config.TGrecv.Remove(i);
                                return string.Format(Config.stopRecvMessage, "QQ");
                            }
                        }
                        return string.Format(Config.notRecvingMessage, "QQ");
                    case 4://send
                        try{
                            foreach (Sisters.WudiLib.Posts.Endpoint i in Config.CQrecv)
                                CoolQHandle.SendTextMessage
                                    (i, $"[{NicknameLookup.GetTGNickname(msg.From)}] {msg.Text.Substring(6)}");
                        }
                        catch(IndexOutOfRangeException){
                            return Config.forceForwardFaliure;
                        }
                        return Config.forceForwardSuccess;
                    case 5://setnick
                        if(command.parameters.Count != 2)
                            return Config.setNickFormatError;
                        NicknameLookup.SetCQNickname(command.parameters[0], command.parameters[1]);
                        //throw new NotImplementedException();
                        return Config.setNickSuccess;
                    case 6://
                        return CoolQHandle.Ping();
                }
            }
            catch(CommandErrorException){
                
            }
            bool isInList = false;
            foreach (Chat i in Config.TGsend)
                if (msg.Chat.Id == i.Id) {
                    isInList = true;
                    break;
                }
            if (!isInList) return "";
            string message_send = $"[{NicknameLookup.GetTGNickname(msg.From)}] {msg.Text}";
            foreach (Sisters.WudiLib.Posts.Endpoint i in Config.CQrecv)
                CoolQHandle.SendTextMessage(i, message_send);
            return "";
        }
    }
    class InterfaceRecv : Module {
        public InterfaceRecv() {
            name = Config.module_name;
            moduleID = Global.cntModules++;
            CoolQHandle.InitCoolQ();
            // nickname连接数据库时在此进行初始化！
        }
        public override void Stop(){
            //should i do anything here?
            Config.CQrecv.Clear();
            Config.CQsend.Clear();
            Config.TGrecv.Clear();
            Config.TGsend.Clear();

            //ok
            return;
        }
    }
}
