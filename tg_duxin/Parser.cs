using System;
using System.Collections.Generic;
using System.Text;

namespace tg_duxin {
    struct Command {
        public int operation;
        public List<string> parameters;
    }
    class CommandErrorException : Exception { };
    class Parser {
        public static Command ParseCommand(string raw, int moduleId) {
            if (raw.Split(' ')[0][0] != '/')
                throw new CommandErrorException();
            Command ret = new Command();
            string command = raw.Split(' ')[0].Substring(1);
            ret.operation = Global.commandsPool[moduleId].IndexOf(command);
            if (raw.TrimEnd().Length == command.Length) return ret;
            raw = raw.Substring(command.Length+1).Trim();
            if (raw[0] != '"') raw = raw.Insert(0, " ");
            ret.parameters = new List<string>();
            for (int i = 0; i < raw.Length; i++) {
                int x = -1;
                switch (raw[i]) {
                    case '"':
                        x = raw.Substring(i + 1).IndexOf('"');
                        if (x == -1) throw new CommandErrorException();
                        ret.parameters.Add(raw.Substring(i + 1, x - i - 1));
                        i = x;break;
                    case ' ':
                        x = raw.Substring(i + 1).Trim().IndexOf(' ');
                        if (x == -1) {
                            ret.parameters.Add(raw.Substring(i + 1).Trim());
                            i = raw.Length;//break,break!
                        }
                        else {
                            ret.parameters.Add(raw.Substring(i + 1, x - i));
                            i = x;
                        }
                        break;
                }
            }
            return ret;
        }
    }
}
