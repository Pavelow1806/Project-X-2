using Core.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ServerPacket : Packet
    {
        public ConnectionType Source = ConnectionType.UNKNOWN;
        public ConnectionType Destination = ConnectionType.UNKNOWN;
        public AssetType DestinationType
        {
            get
            {
                if (Destination != ConnectionType.UNKNOWN)
                {
                    if (Destination != ConnectionType.CLIENT && Destination != ConnectionType.TOOL)
                    {
                        return AssetType.SERVER;
                    }
                    else if (Destination == ConnectionType.CLIENT)
                    {
                        return AssetType.CLIENT;
                    }
                    else
                    {
                        return AssetType.TOOL;
                    }
                }
                else
                {
                    return AssetType.NONE;
                }
            }
        }

        public ServerPacket(PacketParams packetPararms, ConnectionType source, ConnectionType destination) : 
            base(packetPararms) 
        {
            Source = source;
            Destination = destination;
        }
    }
}
