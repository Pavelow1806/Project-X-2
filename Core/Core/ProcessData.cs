using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ByteBuffer;

namespace Core
{
    class ProcessData
    {
        #region Locking
        private static readonly object lockObj = new object();
        #endregion
        public static void processData(int index, byte[] data)
        {
            lock (lockObj)
            {
                try
                {
                    ByteBuffer.ByteBuffer buffer = new ByteBuffer.ByteBuffer();
                    buffer.WriteBytes(data);

                    ConnectionType Source = (ConnectionType)buffer.ReadInteger();
                    int PacketNumber = buffer.ReadInteger();

                    object[] obj;
                    switch (Source)
                    {
                        case ConnectionType.GAMESERVER:
                            if (PacketNumber == 0 || !Enum.IsDefined(typeof(GameServerProcessPacketNumbers), PacketNumber) || Network.instance.Servers[(ConnectionType)index].Socket == null)
                            {
                                return;
                            }
                            Log.log("Packet Received [#" + PacketNumber.ToString("000") + " " + ((GameServerProcessPacketNumbers)PacketNumber).ToString() + "] from Game Server, Processing response..", Log.LogType.RECEIVED);

                            obj = new object[3];
                            obj[0] = Source;
                            obj[1] = index;
                            obj[2] = data;

                            typeof(ProcessData).InvokeMember(((GameServerProcessPacketNumbers)PacketNumber).ToString(), BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static, null, null, obj);
                            break;
                        case ConnectionType.CLIENT:
                            if (PacketNumber == 0 || !Enum.IsDefined(typeof(ClientProcessPacketNumbers), PacketNumber) || Network.instance.Clients[index].Socket == null)
                            {
                                return;
                            }
                            Log.log("Packet Received [#" + PacketNumber.ToString("000") + " " + ((ClientProcessPacketNumbers)PacketNumber).ToString() + "] from Client Index " + index.ToString() + ", Processing response..", Log.LogType.RECEIVED);

                            obj = new object[3];
                            obj[0] = Source;
                            obj[1] = index;
                            obj[2] = data;

                            typeof(ProcessData).InvokeMember(((ClientProcessPacketNumbers)PacketNumber).ToString(), BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static, null, null, obj);
                            break;
                        case ConnectionType.SYNCSERVER:
                            if (PacketNumber == 0 || !Enum.IsDefined(typeof(SyncServerProcessPacketNumbers), PacketNumber) || Network.instance.Servers[(ConnectionType)index].Socket == null)
                            {
                                return;
                            }
                            Log.log("Packet Received [#" + PacketNumber.ToString("000") + " " + ((SyncServerProcessPacketNumbers)PacketNumber).ToString() + "] from Synchronization Server, Processing response..", Log.LogType.RECEIVED);

                            obj = new object[3];
                            obj[0] = Source;
                            obj[1] = index;
                            obj[2] = data;

                            typeof(ProcessData).InvokeMember(((SyncServerProcessPacketNumbers)PacketNumber).ToString(), BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static, null, null, obj);
                            break;
                        default:
                            break;

                    }
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }
            }
        }
    }
