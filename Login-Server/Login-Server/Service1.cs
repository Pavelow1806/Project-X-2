using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace Login_Server
{
    public partial class Service1 : ServiceBase
    {
        Base Base = null;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (args.Length > 0 && args[0] == "DEBUG") Debugger.Launch();
            Log.Write(LogType.Information, "Project-X-2 Login Server starting..");
            Base = new Base();
        }

        protected override void OnStop()
        {

        }
    }
}
