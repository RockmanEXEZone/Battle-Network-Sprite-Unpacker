using System;
using System.IO;

namespace BNSA_Unpacker.classes
{
    class Tileset
    {
        public long Pointer;
        public int Index;
        public int BitmapSize;
        public byte[] BitmapData;
        public byte[] Memory;

        public Tileset(FileStream stream, int index)
        {
            Pointer = stream.Position;
            Index = index;
            Console.WriteLine("Reading Tileset at 0x" + Pointer.ToString("X2"));
            BitmapSize = BNSAFile.ReadIntegerFromStream(stream);
            BitmapData = new byte[BitmapSize];
            stream.Read(BitmapData, 0, BitmapSize);

            //Copy Export Memory
            stream.Seek(Pointer, SeekOrigin.Begin);
            Memory = new byte[BitmapSize + 4];
            stream.Read(Memory, 0, BitmapSize + 4);
        }

        /// <summary>
        /// Reads the memory of a tileset into this object
        /// </summary>
        /// <param name="tilesetPath">Path to tileset bitmap data</param>
        /// <param name="index">Tileset Index</param>
        public Tileset(string tilesetPath, int index)
        {
            this.Index = index;
            BitmapData = File.ReadAllBytes(tilesetPath);
            BitmapSize = BitmapData.Length;
        }

        /// <summary>
        /// Writes this tileset's binary to disk in its own file. Does not include the bitmap data size header.
        /// </summary>
        /// <param name="outputPath">Directory to put .bin into</param>
        /// <param name="index">Tileset index, as part of the filename</param>
        public void Export(string outputPath, int index)
        {
            File.WriteAllBytes(outputPath + @"\tileset" + index.ToString().PadLeft(3, '0') + ".bin", BitmapData);
        }
    }
}
