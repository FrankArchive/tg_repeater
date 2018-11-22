using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace tg_duxin.Module_Start {
    class Interface : Module {
        public override void submitCommands () {
            Global.commandsPool[moduleID] = new List<string> (new string[] { "/start" });
        }
        public Interface () {
            required = new List<MessageType> { MessageType.Text };
            moduleID = Global.cntModules++;
        }
        public override string GetResult (Message a) {
            return "start信息"; //可以考虑加start彩蛋
        }
    }
}