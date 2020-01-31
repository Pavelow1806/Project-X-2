using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Packets
{
    public class UnauthServerPacket : Packet
    {
        public int SourceIndex = -1;
        public int DestinationIndex = -1;

        public UnauthServerPacket(PacketParams packetParams, int sourceIndex, int destinationIndex) :
            base(packetParams)
        {
            SourceIndex = sourceIndex;
            DestinationIndex = destinationIndex;
        }
    }
}
