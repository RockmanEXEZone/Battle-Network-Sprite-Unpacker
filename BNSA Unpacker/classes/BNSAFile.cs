using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSA_Unpacker.classes
{
    class BNSAFile
    {
        string filepath;

        public long TilesetStartPointer;
        public long PaletteStartPointer;
        public long ProbablyPaletteStartPointer = long.MaxValue; //will always force min

        public int AnimationCount = 0;
        public int LargestTileset = 0;
        public Boolean ValidBNSA = false;
        public BNSAFile(string path)
        {
            using (FileStream bnsaStream = File.OpenRead(path))
            {
                //HEADER
                LargestTileset = bnsaStream.ReadByte();
                //Magic Number Test
                if (bnsaStream.ReadByte() != 0 || bnsaStream.ReadByte() != 1) //0x1 and 0x2 positions
                {
                    //Invalid Magic Number
                    ValidBNSA = false;
                    return;
                }
                AnimationCount = bnsaStream.ReadByte();
                Console.WriteLine("Number of Animations: " + AnimationCount);
                //Read Animation Pointers
                for (int i = 0; i < AnimationCount; i++)
                {
                    int animationPointer = ReadIntegerFromStream(bnsaStream);
                    long nextPosition = bnsaStream.Position;
                    Animation animation = new Animation(this,animationPointer, bnsaStream);
                    if (i < AnimationCount - 1)
                    {
                        bnsaStream.Seek(nextPosition, SeekOrigin.Begin); //reset position to next pointer
                    }
                }

                //Read Tilesets
                TilesetStartPointer = bnsaStream.Position;
                Console.WriteLine("Reading Tilesets, starting at 0x" + TilesetStartPointer.ToString("X6"));
                while (bnsaStream.Position < ProbablyPaletteStartPointer)
                {
                    //Read Tilesets
                    Tileset ts = new Tileset(bnsaStream);

                }

                //Read Palettes
                PaletteStartPointer = bnsaStream.Position;
                Console.WriteLine("Reading Palettes, starting at 0x" + PaletteStartPointer.ToString("X6"));

                //Starts with byte 0x20. Each palette is 32+4 bytes.

                //Read Mini-Animations and Object Lists
            }
        }

        /// <summary>
        /// Reads a 32-bit integer value from the stream, advancing it 4 bytes forward.
        /// </summary>
        /// <param name="stream">Stream to read</param>
        /// <returns>Integer</returns>
        public static int ReadIntegerFromStream(FileStream stream)
        {
            byte[] pointer = new byte[4]; //32-bit pointer
            stream.Read(pointer, 0, 4);
            return BitConverter.ToInt32(pointer, 0);
        }

        //public int findTilesetDataEnd(FileStream stream)
        //{
        //    while (true)
        //    {
        //        if (verifyValidTilesetSize(archiveFile, position) == true)
        //        {
        //            int size = BitConverter.ToInt32(archiveFile, position);
        //            position += size + 4;
        //            //if (size == 0x20)
        //            //{
        //            //    endOffset = position;
        //            //}
        //        }
        //        else
        //        {
        //            endOffset = position;
        //        }

        //    }

        //    return endOffset;
        //}
    }
}
