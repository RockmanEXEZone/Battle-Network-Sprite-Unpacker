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
        public byte[] Memory;
        public Palette(FileStream stream)
        {
            Pointer = stream.Position;
            Memory = new byte[0x20];
            stream.Read(Memory, 0, 0x20);
        }

        /// <summary>
        /// Writes this palettes's binary to disk in its own file.
        /// </summary>
        /// <param name="outputPath">Directory to put .bin into</param>
        /// <param name="index">Palette index, as part of the filename</param>
        public void Export(string outputPath, int index)
        {
            File.WriteAllBytes(outputPath + @"\palette" + index + ".bin", Memory);
        }
    }
}
