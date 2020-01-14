using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    class Client : Connection
    {
        public string Email = "";
        public bool LoggedIn = false;
        public DateTime LoggedInTime = default(DateTime);

        public bool Available 
        { 
            get 
            { 
                if (Socket == null) 
                    return true;
                return false;
            } 
        }

        public Client(ConnectionType type, int id) :
            base(type, id)
        {

        }

        public override void Start()
        {
            base.Start();
            SessionID = Index.ToString("000") + " - " + IP + " - " + ConnectedTime.ToString("yyyy/MM/dd hh:mm:ss");
        }

        public override void Close()
        {
            Username = "";
            Email = "";
            LoggedIn = false;
            LoggedInTime = default(DateTime);
            base.Close();
        }
    }
}
