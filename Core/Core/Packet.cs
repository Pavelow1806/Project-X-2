using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    class Packet
    {
        public ConnectionType Destination;
        public ConnectionType Source;
        public int Index;

        public int PacketNumber;
        public object[] Contents;
        public byte[] Data;
        public Packet(ConnectionType destination, ConnectionType source, object[] contents, byte[] data)
        {
            Destination = destination;
            Source = source;
            Contents = contents;
            Data = data;
        }
        public override string ToString()
        {
            string result = string.Empty;
            if (Contents.Length > 0)
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
