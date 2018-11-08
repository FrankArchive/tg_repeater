using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace tg_duxin {
    class ReplyerBot {
        private bool isValid = false;
        
        public static void Init() {
            try {
                DBAgent.InitDB();
            }
            catch {
                Console.WriteLine("Database error, fatal...press enter to exit");
                Console.ReadLine();
                Process.GetCurrentProcess().Kill();
            }
        }
        public static string GetResult(string msg) {
            
        }
    }
}
