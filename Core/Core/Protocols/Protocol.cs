using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core.Protocols
{
    public sealed class Protocol
    {
        public ProtocolDirection Direction { get; } = ProtocolDirection.UNKNOWN;
        public ConnectionType Destination { get; } = ConnectionType.UNKNOWN;
        public string Name { get; } = "";

        public List<PacketDefinition> Definitions = new List<PacketDefinition>();

        private bool isDirty = true;
        private bool IsDirty
        {
            get
            {
                return (isDirty || Definitions.Any(x => x.IsDirty));
            }
        }
        //lol 
        public Protocol(ConnectionType destination, ProtocolDirection direction, string name)
        {
            Destination = destination;
            Direction = direction;
            Name = name;
        }

        private string xml = "";
        public string XML
        {
            get
            {
                if (!IsDirty)
                    return xml;

                XElement definitions = new XElement("Packet-Definitions");
                foreach (PacketDefinition pd in Definitions)
                {
                    definitions.Add(pd.XML);
                }
                XElement Protocol =
                    new XElement("Protocol",
                        new XElement("Direction", Direction.ToString()),
                        new XElement("Destination", Destination.ToString()),
                        new XElement("Protocol-Name", Name),
                        definitions
                    );

                return xml = Protocol.ToString();
            }
        }
    }
}
