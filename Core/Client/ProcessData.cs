﻿using System;
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
        public static Packet Process(Connection connection, byte[] data)
        {
            try
            {
                List<object> Contents = new List<object>();
                ByteBuffer.ByteBuffer buffer = new ByteBuffer.ByteBuffer(Contents);
                buffer.WriteBytes(data);

                ConnectionType Source = (ConnectionType)buffer.ReadInteger();
                ConnectionType Destination = (ConnectionType)buffer.ReadInteger();

                ServerPacket sp = new ServerPacket(
                    new Packets.PacketParams
                    {
                        data = data,
                        packetNumber = buffer.ReadInteger(),
                        packetName = "Undefined"
                    }, Source, Destination);

                return sp;
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
