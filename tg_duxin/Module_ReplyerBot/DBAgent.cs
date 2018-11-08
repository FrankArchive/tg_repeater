using System;
using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace tg_duxin {
    class DBAgent {
        
        private static SqLiteHelper data;
        public static void InitDB() {
            if (File.Exists(Global.databaseName) == false)
                SQLiteConnection.CreateFile(Global.databaseName);
            data = new SqLiteHelper($"data source={Global.databaseName};");
            data.CreateTable(
                Global.databaseTablename,
                new string[] { "key", "val", "user" },
                new string[] { "varchar(50)", "varchar(50)", "varchar(50)" }
                );
        }
        public static void Insert(string w, string d, string user) {
            data.InsertValues(Global.databaseTablename, new string[] { w, d, user });
        }
        public static void Erase(string w) {
            data.DeleteValuesAND(
                Global.databaseTablename, 
                new string[] { "key" }, 
                new string[] { w }, 
                new string[] { "=" }
                );
        }
        public static void Update(string w, string d) {
            data.UpdateValues(Global.databaseTablename, new string[] { "val" }, new string[] { d }, "key", w);
        }
        public static bool isExist(string w) {//emmmmm这里写的丑是因为刚才没仔细想sql该咋写
            SQLiteDataReader d = data.ReadTable(
                Global.databaseTablename,
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
                Global.databaseTablename,
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
