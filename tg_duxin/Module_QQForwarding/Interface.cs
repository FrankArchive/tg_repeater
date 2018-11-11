using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace tg_duxin.Module_QQForwarding {
    class Interface : Module {
        public readonly new string name = "qq_tg互联bot";
        public readonly new MessageType required = MessageType.Text;//暂时
        public readonly new int moduleID = Global.cntModules++;

        private Reciever server;
        public override void Init() {
            server = new Reciever(OnMessageRecieved, "http://127.0.0.1:8080/test/");
            server.Run();
        }
        public override void Stop() {
            server.Stop();
        }
        public static string OnMessageRecieved(HttpListenerRequest request) {
            if (request.HasEntityBody == false) return Config.APIHelp;
            using (Stream bodyRecv = request.InputStream) {
                StreamReader bodyRead = new StreamReader(bodyRecv, request.ContentEncoding);
                OptimisticModuleManager.SendText(
                    Protocal.Serialize(bodyRead.ReadToEnd()).ToString()
                    );
                return "ok";
            }
        }
    }
}