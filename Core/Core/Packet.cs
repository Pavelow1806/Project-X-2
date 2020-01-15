using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Packet
    {
        public ConnectionType Destination;
        public ConnectionType Source;
        public int Index;

        public int PacketNumber;
        public string PacketName = "";
        public List<object> Contents = null;
        public byte[] Data;
        public Packet(int packetNumber, string packetName, int index, ConnectionType destination, ConnectionType source, byte[] data)
        {
            PacketNumber = packetNumber;
            PacketName = packetName;
            Index = index;
            Destination = destination;
            Source = source;
            Data = data;
        }
        public Packet(int packetNumber, string packetName, int index, ConnectionType destination, ConnectionType source, List<object> contents, byte[] data)
        {
            PacketNumber = packetNumber;
            PacketName = packetName;
            Index = index;
            Destination = destination;
            Source = source;
            Contents = contents;
            Data = data;
        }
        public override string ToString()
        {
            string result = PacketName == null ? "" : $"{PacketName} >";
            if (Contents == null && Contents.Count > 0)
            {
                foreach (object obj in Contents)
                {
                    result += $" | {obj.ToString()}";
                }
                return result;
            }
            return string.Empty;
        }
    }
}
