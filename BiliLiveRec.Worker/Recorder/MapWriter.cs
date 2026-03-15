using System;
using System.Collections.Generic;
using System.Text;

namespace BiliLiveRec.Worker.Recorder
{
    internal class MapWriter
    {
        private BinaryWriter _bw;
        public MapWriter(string path)
        {
            _bw = new(new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read));
            _bw.Write("BLRMAPFF"u8);
        }
        public void Write(string text,long offset,long length)
        {
            _bw.Write(offset);
            _bw.Write(length);
            Span<byte> tmp = Encoding.UTF8.GetBytes(text);
            _bw.Write(tmp.Length);
            _bw.Write(tmp);
        }
        public void Flush()
        {
            _bw.Flush();
        }
        public void Close()
        {
            _bw.Flush();
            _bw.Dispose();
        }
    }
}
