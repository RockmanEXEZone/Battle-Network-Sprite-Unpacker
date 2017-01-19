using System.IO;

namespace BNSA_Unpacker.classes
{
    class Palette
    {
        public long Pointer;
        public byte[] Memory;
        private string palettePath;

        public Palette(FileStream stream)
        {
            Pointer = stream.Position;
            Memory = new byte[0x20];
            stream.Read(Memory, 0, 0x20);
        }

        /// <summary>
        /// Constructs a palette object from a palette binary file
        /// </summary>
        /// <param name="palettePath">Path to palette file</param>
        public Palette(string palettePath)
        {
            Memory = File.ReadAllBytes(palettePath);
        }

        /// <summary>
        /// Writes this palettes's binary to disk in its own file.
        /// </summary>
        /// <param name="outputPath">Directory to put .bin into</param>
        /// <param name="index">Palette index, as part of the filename</param>
        public void Export(string outputPath, int index)
        {
            File.WriteAllBytes(outputPath + @"\palette" + index.ToString().PadLeft(2,'0') + ".bin", Memory);
        }
    }
}
