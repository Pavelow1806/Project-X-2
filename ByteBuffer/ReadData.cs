using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ByteBuffer
{
    public partial class ByteBuffer
    {
        public string ReadString(bool peek = true)
        {
            int Len = ReadInteger(true);
            if (buffUpdate)
            {
                readBuff = Buff.ToArray();
                buffUpdate = false;
            }

            string ret = Encoding.ASCII.GetString(readBuff, readpos, Len);
            if (peek & Buff.Count > readpos)
            {
                if (ret.Length > 0)
                {
                    readpos += Len;
                }
            }
            return ret;
        }

        public byte ReadByte(bool peek = true)
        {
            if (Buff.Count > readpos)
            {
                if (buffUpdate)
                {
                    readBuff = Buff.ToArray();
                    buffUpdate = false;
                }

                byte ret = readBuff[readpos];
                if (peek & Buff.Count > readpos)
                {
                    readpos += 1;
                }
                return ret;
            }
            else
            {
                throw new Exception("Byte Buffer is past its limit!");
            }
        }

        public byte[] ReadBytes(int length, bool peek = true)
        {
            if (buffUpdate)
            {
                readBuff = Buff.ToArray();
                buffUpdate = false;
            }

            byte[] ret = Buff.GetRange(readpos, length).ToArray();
            if (peek)
            {
                readpos += length;
            }
            return ret;
        }

        public float ReadFloat(bool peek = true)
        {
            if (Buff.Count > readpos)
            {
                if (buffUpdate)
                {
                    readBuff = Buff.ToArray();
                    buffUpdate = false;
                }

                float ret = BitConverter.ToSingle(readBuff, readpos);
                if (peek & Buff.Count > readpos)
                {
                    readpos += 4;
                }
                return ret;
            }
            else
            {
                throw new Exception("Byte Buffer is past its limit!");
            }
        }

        public int ReadInteger(bool peek = true)
        {
            if (Buff.Count > readpos)
            {
                if (buffUpdate)
                {
                    readBuff = Buff.ToArray();
                    buffUpdate = false;
                }

                int ret = BitConverter.ToInt32(readBuff, readpos);
                if (peek & Buff.Count > readpos)
                {
                    readpos += 4;
                }
                return ret;
            }
            else
            {
                throw new Exception("Byte Buffer is past its limit!");
            }
        }
    }
}
