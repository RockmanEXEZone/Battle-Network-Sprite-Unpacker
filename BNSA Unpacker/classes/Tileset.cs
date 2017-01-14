using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSA_Unpacker.classes
{
    class Tileset
    {
        public long Pointer;
        public int BitmapSize;
        public byte[] BitmapData;
        
        public Tileset(FileStream stream)
        {
            Pointer = stream.Position;
            BitmapSize = BNSAFile.ReadIntegerFromStream(stream);
            BitmapData = new byte[BitmapSize];
            stream.Read(BitmapData, 0, BitmapSize);
        }
    }
}
