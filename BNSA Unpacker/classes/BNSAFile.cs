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
        public byte[] Memory;
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
                    Animation animation = new Animation(animationPointer, bnsaStream);
                    bnsaStream.Seek(nextPosition, SeekOrigin.Begin); //reset position to next pointer
                }
                //
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
    }
}
