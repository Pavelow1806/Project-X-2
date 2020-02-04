using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SendData : SendDataBase
    {
        public static void Send(Packet packet)
        {
            try
            {
                string output;
                //switch (packet.DestinationType)
                //{
                //    case AssetType.CLIENT:
                //        Client client = null;
                //        output = CheckDestination(packet.Index, out client);
                //        if (client == null)
                //        {
                //            Log.Write(LogType.Error, output);
                //            return;
                //        }
                //        client.Stream.BeginWrite(packet.Data, 0, packet.Data.Length, null, null);
                //        Log.Write(LogType.TransmissionOut, $"Packet sent to Client on Index {client.Index.ToString()}: {packet.ToString()}");
                //        break;
                //    case AssetType.SERVER:
                //        Server server = null;
                //        output = CheckDestination(packet.IP, packet.Destination, out server);
                //        if (server == null)
                //        {
                //            Log.Write(LogType.Error, output);
                //            return;
                //        }
                //        server.Stream.BeginWrite(packet.Data, 0, packet.Data.Length, null, null);
                //        Log.Write(LogType.TransmissionOut, $"Packet sent to Server on connection type {server.Type.ToString()}: {packet.ToString()}");
                //        break;
                //    case AssetType.NONE:
                //        Log.Write(LogType.Error, $"The destination type parameter was incorrect for packet: {packet.ToString()}");
                //        break;
                //}
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
        /// <summary>
        /// Build the header for sending to a server from either a client or server.
        /// </summary>
        /// <param name="DestinationType">The </param>
        /// <param name="Destination"></param>
        /// <param name="PacketNumber"></param>
        /// <param name="buffer"></param>
        public new static void BuildBasePacket(AssetType DestinationType, Server Destination, int PacketNumber, ref ByteBuffer.ByteBuffer buffer)
        {
            buffer.WriteInteger((int)DestinationType);
            if (Destination.Authenticated)
            {
                buffer.WriteInteger((int)Network.Instance.MyConnectionType);
                buffer.WriteInteger((int)Destination.Type);
            }
            else
            {
                buffer.WriteInteger(Destination.MyIndex);
                buffer.WriteInteger(Destination.Index);
            }
            buffer.WriteInteger(PacketNumber);
        }
        /// <summary>
        /// Build the header for sending from a server to a client.
        /// </summary>
        /// <param name="Destination">The Client that will be receiving the packet.</param>
        /// <param name="packetNumber">The number, to be translated into execution via enum.</param>
        /// <param name="buffer">The buffer being used to write to.</param>
        //public static void BuildBasePacket(Client Destination, int packetNumber, ref ByteBuffer.ByteBuffer buffer)
        //{
        // Source
        // Destination
        //    buffer.WriteInteger(packetNumber);
        //}

        //private static string CheckDestination(int Index, out Client client)
        //{
        //    if (Index > Constants.MaxConnections || Index < 0)
        //    {
        //        client = null;
        //        return $"The Index {Index.ToString()} was less than 0 or greater than {Constants.MaxConnections.ToString()}.";
        //    }
        //    Client c = null;
        //    lock (NetworkBase.Instance.Clients)
        //    {
        //        if (NetworkBase.Instance.Clients[Index] == null)
        //        {
        //            client = null;
        //            return $"The Client object at Index {Index.ToString()} was null.";
        //        }

        //        c = NetworkBase.Instance.Clients[Index];
        //    }
        //    if (!c.Connected)
        //    {
        //        client = null;
        //        return $"The Client object at Index {Index.ToString()} was not connected.";
        //    }
        //    else if (c.Stream == null)
        //    {
        //        client = null;
        //        return $"The Clients Network Stream on Index {Index.ToString()} was null.";
        //    }
        //    else if (!c.Stream.CanWrite)
        //    {
        //        client = null;
        //        return $"The Clients Network Stream on Index {Index.ToString()} is not able to write to the Network Stream.";
        //    }
        //        client = c;
        //        return "";
        //}
        private static string CheckDestination(string IP, ConnectionType connectionType, out Server server, int Index = -1)
        {
            if (!Enum.IsDefined(typeof(ConnectionType), connectionType))
            {
                server = null;
                return $"The Server connection type parameter {connectionType.ToString()} was not valid.";
            }
            Server s = null;
            if (Index > -1)
            {
                lock (Network.Instance.ServerQueue)
                {
                    if (!Network.Instance.ServerQueue.Any(x => x.Index == Index))
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
                    s = Network.Instance.ServerQueue.SingleOrDefault(x => x.Index == Index);
                    if (s == null)
                    {
                        server = null;
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
        public static void Authenticate(ConnectionType Source, int Index, Server Destination, string AuthenticationCode)
        {
            List<object> Contents = new List<object>();
            ByteBuffer.ByteBuffer buffer = new ByteBuffer.ByteBuffer(Contents);
            //BuildBasePacket(Source, Index, -1, ref buffer);
            buffer.WriteString(AuthenticationCode);
            Server server = null;
            string output = CheckDestination(Destination.IP, Destination.Type, out server, Destination.Index);
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
                //Packet packet = new Packet((int)ToolProcessPacketNumbers.SendLogs, ToolProcessPacketNumbers.SendLogs.ToString(), Destination.IP, -1, Destination.Type, Source, buffer.ToArray());
                //Send(packet);
            }
        }
    }
}
