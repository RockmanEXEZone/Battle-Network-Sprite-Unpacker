using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSA_Unpacker.classes
{
    class Frame
    {
        public int TilesetPointer;
        public int PalettePointer;
        public int MiniAnimationPointer;
        public int ObjectListPointer;
        public byte FrameDelay;
        public byte Flags;
        public Boolean EndFrame = false;
        public Boolean Loops = false;

        /// <summary>
        /// Creates a new frame from the next 0x20 bytes of the given stream.
        /// </summary>
        /// <param name="stream">Stream to read a frame from</param>
        public Frame(FileStream stream)
        {
            TilesetPointer = BNSAFile.ReadIntegerFromStream(stream);
            PalettePointer = BNSAFile.ReadIntegerFromStream(stream);
            MiniAnimationPointer = BNSAFile.ReadIntegerFromStream(stream);
            ObjectListPointer = BNSAFile.ReadIntegerFromStream(stream);
            FrameDelay = (byte) stream.ReadByte();
            stream.ReadByte(); //constant 00?
            Flags = (byte)stream.ReadByte();
            stream.ReadByte(); //constant 00 ?

            //Convenience Booleans
            EndFrame = (Flags & 0x80) != 0;
            Loops = (Flags & 0x40) != 0;

        }
    }
}