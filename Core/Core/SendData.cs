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
                string output;
                switch (packet.DestinationType)
                {
                    case AssetType.CLIENT:
                        Client client = null;
                        output = CheckDestination(packet.Index, out client);
                        if (client == null)
                        {
                            Log.Write(LogType.Error, output);
                            return;
                        }
                        client.Stream.BeginWrite(packet.Data, 0, packet.Data.Length, null, null);
                        Log.Write(LogType.TransmissionOut, $"Packet sent to Client on Index {client.Index.ToString()}: {packet.ToString()}");
                        break;
                    case AssetType.SERVER:
                        Server server = null;
                        output = CheckDestination(packet.Destination, out server);
                        if (server == null)
                        {
                            Log.Write(LogType.Error, output);
                            return;
                        }
                        server.Stream.BeginWrite(packet.Data, 0, packet.Data.Length, null, null);
                        Log.Write(LogType.TransmissionOut, $"Packet sent to Server on connection type {server.Type.ToString()}: {packet.ToString()}");
                        break;
                    case AssetType.NONE:
                        Log.Write(LogType.Error, $"The destination type parameter was incorrect for packet: {packet.ToString()}");
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

        private static string CheckDestination(int Index, out Client client)
        {
            if (Index > Constants.MaxConnections || Index < 0)
            {
                client = null;
                return $"The Index {Index.ToString()} was less than 0 or greater than {Constants.MaxConnections.ToString()}.";
            }
            else if (Network.Instance.Clients[Index] == null)
            {
                client = null;
                return $"The Client object at Index {Index.ToString()} was null.";
            }
            Client c = Network.Instance.Clients[Index];
            if (!c.Connected)
            {
                client = null;
                return $"The Client object at Index {Index.ToString()} was not connected.";
            }
            else if (c.Stream == null)
            {
                client = null;
                return $"The Clients Network Stream on Index {Index.ToString()} was null.";
            }
            else if (!c.Stream.CanWrite)
            {
                client = null;
                return $"The Clients Network Stream on Index {Index.ToString()} is not able to write to the Network Stream.";
            }
            else
            {
                client = c;
                return "";
            }
        }
        private static string CheckDestination(ConnectionType connectionType, out Server server)
        {
            if (!Enum.IsDefined(typeof(ConnectionType), connectionType))
            {
                server = null;
                return $"The Server connection type parameter {connectionType.ToString()} was not valid.";
            }
            else if (!Network.Instance.Servers.ContainsKey(connectionType))
            {
                server = null;
                if (Network.Instance.ServerQueue.Any(x => x.Type == connectionType))
                    return $"The Server with connection type {connectionType.ToString()} has connected, but has not yet authenticated.";
                else
                    return $"The Server with connection type {connectionType.ToString()} is not connected.";
            }
            Server s = Network.Instance.Servers[connectionType];
            if (!s.Connected)
            {
                server = null;
                return $"The Server with connection type {connectionType.ToString()} was not connected.";
            }
            else if (s.Stream == null)
            {
                server = null;
                return $"The Server with connection type {connectionType.ToString()} had a Network Stream that was null.";
            }
            else if (s.Stream.CanWrite)
            {
                server = null;
                return $"The Server with connection type {connectionType.ToString()} is not able to write to the Network Stream.";
            }
            else
            {
                server = s;
                return "";
            }
        }
    }
}
