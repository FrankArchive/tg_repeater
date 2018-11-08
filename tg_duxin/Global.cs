using System;
using System.Collections.Generic;
using System.Text;

namespace tg_duxin {
    class Global {
        public static List<List<string> > commandsPool
            = new List<List<string>>();
        public static readonly string bot_key =
            "777763230:AAE-nufwr7d8iiK517BVmfBZswjl6q4ZdRM";

        public static string databaseName = "bot.db";
        public static int    databaseVersion = 3;
        public static string databaseTablename = "map";

        public static int cntModules = 0;
    }
}
