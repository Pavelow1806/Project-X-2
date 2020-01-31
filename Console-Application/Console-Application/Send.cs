using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Application
{
    class Send
    {
        public static void RequestLogs(Core.Server Destination)
        {
            List<object> Contents = new List<object>();
            ByteBuffer.ByteBuffer buffer = new ByteBuffer.ByteBuffer(Contents);
            SendData.BuildBasePacket(ConnectionType.TOOL, (int)ToolSendPacketNumbers.RequestLogs, ref buffer);
            Packet packet = new Packet((int)ToolSendPacketNumbers.RequestLogs, ToolSendPacketNumbers.RequestLogs.ToString(), Destination.IP, -1, Destination.Type, ConnectionType.TOOL, buffer.ToArray());
            SendData.Send(packet);
        }
    }
}
