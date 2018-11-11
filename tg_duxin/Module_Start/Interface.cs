using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace tg_duxin.Module_Start {
    class Interface : Module {
        public override void Init() {
            
        }
        public override string GetResult(Message a) {
            return "start信息";//可以考虑加start彩蛋
        }
    }
}
