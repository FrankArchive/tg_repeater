using System;
using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace tg_duxin.Module_ReplyerBot {
    class DBAgent {
        
        private static SQLHelper data;
        public static void InitDB() {
            if (File.Exists(Config.databaseName) == false)
                SQLiteConnection.CreateFile(Config.databaseName);
            data = new SQLHelper($"data source={Config.databaseName};");
            data.CreateTable(
                Config.databaseTablename,
                new string[] { "key", "val", "create_by", "last_edit" },
                new string[] { "text", "text", "text", "text" }
                );
        }
        public static void Insert(string w, string d, string user) {
            data.InsertValues(Config.databaseTablename, new string[] { w, d, user, "" });
        }
        public static void Erase(string w) {
            data.DeleteValuesAND(
                Config.databaseTablename, 
                new string[] { "key" }, 
                new string[] { w }, 
                new string[] { "=" }
                );
        }
        public static void Update(string w, string d, string user) {
            data.UpdateValues(Config.databaseTablename, 
                new string[] { "val", "last_edit" }, new string[] { d, user }, "key", w);
        }
        public static bool isExist(string w) {//emmmmm这里写的丑是因为刚才没仔细想sql该咋写
            SQLiteDataReader d = data.ReadTable(
                Config.databaseTablename,
                new string[] { "val" },
                new string[] { "key" },
                new string[] { "=" },
                new string[] { $"\'{w}\'" }
                );
            if (d.Read()) { return true; }
            else { return false; }
        }
        public static string Lookup(string w) {
            SQLiteDataReader d = data.ReadTable(
                Config.databaseTablename,
                new string[] { "val" },
                new string[] { "key" },
                new string[] { "=" },
                new string[] { $"\'{w}\'" }
                );
            string ret = "";
            while (d.Read())
                ret = d["val"].ToString();
            return ret;
        }
    }
}
