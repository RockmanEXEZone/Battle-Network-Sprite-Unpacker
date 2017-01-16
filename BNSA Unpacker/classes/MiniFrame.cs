using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSA_Unpacker.classes
{
    class MiniFrame
    {

        public byte ObjectListIndex;
        public byte FrameDelay;
        public byte Flags;
        public Boolean EndFrame;
        public Boolean Loops;
        public Boolean IsValid = true; //only end frames can be invalid... ish.

        /// <summary>
        /// Constructs a mini-frame object, part of a mini-animation.
        /// </summary>
        /// <param name="stream">Stream to construct a miniframe from</param>
        public MiniFrame(FileStream stream)
        {
            ObjectListIndex = (byte)stream.ReadByte();
            FrameDelay = (byte)stream.ReadByte();
            Flags = (byte)stream.ReadByte();

            //Convenience Booleans
            EndFrame = (Flags & 0x80) != 0;
            Loops = (Flags & 0x40) != 0;

            if (EndFrame)
            {
                byte endSignature = (byte) stream.ReadByte();
                endSignature &= (byte)stream.ReadByte();
                endSignature &= (byte)stream.ReadByte();
                if (endSignature != 0xFF)
                {
                    IsValid = false;
                } else
                {
                    IsValid = true;
                }
            }
        }
    }
}
