using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SendDataBase
    {
        public static void Send(Packet packet, Connection connection)
        {
            try
            {
                connection.Stream.BeginWrite(packet.Data, 0, packet.Data.Length, null, null);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
