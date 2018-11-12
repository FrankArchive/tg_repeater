using System;
using System.Collections.Generic;
using System.Text;

namespace tg_duxin.Module_QQForwarding {
    class Config {
        public static readonly string APIHelp = $"请发送一个含有{message_body_key},{message_from_key}与{message_type_key}的json对象";
        public static readonly int listenPort = 2333;
        public static readonly string serverStartedInfo = $"已开始在端口{listenPort}进行监听";

        public static readonly string message_from_key = "from";
        public static readonly string message_body_key = "body";
        public static readonly string message_type_key = "type";

        public static List<string> chatNameToSendTo;
        public static List<long>   chatIdToSendTo;
        public static List<long>   chatIdConfirmed;
    }
}
