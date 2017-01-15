using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSA_Unpacker.classes
{
    class MiniAnim
    {
        public long Pointer;
        public byte ObjectListIndex;
        public byte Delay;
        public byte Flags;

        /// <summary>
        /// Constructs a MiniAnim object from the current position of the stream. Will create sub-mini frames.
        /// </summary>
        /// <param name="stream">Stream to construct minianim from.</param>
        public MiniAnim(FileStream stream)
        {

        }

    }
}
