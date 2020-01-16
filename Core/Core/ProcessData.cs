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
        public static Packet Process(Connection connection, int index, byte[] data)
        {
            try
            {
                ByteBuffer.ByteBuffer buffer = new ByteBuffer.ByteBuffer();
                List<object> Contents = new List<object>();
                buffer.WriteBytes(data, Contents);

                ConnectionType Source = (ConnectionType)buffer.ReadInteger();
                int PacketNumber = buffer.ReadInteger();

                Packet packet = new Packet(PacketNumber, "", index, connection.Type, Source, Contents, data);

                return packet;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            return null;
        }
    }
}
