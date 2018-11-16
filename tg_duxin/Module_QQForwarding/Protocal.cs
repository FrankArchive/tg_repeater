using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace tg_duxin.Module_QQForwarding {
    class Protocal {
        public static string Deserialize(string raw) {
            JObject res = new JObject();
            try {
                res = (JObject)JsonConvert.DeserializeObject(raw);
            }
            catch (JsonException) {
                throw new FormatException();
            }
            string body, from, type;
            try {
                body = res[Config.message_body_key].ToString();
                from = res[Config.message_from_key].ToString();
                type = res[Config.message_type_key].ToString();
            }
            catch (NullReferenceException) {
                throw new FormatException();
            }
            switch (type) {
                case "text":
                    return $"***[{from}]*** {body}";
                default:
                    throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }
        public static string Serialize(Message obj) {
            throw new NotImplementedException();
        }
    }
}
