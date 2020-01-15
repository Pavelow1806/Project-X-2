using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SendData
    {
        public  static void Send(Packet packet)
        {
            try
            {
                // TODO: Send packet to either client or server
                switch (packet.Destination)
                {
                    case ConnectionType.GAMESERVER:
                        Network.instance.Servers[packet.Destination].Stream.BeginWrite(packet.Data, 0, packet.Data.Length, null, null);
                        Log.Write(LogType.Information, $"Packet {((GameServerSendPacketNumbers)packet.PacketNumber).ToString()} ({packet.PacketNumber.ToString()}) sent to Game Server.");
                        break;
                    case ConnectionType.CLIENT:
                        Network.instance.Clients[packet.Index].Stream.BeginWrite(packet.Data, 0, packet.Data.ToArray().Length, null, null);
                        Log.Write(LogType.Information, $"Packet {((ClientSendPacketNumbers)packet.PacketNumber).ToString()} ({packet.PacketNumber.ToString()}) sent to Client ({Network.instance.Clients[packet.Index].IP}).");
                        break;
                    case ConnectionType.SYNCSERVER:
                        Network.instance.Servers[packet.Destination].Stream.BeginWrite(packet.Data, 0, packet.Data.ToArray().Length, null, null);
                        Log.Write(LogType.Information, $"Packet {((SyncServerSendPacketNumbers)packet.PacketNumber).ToString()} ({packet.PacketNumber.ToString()}) sent to Synchronization Server.");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                // Output error message
                Log.Write($"An error occurred when attempting to send the following packet:\n{packet.ToString() ?? ""}", ex);
            }
        }
        public static void BuildBasePacket(int packetNumber, ref ByteBuffer.ByteBuffer buffer, ref List<object> Contents)
        {
            buffer.WriteInteger((int)ConnectionType.LOGINSERVER, Contents);
            buffer.WriteInteger(packetNumber, Contents);
        }
    }
}
