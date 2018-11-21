using System;
using Sisters.WudiLib;
using Sisters.WudiLib.Posts;
//using Telegram.Bot.Types;

namespace tg_duxin.Module_CoolQForward
{
    public class CoolQHandle
    {
        public static void InitCoolQ(){
            if (Config.isStarted == false) {
                Config.CoolQClient = new HttpApiClient();
                Config.CoolQClient.ApiAddress = Config.apiAddr;
                Console.WriteLine($"已连接到{Config.apiAddr} 酷Qapi");
                Config.CoolQListener = new Sisters.WudiLib.Posts.ApiPostListener(Config.listenAddr);
                Config.CoolQListener.ApiClient = Config.CoolQClient;
                Config.CoolQListener.MessageEvent += onMessageReceived;
                Config.CoolQListener.AnonymousMessageEvent += onMessageReceived;
                Config.CoolQListener.GroupNoticeEvent += onNoticeReceived;

                Config.CoolQListener.GroupRequestEvent += ApiPostListener.ApproveAllGroupRequests;
                Config.CoolQListener.StartListen();
                Console.WriteLine($"在{Config.listenAddr}进行监听");
                Config.isStarted = true;
            }
        }
        public static async void SendTextMessage(Endpoint i, string message) 
            => await Config.CoolQClient.SendMessageAsync(i, message);
        private static void onNoticeReceived(HttpApiClient api, Sisters.WudiLib.Posts.Message notice){
            // TODO: 群公告通知;
            foreach (Section i in notice.Content.Sections){
                
            }
        }
        private static SendingMessage repeate(Sisters.WudiLib.Posts.Message message){
            throw new NotImplementedException();
        }
        private async static void onMessageReceived(HttpApiClient api, Sisters.WudiLib.Posts.Message message) {
            // LOAD MOD FOR QQ GROUPS TOO!!!
            //api.RecallMessageAsync(message);

            try{
                Command com = Parser.ParseCommand(message.RawMessage, Config.module_ID);

                switch(com.operation){
                    case 0://start_send
                        if (Config.CQsend.Contains(message.Endpoint)) {
                            await api.SendMessageAsync(
                                    message.Endpoint,
                                    string.Format(Config.alreadySendingMessage, "TG")
                                    );
                            return;
                        }
                        Config.CQsend.Add(message.Endpoint);
                        await api.SendMessageAsync(
                                    message.Endpoint,
                                    string.Format(Config.startSendMessage, "TG")
                                    );
                        return;
                    case 1://start_recv
                        if (Config.CQrecv.Contains(message.Endpoint)) {
                            await api.SendMessageAsync(message.Endpoint, 
                                string.Format(Config.alreadyRecvingMessage, "TG"));
                            return;
                        }
                        Config.CQrecv.Add(message.Endpoint);
                        await api.SendMessageAsync(message.Endpoint, 
                            string.Format(Config.startRecvMessage, "TG"));
                        return;
                    case 2://stop_send
                        if (Config.CQsend.Contains(message.Endpoint)) {
                            Config.CQsend.Remove(message.Endpoint);
                            await api.SendMessageAsync(message.Endpoint, 
                                string.Format(Config.stopSendMessage, "TG"));
                            return;
                        }
                        await api.SendMessageAsync(message.Endpoint,
                            string.Format(Config.notSendingMessage, "TG"));
                        return;
                    case 3://stop_recv
                        if (Config.CQrecv.Contains(message.Endpoint)) {
                            Config.CQrecv.Remove(message.Endpoint);
                            await api.SendMessageAsync(message.Endpoint, 
                                string.Format(Config.stopRecvMessage, "TG"));
                            return;
                        }
                        await api.SendMessageAsync(message.Endpoint, 
                            string.Format(Config.notRecvingMessage, "TG"));
                        return;
                    case 4://send
                        try {
                            foreach (Telegram.Bot.Types.Chat i in Config.TGrecv)
                                OptimisticModuleManager.SendText(i, $"[{NicknameLookup.GetCQNickname(message.Sender)}] {message.RawMessage.Substring(6)}");
                            await api.SendMessageAsync(message.Endpoint, Config.forceForwardSuccess);
                        }
                        catch(IndexOutOfRangeException) {
                            await api.SendMessageAsync(message.Endpoint, Config.forceForwardFaliure);
                        }
                        return;
                    case 5://setnick
                        if(com.parameters.Count != 2) {
                            await api.SendMessageAsync(message.Endpoint, Config.setNickFormatError);
                            return;
                        }
                        NicknameLookup.SetTGNickname(com.parameters[0], com.parameters[1]);
                        //throw new NotImplementedException();
                        await api.SendMessageAsync(message.Endpoint, Config.setNickSuccess);
                        return;
                }
            }
            catch(CommandErrorException){

            }
            if (Config.CQsend.Contains(message.Endpoint) == false) return;
            
            foreach (Telegram.Bot.Types.Chat i in Config.TGrecv)
                ConvertSendToTG(i, message);
                //OptimisticModuleManager.SendText(i, $"[{NicknameLookup.GetCQNickname(message.Sender)}] {message.RawMessage}");
        }
        private static void ConvertSendToTG(
            Telegram.Bot.Types.Chat target, 
            Sisters.WudiLib.Posts.Message message
        ){
            foreach (Section i in message.Content.Sections) {
                switch(i.Type) {
                    case Section.TextType:
                        OptimisticModuleManager.SendText(target, $"[{NicknameLookup.GetCQNickname(message.Sender)}] {i.Data["text"]}");
                        return;
                    case Section.ImageType:
                        OptimisticModuleManager.SendText(target, $"{NicknameLookup.GetCQNickname(message.Sender)}发送了图片");
                        OptimisticModuleManager.SendPhoto(target, i.Data["url"]);
                        return;
                    case Section.FaceType:
                        OptimisticModuleManager.SendText(target, $"[{NicknameLookup.GetCQNickname(message.Sender)}] {ConvertEmoji(Convert.ToInt32(i.Data["id"]))}");
                        return;
                }
            }
        }
        private static string ConvertEmoji(int FaceId) {
            return "一个qq表情";
            //TODO: 将qq表情转换为emoji
        }
    }
}