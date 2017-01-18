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
        public int Index;
        public List<OAMDataListEntry> OAMDataListEntries = new List<OAMDataListEntry>();
        public Boolean IsValid = true;
        private string oamDataListsBasepath;
        private int i;

        /// <summary>
        /// Constructs a list of mini-animations from a file stream, starting with the beginning pointer table.
        /// </summary>
        /// <param name="stream">Stream to read from, starting with a pointer table to sub-mini anims.</param>
        public OAMDataList(FileStream stream)
        {
            Pointer = stream.Position;
            Console.WriteLine("----Reading OAM Data List at 0x" + Pointer.ToString("X2"));
            //while (/*stream.Position < FirstObjectEntryPointer + Pointer*/ true)
            //{

            //    //long nextPosition = stream.Position;
            //    OAMDataListEntry oamEntry = new OAMDataListEntry(stream);
            //    if (oamEntry.EndOfListEntry)
            //    {
            //        break; //This is the end of the list indicator. Don't add to list, and return.
            //    }

            //    //List Entry
            //    OAMDataListEntries.Add(oamEntry);
            //    //if (nextPosition < FirstObjectEntryPointer + Pointer)
            //    //{
            //    //    //Read the next 4 bytes in the pointer table as its a new pointer
            //    //    stream.Seek(nextPosition, SeekOrigin.Begin);
            //    //}
            //}

            int firstOAMListPointer = int.MaxValue;
            while (stream.Position < firstOAMListPointer + Pointer)
            {
                int oamDataListPointer = BNSAFile.ReadIntegerFromStream(stream);
                firstOAMListPointer = Math.Min(firstOAMListPointer, oamDataListPointer); //should only be triggered by the first pointer as it goes ascending.
                long nextPosition = stream.Position; //Address of next pointer in the list
                stream.Seek(oamDataListPointer + Pointer, SeekOrigin.Begin);
                //long nextPosition = stream.Position;
                while (true)
                {
                    OAMDataListEntry oamEntry = new OAMDataListEntry(stream);
                    if (oamEntry.EndOfListEntry)
                    {
                        break; //This is the end of the list indicator. Don't add to list, and return.
                    }

                    //List Entry
                    OAMDataListEntries.Add(oamEntry);
                }
                if (nextPosition < firstOAMListPointer + Pointer)
                {
                    //Read the next 4 bytes in the pointer table as its a new pointer
                    stream.Seek(nextPosition, SeekOrigin.Begin);
                }
            }
        }

        /// <summary>
        /// Builds an OAMDataList from a base filename path with -X being the list entry index.
        /// </summary>
        /// <param name="oamDataListsBasepath">Path to find entries from</param>
        /// <param name="i">List index to filter binary files with</param>
        public OAMDataList(string oamDataListsBasepath, int i)
        {
            string baseOAMDataEntryName = oamDataListsBasepath + i + "-";
            int nextEntryIndex = 0;
            OAMDataListEntries = new List<OAMDataListEntry>();
            while (File.Exists(baseOAMDataEntryName + nextEntryIndex + ".bin"))
            {
                //Console.WriteLine("Reading MiniFrame " + baseAnimName + nextFrameIndex + ".bin");
                OAMDataListEntry oamDataListEntry = new OAMDataListEntry(baseOAMDataEntryName + nextEntryIndex + ".bin");
                OAMDataListEntries.Add(oamDataListEntry);
                nextEntryIndex++;
            }
        }

        internal void Export(string outputDirectory, int oamDataListIndex/*, int oamDataListIndex*/)
        {
            Index = oamDataListIndex;
            int i = 0;
            foreach (OAMDataListEntry oamDataListEntry in OAMDataListEntries)
            {
                //Console.WriteLine("--Resolving Frame " + i + " references");
                oamDataListEntry.Export(outputDirectory, oamDataListIndex, i);
                i++;
            }
        }
    }
}
