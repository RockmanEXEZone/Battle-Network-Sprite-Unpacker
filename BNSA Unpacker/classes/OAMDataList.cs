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

        /// <summary>
        /// Constructs a list of mini-animations from a file stream, starting with the beginning pointer table.
        /// </summary>
        /// <param name="stream">Stream to read from, starting with a pointer table to sub-mini anims.</param>
        public OAMDataList(FileStream stream)
        {
            Pointer = stream.Position;
            Console.WriteLine("----Reading OAM Data List at 0x" + Pointer.ToString("X2"));
            {
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
            }
        }

        /// <summary>
        /// Builds an OAMDataList from a base filename path with -X being the list entry index.
        /// </summary>
        /// <param name="oamDataListsBasepath">Path to find entries from</param>
        /// <param name="groupIndex">List index to filter binary files with</param>
        public OAMDataList(string oamDataListsBasepath, int groupIndex, int listIndex)
        {
            string baseOAMDataEntryName = oamDataListsBasepath + listIndex + "-";
            int nextEntryIndex = 0;
            OAMDataListEntries = new List<OAMDataListEntry>();
            while (File.Exists(baseOAMDataEntryName + nextEntryIndex+".bin")) //group-list-firstentry.bin
            {
                //Console.WriteLine("Reading MiniFrame " + baseAnimName + nextFrameIndex + ".bin");
                OAMDataListEntry oamDataListEntry = new OAMDataListEntry(baseOAMDataEntryName + nextEntryIndex + ".bin", nextEntryIndex);
                OAMDataListEntries.Add(oamDataListEntry);
                nextEntryIndex++;
            }
        }

        internal void Export(string outputDirectory, int oamDataGroupIndex, int oamDataListIndex)
        {
            Index = oamDataListIndex;
            int i = 0;
            foreach (OAMDataListEntry oamDataListEntry in OAMDataListEntries)
            {
                //Console.WriteLine("--Resolving Frame " + i + " references");
                oamDataListEntry.Export(outputDirectory, oamDataGroupIndex, oamDataListIndex, i);
                i++;
            }
        }
    }
}
