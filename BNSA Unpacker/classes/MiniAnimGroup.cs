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
        public int Index; //For XML
        public List<MiniAnim> MiniAnimations = new List<MiniAnim>();
        public Boolean IsValid = true;
        /// <summary>
        /// Constructs a list of mini-animations from a file stream, starting with the beginning pointer table.
        /// </summary>
        /// <param name="stream">Stream to read from, starting with a pointer table to sub-mini anims.</param>
        public MiniAnimGroup(FileStream stream)
        {
            Pointer = stream.Position;
            int firstAnimPointer = int.MaxValue;
            while (stream.Position < firstAnimPointer + Pointer)
            {
                int animationPointer = BNSAFile.ReadIntegerFromStream(stream);
                firstAnimPointer = Math.Min(firstAnimPointer, animationPointer); //should only be triggered by the first pointer as it goes ascending.
                long nextPosition = stream.Position;
                stream.Seek(Pointer + animationPointer, SeekOrigin.Begin); //Move cursor to start of minianim
                MiniAnim animation = new MiniAnim(stream, Pointer);
                IsValid &= animation.IsValid;
                if (!IsValid)
                {
                    break; //Invalid data, stop caring and exit.
                }

                MiniAnimations.Add(animation);
                if (nextPosition < firstAnimPointer + Pointer)
                {
                    //Read the next 4 bytes in the pointer table as its a new pointer
                    stream.Seek(nextPosition, SeekOrigin.Begin);
                }
            }
        }

        internal void Export(string outputDirectory, int miniAnimGroupIndex)
        {
            this.Index = miniAnimGroupIndex;

            int i = 0;
            foreach (MiniAnim minianim in MiniAnimations)
            {
                //Console.WriteLine("--Resolving Frame " + i + " references");
                minianim.Export(outputDirectory, miniAnimGroupIndex, i);
                i++;
            }
        }

        internal void ResolveReferences(BNSAFile parsedBNSA, Frame owningFrame)
        {
            int i = 0;
            foreach (MiniAnim miniAnim in MiniAnimations)
            {
                //Console.WriteLine("--Resolving Frame " + i + " references");
                miniAnim.ResolveReferences(parsedBNSA, owningFrame);
                i++;
            }
        }
    }
}
