using System;
using tg_duxin.Module_ReplyerBot;

namespace tg_duxin.Module_CoolQForward {
    public class Module_QQ {
        public static string GetResult (string msg, string user) {
            string toRep = Program.Repeate (msg, 233);
            if (toRep != "") return msg;

            Command x;
            try {
                x = Parser.ParseCommand (msg, Config.sub_module_ID);
            } catch (CommandErrorException) {
                if (DBAgent.isExist (msg) == false)
                    return "";
                else
                    return DBAgent.Lookup (msg); //带空格的情况
            }
            bool force = true;

            switch (x.operation) {
                case 0: //teach
                    force = false;
                    goto spin_jump_with_eyes_closed;
                case 1: //force
                    force = true;
                    spin_jump_with_eyes_closed: ; //只用一次，就一次
                    if (x.parameters.Count != 2) throw new FormatException ();
                    if (DBAgent.isExist (x.parameters[0])) {
                        if (force) {
                            DBAgent.Update (x.parameters[0], x.parameters[1], user);
                            return "好吧好吧听你的";
                        }
                        return "我已经被安排过这句话了！";
                    }
                    DBAgent.Insert (x.parameters[0], x.parameters[1], user);
                    return "谢谢你，我学会了，你呢?";
                case 2: //reply 实际上是强制回复
                    if (x.parameters.Count > 1) throw new FormatException ();
                    if (DBAgent.isExist (x.parameters[0]))
                        return DBAgent.Lookup (x.parameters[0]);
                    return "我还不会这句话。。。";
                case 3:
                    if (x.parameters.Count > 1) throw new FormatException ();
                    if (DBAgent.isExist (x.parameters[0])) {
                        DBAgent.Erase (x.parameters[0]);
                        return "我。。忘了什么？";
                    }
                    return "我本来就不知道这句话，那你叫我忘掉啥";
                case 4:
                case 5:
                    Newtonsoft.Json.Linq.JObject result;
                    string url = "https://v1.hitokoto.cn/?encode=json&charset=utf-8";
                    if (x.parameters.Count >= 1) {
                        if (x.parameters[0] == "help") {
                            string ret = "可用的类型有:\n";
                            foreach (var i in Module_hitokoto.Interface.types) ret += i + '\n';
                            return ret;
                        }
                        if (Module_hitokoto.Interface.types.Contains (x.parameters[0]))
                            url +=
                            $"&c={Convert.ToChar('a' + Module_hitokoto.Interface.types.IndexOf(x.parameters[0]))}";
                    }
                    try {
                        using (var http = new System.Net.Http.HttpClient ()) {
                            string res = http.GetStringAsync (url).Result;
                            result = (Newtonsoft.Json.Linq.JObject) Newtonsoft.Json.JsonConvert.DeserializeObject (res);
                        }
                    } catch { return "网络错误"; }
                    return $"{result["hitokoto"].ToString()}\n--{result["from"].ToString()}";
                case -1:
                    return "";
            }
            throw new Exception ("有毒");
            //Console.Write("程序不会跑到这里的".Length);
        }
    }
}