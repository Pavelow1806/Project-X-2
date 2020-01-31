using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Protocols;

namespace Test_Environment
{
    class Program
    {
        static void Main(string[] args)
        {
            //Log.Write(LogType.Debug, "Hello World!");
            Protocol Protocol = new Protocol(ConnectionType.LOGINSERVER, ProtocolDirection.Inbound, "LoginServerInboundTest");
            PacketDefinition pd1 = new PacketDefinition(0, "Example Packet 1");
            pd1.Types.Add(new DataType(DataTypes.Byte, "Some Byte"));
            pd1.Types.Add(new DataType(DataTypes.Integer, "Some Int"));
            pd1.Types.Add(new DataType(DataTypes.Bytes, "Some Bytes"));
            pd1.Types.Add(new DataType(DataTypes.Float, "Some Float"));
            pd1.Types.Add(new DataType(DataTypes.String, "Some String"));
            Protocol.Definitions.Add(pd1);
            PacketDefinition pd2 = new PacketDefinition(0, "Example Packet 2");
            pd2.Types.Add(new DataType(DataTypes.Byte, "Some Byte"));
            pd2.Types.Add(new DataType(DataTypes.Integer, "Some Int"));
            pd2.Types.Add(new DataType(DataTypes.Bytes, "Some Bytes"));
            pd2.Types.Add(new DataType(DataTypes.Float, "Some Float"));
            pd2.Types.Add(new DataType(DataTypes.String, "Some String"));
            Protocol.Definitions.Add(pd2);
            PacketDefinition pd3 = new PacketDefinition(0, "Example Packet 3");
            pd3.Types.Add(new DataType(DataTypes.Byte, "Some Byte"));
            pd3.Types.Add(new DataType(DataTypes.Integer, "Some Int"));
            pd3.Types.Add(new DataType(DataTypes.Bytes, "Some Bytes"));
            pd3.Types.Add(new DataType(DataTypes.Float, "Some Float"));
            pd3.Types.Add(new DataType(DataTypes.String, "Some String"));
            Protocol.Definitions.Add(pd3);
            PacketDefinition pd4 = new PacketDefinition(0, "Example Packet 4");
            pd4.Types.Add(new DataType(DataTypes.Byte, "Some Byte"));
            pd4.Types.Add(new DataType(DataTypes.Integer, "Some Int"));
            pd4.Types.Add(new DataType(DataTypes.Bytes, "Some Bytes"));
            pd4.Types.Add(new DataType(DataTypes.Float, "Some Float"));
            pd4.Types.Add(new DataType(DataTypes.String, "Some String"));
            Protocol.Definitions.Add(pd4);
            PacketDefinition pd5 = new PacketDefinition(0, "Example Packet 5");
            pd5.Types.Add(new DataType(DataTypes.Byte, "Some Byte"));
            pd5.Types.Add(new DataType(DataTypes.Integer, "Some Int"));
            pd5.Types.Add(new DataType(DataTypes.Bytes, "Some Bytes"));
            pd5.Types.Add(new DataType(DataTypes.Float, "Some Float"));
            pd5.Types.Add(new DataType(DataTypes.String, "Some String"));
            Protocol.Definitions.Add(pd5);
            PacketDefinition pd6 = new PacketDefinition(0, "Example Packet 6");
            pd6.Types.Add(new DataType(DataTypes.Byte, "Some Byte"));
            pd6.Types.Add(new DataType(DataTypes.Integer, "Some Int"));
            pd6.Types.Add(new DataType(DataTypes.Bytes, "Some Bytes"));
            pd6.Types.Add(new DataType(DataTypes.Float, "Some Float"));
            pd6.Types.Add(new DataType(DataTypes.String, "Some String"));
            Protocol.Definitions.Add(pd6);

            string result = Protocol.XML;
            Console.WriteLine(result);
            Console.ReadLine();
        }
    }
}
