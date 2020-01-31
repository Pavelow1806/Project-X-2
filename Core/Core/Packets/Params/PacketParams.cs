using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Packets
{
    public sealed class PacketParams
    {
        public int packetNumber;
        public string packetName;
        public List<object> contents;
        public byte[] data;
    }
}
