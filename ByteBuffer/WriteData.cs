using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ByteBuffer
{
    public partial class ByteBuffer
    {
        public void WriteByte(byte Input)
        {
            Contents.Add(Input);
            Buff.Add(Input);
            buffUpdate = true;
        }

        public void WriteBytes(byte[] Input)
        {
            Contents.Add(Input);
            Buff.AddRange(Input);
            buffUpdate = true;
        }

        public void WriteShort(short Input)
        {
            Contents.Add(Input);
            Buff.AddRange(BitConverter.GetBytes(Input));
            buffUpdate = true;
        }

        public void WriteInteger(int Input)
        {
            Contents.Add(Input);
            Buff.AddRange(BitConverter.GetBytes(Input));
            buffUpdate = true;
        }

        public void WriteFloat(float Input)
        {
            Contents.Add(Input);
            Buff.AddRange(BitConverter.GetBytes(Input));
            buffUpdate = true;
        }

        public void WriteString(string Input)
        {
            Contents.Add(Input);
            Buff.AddRange(BitConverter.GetBytes(Input.Length));
            Buff.AddRange(Encoding.ASCII.GetBytes(Input));
            buffUpdate = true;
        }
    }
}
