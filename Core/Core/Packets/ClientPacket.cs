using Core.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ClientPacket : Packet
    {
        public int Index;
        public ClientPacket(PacketParams packetParams, int index) :
            base(packetParams)
        {
            Index = index;
        }
    }
}
