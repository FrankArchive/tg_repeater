using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Types;

namespace tg_duxin.Module_CoolQForward
{
    public class NicknameLookup
    {
        // TODO: 连接数据库
        // nickname连接数据库时在Interface.cs进行初始化！
        private static Dictionary<string, string> tg_dict = new Dictionary<string, string>();
        private static Dictionary<string, string> cq_dict = new Dictionary<string, string>();
        
        /// <summary>
        /// 由TG消息处理事件调用，向QQ转发时获取QQ方设置的TG成员昵称
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetTGNickname(User user) {
            if(tg_dict[user.Username] is null)
                tg_dict[user.Username] = user.Username;
            
            return tg_dict[user.Username];
        }
        /// <summary>
        /// 由QQ消息处理事件调用
        /// </summary>
        /// <param name="original"></param>
        /// <param name="nickname"></param>
        public static void SetTGNickname(string original, string nickname) 
            => tg_dict[original] = nickname;


        /// <summary>
        /// 由QQ消息处理事件调用
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetCQNickname(JObject user){
            string uname = user["nickname"].ToString();
            if(cq_dict[uname] is null)
                cq_dict[uname] = uname;
            
            return cq_dict[uname];
        }

        /// <summary>
        /// 由TG消息处理事件调用
        /// </summary>
        /// <param name="original"></param>
        /// <param name="nickname"></param>
        public static void SetCQNickname(string original, string nickname)
            => cq_dict[original] = nickname;
    }
}