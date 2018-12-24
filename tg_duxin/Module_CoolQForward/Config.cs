using System;
using System.Collections.Generic;
using System.Text;
using cqhttp.Cyan.Enums;
using Telegram.Bot.Types;

namespace tg_duxin.Module_CoolQForward {
    class Config {
        public static string module_name = "酷Q转发(send)";
        public static int module_ID, sub_module_ID;
        public static bool isStarted = false;
        public static cqhttp.Cyan.Instance.CQHTTPClient CoolQClient;
        public static string apiAddr = "http://localhost:233";
        public static int listenPort = 234;
        public static string accessToken = System.Environment.GetEnvironmentVariable("CQ_ACCESS_TOKEN");
        public static string secret = System.Environment.GetEnvironmentVariable("CQ_EVENT_SECRET");

        public static Dictionary<string, cqhttp.Cyan.Messages.Message> lastSaid;

        /// <summary>
        /// 列表内的QQ成员/群组能够向TG发出消息
        /// </summary>
        /// <typeparam name="(MessageType,long)"></typeparam>
        /// <returns></returns>
        public static List < (MessageType, long) > CQsend = new List < (MessageType, long) > ();
        /// <summary>
        /// 列表内的QQ成员/群组能够收到来自TG的消息
        /// </summary>
        /// <typeparam name="(MessageType,long)"></typeparam>
        /// <returns></returns>
        public static List < (MessageType, long) > CQrecv = new List < (MessageType, long) > ();

        /// <summary>
        /// 列表内的TG成员/群组能够向QQ发送消息
        /// </summary>
        /// <typeparam name="Chat"></typeparam>
        /// <returns></returns>
        public static List<Chat> TGsend = new List<Chat> ();
        /// <summary>
        /// 列表内的TG成员/群组能够收到来自QQ的消息
        /// </summary>
        /// <typeparam name="Chat"></typeparam>
        /// <returns></returns>
        public static List<Chat> TGrecv = new List<Chat> ();

        public static string forceForwardFaliure = "要我转发什么？";
        public static string forceForwardSuccess = "强制转发成功";
        public static string setNickFormatError = "设置昵称格式错误";
        public static string setNickSuccess = "昵称设置成功";
        public static string alreadySendingMessage = "本来就在向{0}转发消息";
        public static string alreadyRecvingMessage = "本来就在接收{0}的消息";
        public static string notSendingMessage = "本来就没有在向{0}转发消息";
        public static string notRecvingMessage = "本来就没有接收来自{0}的消息";
        public static string startSendMessage = "开始向{0}转发消息";
        public static string stopSendMessage = "停止向{0}转发消息";
        public static string startRecvMessage = "开始接收{0}的消息";
        public static string stopRecvMessage = "停止接收{0}的消息";
    }
}