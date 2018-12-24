using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using cqhttp.Cyan.Enums;
using cqhttp.Cyan.Events.CQEvents;
using cqhttp.Cyan.Events.CQEvents.Base;
using cqhttp.Cyan.Events.CQEvents.CQResponses;
using cqhttp.Cyan.Events.CQEvents.CQResponses.Base;
using cqhttp.Cyan.Instance;
using cqhttp.Cyan.Messages;
using cqhttp.Cyan.Messages.CQElements.Base;
using tg_duxin;
using tg_duxin.Module_CoolQForward;
//using Telegram.Bot.Types;

namespace tg_duxin.Module_CoolQForward {
    public class CoolQHandle {
        public static void InitCoolQ () {
            if (Config.isStarted == false) {
                Config.lastSaid =
                    new System.Collections.Generic.Dictionary<string, Message> ();
                Config.CoolQClient = new cqhttp.Cyan.Instance.CQHTTPClient (
                    accessUrl: Config.apiAddr,
                    listen_port: Config.listenPort,
                    accessToken: Config.accessToken,
                    secret: Config.secret
                );
                Config.CoolQClient.OnEventDelegate += HandleEvent;
                Console.WriteLine ($"已连接到{Config.apiAddr} 酷Qapi");
                Config.isStarted = true;
                cqhttp.Cyan.Logger.verbosity_level = Verbosity.INFO;
            }
        }
        private static CQResponse HandleEvent (CQApiClient client, CQEvent e) {
            if (e is MessageEvent) {
                onMessageReceived (e as MessageEvent);
            }
            return new EmptyResponse ();
        }
        private static long getFromNum (MessageEvent e) {
            if (e is GroupMessageEvent)
                return (e as GroupMessageEvent).group_id;
            else if (e is PrivateMessageEvent)
                return (e as PrivateMessageEvent).sender_id;
            else return (e as DiscussMessageEvent).discuss_id;
        }
        private static string raw_string (Message m) {
            string constructor = "";
            foreach (var i in m.data) {
                switch (i.type) {
                    case "text":
                        constructor += i.data["text"];
                        break;
                    case "face":
                    case "emoji":
                        constructor += i.data["id"].GetHashCode ();
                        break;
                }
            }
            return constructor;
        }
        private static Dictionary<long, Message> lastMessage = new Dictionary<long, Message> ();
        private static bool repeated = false;
        private static Message Repeate (Message message, long id) {
            try {
                if (message == lastMessage[id]) {
                    if (repeated == false) {
                        repeated = true;
                        return lastMessage[id];
                    } else return null;
                }
            } catch (KeyNotFoundException) { }
            lastMessage[id] = message;
            repeated = false;
            return null;
        }
        static async void WriteLog (string message) {
            using (StreamWriter sw = new StreamWriter ("messageLog.log", true)) {
                await sw.WriteLineAsync (message);
            }
        }
        private async static void onMessageReceived (MessageEvent e) {
            (MessageType, long) endPoint = (e.messageType, getFromNum (e));
            // LOAD MOD FOR QQ GROUPS TOO!!!
            //api.RecallMessageAsync(message);
            Console.WriteLine (
                $"收到来自{e.sender.nickname}的QQ消息{e.message.raw_data_cq}"
            );
            WriteLog ($"收到来自{e.sender.nickname}的QQ消息{e.message.raw_data_cq}");
            Message torep = Repeate (e.message, endPoint.Item2);
            if (torep != null) {
                await Config.CoolQClient.SendMessageAsync (
                    endPoint.Item1,
                    endPoint.Item2,
                    torep
                );
            }
            string m = raw_string (e.message);
            if (e.message.raw_data_cq.Contains ("[CQ:") == false) {
                try {
                    Message result =
                        Module_QQ.GetResult (m, e.sender.nickname);
                    if (result.data[0].type != "text" || result.data[0].data["text"] != "") {
                        await Config.CoolQClient.SendMessageAsync (
                            e.messageType,
                            getFromNum (e),
                            result
                        );
                    }
                } catch { }

                try {
                    Config.lastSaid[e.sender.nickname] = e.message;
                } catch { }
                try {
                    Command com = Parser.ParseCommand (m, Config.module_ID);
                    switch (com.operation) {
                        case 0: //start_send
                            if (Config.CQsend.Contains (endPoint)) {
                                await Config.CoolQClient.SendTextAsync (
                                    endPoint.Item1, endPoint.Item2,
                                    string.Format (Config.alreadySendingMessage, "TG")
                                );
                                return;
                            }
                            Config.CQsend.Add (endPoint);
                            await Config.CoolQClient.SendTextAsync (
                                endPoint.Item1, endPoint.Item2,
                                string.Format (Config.startSendMessage, "TG")
                            );
                            return;
                        case 1: //start_recv
                            if (Config.CQrecv.Contains (endPoint)) {
                                await Config.CoolQClient.SendTextAsync (
                                    endPoint.Item1, endPoint.Item2,
                                    string.Format (Config.alreadyRecvingMessage, "TG")
                                );
                                return;
                            }
                            Config.CQrecv.Add (endPoint);
                            await Config.CoolQClient.SendTextAsync (
                                endPoint.Item1, endPoint.Item2,
                                string.Format (Config.startRecvMessage, "TG")
                            );
                            return;
                        case 2: //stop_send
                            if (Config.CQsend.Contains (endPoint)) {
                                Config.CQsend.Remove (endPoint);
                                await Config.CoolQClient.SendTextAsync (
                                    endPoint.Item1, endPoint.Item2,
                                    string.Format (Config.stopSendMessage, "TG")
                                );
                                return;
                            }
                            await Config.CoolQClient.SendTextAsync (
                                endPoint.Item1, endPoint.Item2,
                                string.Format (Config.notSendingMessage, "TG")
                            );
                            return;
                        case 3: //stop_recv
                            if (Config.CQrecv.Contains (endPoint)) {
                                Config.CQrecv.Remove (endPoint);
                                await Config.CoolQClient.SendTextAsync (
                                    endPoint.Item1, endPoint.Item2,
                                    string.Format (Config.stopRecvMessage, "TG")
                                );
                                return;
                            }
                            await Config.CoolQClient.SendTextAsync (
                                endPoint.Item1, endPoint.Item2,
                                string.Format (Config.notRecvingMessage, "TG")
                            );
                            return;
                        case 4: //send
                            try {
                                foreach (Telegram.Bot.Types.Chat i in Config.TGrecv)
                                    ConvertSendToTG (i,
                                        e.message,
                                        NicknameLookup.GetCQNickname (e.sender.nickname)
                                    );
                                await Config.CoolQClient.SendTextAsync (
                                    endPoint.Item1, endPoint.Item2,
                                    Config.forceForwardSuccess
                                );
                            } catch (IndexOutOfRangeException) {
                                await Config.CoolQClient.SendTextAsync (
                                    endPoint.Item1, endPoint.Item2,
                                    Config.forceForwardFaliure
                                );
                            }
                            return;
                        case 5: //setnick
                            if (com.parameters.Count != 2) {
                                await Config.CoolQClient.SendTextAsync (
                                    endPoint.Item1, endPoint.Item2,
                                    Config.setNickFormatError
                                );
                                return;
                            }
                            NicknameLookup.SetTGNickname (com.parameters[0], com.parameters[1]);
                            //throw new NotImplementedException();
                            await Config.CoolQClient.SendTextAsync (
                                endPoint.Item1, endPoint.Item2,
                                Config.setNickSuccess
                            );
                            return;
                        case 6: //getlast
                            if (com.parameters.Count != 1 || !Config.lastSaid.ContainsKey (com.parameters[0])) {
                                await Config.CoolQClient.SendTextAsync (endPoint.Item1, endPoint.Item2, "爱莫能助");
                                return;
                            }
                            await Config.CoolQClient.SendTextAsync (
                                endPoint.Item1, endPoint.Item2,
                                $"{com.parameters[0]} 刚才发了:"
                            );
                            await Config.CoolQClient.SendMessageAsync (
                                endPoint.Item1, endPoint.Item2,
                                Config.lastSaid[com.parameters[0]]
                            );
                            return;

                    }
                } catch (CommandErrorException) {

                }
            }
            if (Config.CQsend.Contains (endPoint) == false) return;

            foreach (Telegram.Bot.Types.Chat i in Config.TGrecv)
                ConvertSendToTG (i, e.message, e.sender.nickname);
            //OptimisticModuleManager.SendText(i, $"[{NicknameLookup.GetCQNickname(message.Sender)}] {message.RawMessage}");
        }
        private static void ConvertSendToTG (
            Telegram.Bot.Types.Chat target,
            cqhttp.Cyan.Messages.Message message,
            string sender_nick
        ) {
            foreach (var i in message.data) {
                switch (i.type) {
                    case "text":
                        OptimisticModuleManager.SendText (target, $"[{NicknameLookup.GetCQNickname(sender_nick)}] {i.data["text"]}");
                        break;
                    case "image":
                        OptimisticModuleManager.SendText (target, $"{NicknameLookup.GetCQNickname(sender_nick)}发送了图片");
                        OptimisticModuleManager.SendPhoto (target, i.data["url"]);
                        break;
                    case "face":
                        OptimisticModuleManager.SendText (target, $"[{NicknameLookup.GetCQNickname(sender_nick)}] " +
                            $"{ConvertEmoji(Convert.ToInt32(i.data["id"]))}");
                        break;
                }
            }
        }

        private static string ConvertEmoji (int FaceId) {
            return "一个qq表情";
            //TODO: 将qq表情转换为emoji
        }
    }
}