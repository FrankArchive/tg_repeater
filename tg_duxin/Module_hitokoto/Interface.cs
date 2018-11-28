using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace tg_duxin.Module_hitokoto {
    public class Interface : Module {
        public Interface () {
            moduleID = Global.cntModules++;
            required = new System.Collections.Generic.List<Telegram.Bot.Types.Enums.MessageType> { Telegram.Bot.Types.Enums.MessageType.Text };
            name = "一言bot";
        }
        public static List<string> types = new List<string> {
            "Anime",
            "Comic",
            "Game",
            "Novel",
            "Original",
            "Internet",
            "Other"
        };
        public override string GetResult (Telegram.Bot.Types.Message msg) {
            JObject result;
            try {
                Command x = Parser.ParseCommand (msg.Text, moduleID);
                string url = "https://v1.hitokoto.cn/?encode=json&charset=utf-8";
                if (x.parameters.Count >= 1) {
                    if (x.parameters[0] == "help") {
                        string ret = "可用的类型有:\n";
                        foreach (var i in types) ret += i + '\n';
                        return ret;
                    }
                    if (types.Contains (x.parameters[0]))
                        url += $"&c={Convert.ToChar('a' + types.IndexOf(x.parameters[0]))}";
                }
                using (var http = new HttpClient ()) {
                    string res = http.GetStringAsync (url).Result;
                    result = (JObject) JsonConvert.DeserializeObject (res);
                }
            } catch (Exception e) {
                if (e is CommandErrorException) return "命令格式错误";
                else return "网络错误";
            }

            return $"{result["hitokoto"].ToString()}\n--{result["from"].ToString()}";
        }
        public override void submitCommands () {
            Global.commandsPool[moduleID] = (new List<string> { "/hitokoto", "/一言" });
        }
    }
}