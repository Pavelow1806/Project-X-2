using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ByteBuffer;

namespace Core
{
    public static class ProcessData
    {
        public static ServerPacket Process(Server Source, int DestinationIndex, byte[] data)
        {

        }
        public static ClientPacket Process(Client Source, byte[] data)
        {

            ClientPacket packet;
        }
        public static Packet Process(Connection connection, int index, byte[] data)
        {
            try
            {
                List<object> Contents = new List<object>();
                ByteBuffer.ByteBuffer buffer = new ByteBuffer.ByteBuffer(Contents);
                buffer.WriteBytes(data);

                ConnectionType Source = (ConnectionType)buffer.ReadInteger();

                if (Source == ConnectionType.CLIENT)
                {
                    ClientPacket cp = new ClientPacket(
                        new Packets.PacketParams
                        {
                            data = data,
                            packetNumber = buffer.ReadInteger(),
                            packetName = "Undefined"
                        });
                }

                int ServerIndex = buffer.ReadInteger();
                int PacketNumber = buffer.ReadInteger();

                Packet packet = new Packet(PacketNumber, "", connection.IP, (Source == ConnectionType.CLIENT ? index : ServerIndex), connection.Type, Source, Contents, data);

                return packet;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            return null;
        }
        public static void ReadHeader(ref ByteBuffer.ByteBuffer buffer)
        {
            buffer.ReadInteger();
            buffer.ReadInteger();
            buffer.ReadInteger();
        }
    }
}
