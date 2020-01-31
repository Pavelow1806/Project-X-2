using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class SendData
    {
        public static void Send(Packet packet)
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
                        output = CheckDestination(packet.IP, packet.Destination, out server);
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
                Log.Write(ex);
            }
        }
        public static void BuildBasePacket(ConnectionType Source, int packetNumber, ref ByteBuffer.ByteBuffer buffer)
        {
            buffer.WriteInteger((int)Source);
            buffer.WriteInteger(packetNumber);
        }

        private static string CheckDestination(int Index, out Client client)
        {
            if (Index > Constants.MaxConnections || Index < 0)
            {
                client = null;
                return $"The Index {Index.ToString()} was less than 0 or greater than {Constants.MaxConnections.ToString()}.";
            }
            Client c = null;
            lock (Network.Instance.Clients)
            {
                if (Network.Instance.Clients[Index] == null)
                {
                    client = null;
                    return $"The Client object at Index {Index.ToString()} was null.";
                }

                c = Network.Instance.Clients[Index];
            }
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
                client = c;
                return "";
        }
        private static string CheckDestination(string IP, ConnectionType connectionType, out Server server, bool CheckQueue = false)
        {
            if (!Enum.IsDefined(typeof(ConnectionType), connectionType))
            {
                server = null;
                return $"The Server connection type parameter {connectionType.ToString()} was not valid.";
            }
            Server s = null;
            if (CheckQueue)
            {
                lock (Network.Instance.ServerQueue)
                {
                    if (!Network.Instance.ServerQueue.Any(x => x.Type == connectionType))
                    {
                        lock (Network.Instance.Servers)
                        {
                            if (!Network.Instance.Servers.ContainsKey(connectionType))
                            {
                                server = null;
                                if (Network.Instance.ServerQueue.Any(x => x.Type == connectionType))
                                    return $"The Server with connection type {connectionType.ToString()} has connected, but has not yet authenticated.";
                                else
                                    return $"The Server with connection type {connectionType.ToString()} is not connected.";
                            }
                            s = Network.Instance.Servers[connectionType];
                        }
                    }
                    server = Network.Instance.ServerQueue.SingleOrDefault(x => x.IP == IP);
                    if (s == null)
                    {
                        return "The server found could not be found in the server queue";
                    }
                }
            }
            else
            {
                lock (Network.Instance.Servers)
                {
                    if (!Network.Instance.Servers.ContainsKey(connectionType))
                    {
                        server = null;
                        if (Network.Instance.ServerQueue.Any(x => x.Type == connectionType))
                            return $"The Server with connection type {connectionType.ToString()} has connected, but has not yet authenticated.";
                        else
                            return $"The Server with connection type {connectionType.ToString()} is not connected.";
                    }
                    s = Network.Instance.Servers[connectionType];
                }
            }
            if (s == null)
            {
                server = null;
                return $"The socket was null";
            }
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
            else
            {
                server = s;
                return "";
            }
        }

        /// <summary>
        /// Send the authentication packet to the destination server.
        /// </summary>
        /// <param name="Server">This server (this)</param>
        /// <param name="Destination">The Destination server</param>
        public static void Authenticate(ConnectionType Source, Server Destination, string AuthenticationCode)
        {
            List<object> Contents = new List<object>();
            ByteBuffer.ByteBuffer buffer = new ByteBuffer.ByteBuffer(Contents);
            BuildBasePacket(Source, -1, ref buffer);
            buffer.WriteString(AuthenticationCode);
            Server server = null;
            string output = CheckDestination(Destination.IP, Destination.Type, out server, true);
            if (server == null)
            {
                Log.Write(LogType.Error, output);
                return;
            }
            server.Stream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
            Log.Write(LogType.TransmissionOut, $"Authentication packet sent to Server on connection type {server.Type.ToString()}");
        }

        public static void SendLogs(ConnectionType Source, Server Destination)
        {
            List<string> logs = new List<string>(Log.LogContent);
            foreach (string log in logs)
            {
                List<object> Contents = new List<object>();
                ByteBuffer.ByteBuffer buffer = new ByteBuffer.ByteBuffer(Contents);
                buffer.WriteString(log);
                Packet packet = new Packet((int)ToolProcessPacketNumbers.SendLogs, ToolProcessPacketNumbers.SendLogs.ToString(), Destination.IP, -1, Destination.Type, Source, buffer.ToArray());
                Send(packet);
            }
        }
    }
}
