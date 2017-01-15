using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSA_Unpacker.classes
{
    class Animation
    {
        private int Pointer;
        /// <summary>
        /// Animation Object, created from a pointer in the BNSA archive. The passed in stream will be used to load data. The stream position will be modified, so save it before you call this.
        /// </summary>
        /// <param name="bnsa">BNSA Object. Will be used so the probably palette pointer can be updated.</param>
        /// <param name="ptr">Pointer (from start of file) to the animation data.</param>
        /// <param name="stream">File stream to read read from</param>
        public Animation(BNSAFile bnsa, int ptr, FileStream stream)
        {
            this.Pointer = ptr;
            this.Pointer += 4; //Skip Header
            Console.WriteLine("Animation Pointer 0x" + this.Pointer.ToString("X8")); //"X8" = 8 byte hex output
            stream.Seek(this.Pointer, SeekOrigin.Begin);
            List<Frame> frames = new List<Frame>();
            int frameindex = 0;
            while (true)
            {
                //Read Frame Until Stop or Loop at Pos + 0x12
                Console.WriteLine("Reading Frame " + frameindex+" at 0x"+stream.Position.ToString("X6"));
                Frame frame = new Frame(stream);
                bnsa.ProbablyPaletteStartPointer = Math.Min(bnsa.ProbablyPaletteStartPointer, frame.PalettePointer); //Find first Palette Pointer
                frames.Add(frame);
                if (frame.EndFrame) //End of animation indicator
                {
                    break;
                }
                frameindex++;
            }
        }
    }
}
