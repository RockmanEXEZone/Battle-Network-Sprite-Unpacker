using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSA_Unpacker.classes
{
    class MiniAnimGroup
    {
        public long Pointer;
        public List<MiniAnim> MiniAnimations = new List<MiniAnim>();
        public Boolean IsValid = true;
        /// <summary>
        /// Constructs a list of mini-animations from a file stream, starting with the beginning pointer table.
        /// </summary>
        /// <param name="stream">Stream to read from, starting with a pointer table to sub-mini anims.</param>
        public MiniAnimGroup(FileStream stream)
        {
            Pointer = stream.Position;
            int FirstAnimPointer = int.MaxValue;
            while (stream.Position < FirstAnimPointer + Pointer)
            {
                int animationPointer = BNSAFile.ReadIntegerFromStream(stream);
                FirstAnimPointer = Math.Min(FirstAnimPointer, animationPointer); //should only be triggered by the first pointer as it goes ascending.

                long nextPosition = stream.Position;
                MiniAnim animation = new MiniAnim(stream, Pointer);
                IsValid &= animation.IsValid;
                if (!IsValid)
                {
                    break; //Invalid data, stop caring and exit.
                }

                MiniAnimations.Add(animation);
                if (nextPosition < FirstAnimPointer + Pointer)
                {
                    //Read the next 4 bytes in the pointer table as its a new pointer
                    stream.Seek(nextPosition, SeekOrigin.Begin);
                }
            }
        }
    }
}
