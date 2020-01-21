using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login_Server
{
    class ProcessPacket
    {
        public static void Process(object sender, PacketEventArgs e)
        {
            Log.Write(LogType.Information, $"Starting to process packet:\n{e.Packet.ToString()}");
            switch (e.Packet.Source)
            {
                case ConnectionType.NONE:
                    Log.Write(LogType.Error, $"The source type was invalid");
                    return;
                case ConnectionType.GAMESERVER:
                    break;
                case ConnectionType.CLIENT:
                    break;
                case ConnectionType.LOGINSERVER:
                    Log.Write(LogType.Error, $"The source type was invalid");
                    return;
                case ConnectionType.SYNCSERVER:
                    break;
                case ConnectionType.TOOL:
                    Process_ToolPacket(e.Packet);
                    break;
                default:
                    Log.Write(LogType.Error, $"The source type was invalid");
                    return;
            }
            Log.Write(LogType.Information, $"Finished processing packet:\n{e.Packet.ToString()}");
        }
        public static void Process_ToolPacket(Packet packet)
        {
            switch ((ToolSendPacketNumbers)packet.PacketNumber)
            {
                case ToolSendPacketNumbers.RequestLogs:
                    SendData.SendLogs(packet.Destination, packet.Source);
                    break;
                default:
                    break;
            }
        }
    }
}
