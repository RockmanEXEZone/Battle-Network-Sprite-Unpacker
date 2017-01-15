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
            Console.WriteLine("Reading Tileset at 0x" + Pointer.ToString("X6"));
            BitmapSize = BNSAFile.ReadIntegerFromStream(stream);
            BitmapData = new byte[BitmapSize];
            stream.Read(BitmapData, 0, BitmapSize);
        }

        private bool verifyValidTilesetSize(byte[] archiveFile, int offset)
        {
            uint size = BitConverter.ToUInt32(archiveFile, offset);
            if (size % 0x20 != 0)
            {
                return false;
            }
            if (size == 0x20)
            {
                //var 


                //int guessPalette0 = usedPalettes.Min();
                //if (guessPalette0 - offset >= 0 && guessPalette0 - offset <= 0x04)
                //{
                //    uint test = BitConverter.ToUInt32(archiveFile, (int)(offset + size));
                //    if (test >= 0x00010000)
                //    {
                //        return false;
                //    }
                //}

            }
            return true;
        }
    }
}
