using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSA_Unpacker.classes
{
    class OAMDataList
    {

        public long Pointer;
        public List<OAMDataListEntry> OAMDataListEntries = new List<OAMDataListEntry>();
        public Boolean IsValid = true;
        /// <summary>
        /// Constructs a list of mini-animations from a file stream, starting with the beginning pointer table.
        /// </summary>
        /// <param name="stream">Stream to read from, starting with a pointer table to sub-mini anims.</param>
        public OAMDataList(FileStream stream)
        {
            Pointer = stream.Position;
            Console.WriteLine("----Reading OAM Data List at 0x" + Pointer.ToString("X2"));
            while (/*stream.Position < FirstObjectEntryPointer + Pointer*/ true)
            {

                //long nextPosition = stream.Position;
                OAMDataListEntry oamEntry = new OAMDataListEntry(stream);
                if (oamEntry.EndOfListEntry)
                {
                    break; //This is the end of the list indicator. Don't add to list, and return.
                }

                //List Entry
                OAMDataListEntries.Add(oamEntry);
                //if (nextPosition < FirstObjectEntryPointer + Pointer)
                //{
                //    //Read the next 4 bytes in the pointer table as its a new pointer
                //    stream.Seek(nextPosition, SeekOrigin.Begin);
                //}
            }
        }
    }
}
