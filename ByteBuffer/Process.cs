using System;
using System.Collections.Generic;
using System.Text;

namespace ByteBuffer
{
    public partial class ByteBuffer : IDisposable
    {
        List<byte> Buff;
        byte[] readBuff;
        int readpos;
        bool buffUpdate = false;

        public ByteBuffer()
        {
            Buff = new List<byte>();
            readpos = 0;
        }

        public int GetReadPos()
        {
            return readpos;
        }

        public void SetReadPos(int newpos)
        {
            if (Buff.Count > newpos)
            {
                if (buffUpdate)
                {
                    readBuff = Buff.ToArray();
                    buffUpdate = false;
                }

                readpos = newpos;
            }
            else
            {
                throw new Exception("Byte Buffer is past its limit!");
            }
        }

        public byte[] ToArray()
        {
            return Buff.ToArray();
        }

        public int Count()
        {
            return Buff.Count;
        }

        public int Length()
        {
            return Count() - readpos;
        }

        public void Clear()
        {
            Buff.Clear();
            readpos = 0;
        }

        private bool disposedValue = false;

        //IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    Buff.Clear();
                }

                readpos = 0;
            }
            this.disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

