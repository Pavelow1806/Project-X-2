using Core.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public abstract class Packet
    {
        public int PacketNumber;
        public string PacketName = "";
        public List<object> Contents = null;
        public byte[] Data;
        public Packet(PacketParams packetParams)
        {
            PacketNumber = packetParams.packetNumber;
            PacketName = packetParams.packetName;
            Contents = packetParams.contents;
            Data = packetParams.data;
        }
        public override string ToString()
        {
            if (Contents == null || Contents.Count == 0)
                return "Packet contents were not contained.";

            string result = PacketName == null ? "" : $"{PacketName} >";
            if (Contents == null && Contents.Count > 0)
            {
                foreach (object obj in Contents)
                {
                    result += $" | {obj.ToString()}";
                }
            }
            return result;
        }
    }
}
