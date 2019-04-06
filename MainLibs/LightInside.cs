using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TejiAdesa.MainLibs
{
    public class StartLight
    {
        public String ServerName { get; set; }
        public String ServerUser { get; set; }
        public String ServerPassword { get; set; }
        public IList<Statement> ScriptsList { get; set; }

        public StartLight(IList<Statement> myListObj)
        {
            ScriptsList = myListObj;
        }

        public StartLight()
        {
        }
    }

    public class Statement
    {
        public String Identity { get; set; }
        public String Label { get; set; }
        public String Description { get; set; }
        public String ScriptToLaunch { get; set; }

        public string OperationType { get; set; }

        public string Parameters { get; set; }

        public String ServerName { get; set; }
        public String ServerUser { get; set; }
        public String ServerPassword { get; set; }
    }
}
