using System;
using System.Collections.Generic;
using System.Text;
using Sisters.WudiLib;
using Sisters.WudiLib.Posts;
using Telegram.Bot.Types;

namespace tg_duxin.Module_CoolQForward {
    class Config {
        public static string module_name = "酷Q转发(send)";

        public static int listenPort = 2333;

        public static bool isStarted = false;
        public static HttpApiClient CoolQClient;
        public static ApiPostListener CoolQListener;
        public static string apiAddr = "http://localhost:233";
        public static string listenAddr = "http://+:234";

        public static List<Endpoint> CQinSight = new List<Endpoint>();
        public static List<Endpoint> CQsend = new List<Endpoint>();
        public static List<Endpoint> CQrecv = new List<Endpoint>();

        public static List<Chat> TGinSight = new List<Chat>();
        public static List<Chat> TGsend = new List<Chat>();
        public static List<Chat> TGrecv = new List<Chat>();
    }
}
