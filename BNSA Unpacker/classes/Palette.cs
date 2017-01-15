using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSA_Unpacker.classes
{
    class Palette
    {
        public long Pointer;
        public byte[] PaletteMemory;
        public Palette(FileStream stream)
        {
            Pointer = stream.Position;
            PaletteMemory = new byte[0x20];
            stream.Read(PaletteMemory, 0, 0x20);
        }
    }
}
