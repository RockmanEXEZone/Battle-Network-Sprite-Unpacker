using System;
using System.Collections.Generic;
using System.IO;

namespace BNSA_Unpacker.classes
{
    public class OAMDataListGroup
    {
        public long Pointer;
        public int Index;
        public List<OAMDataList> OAMDataLists = new List<OAMDataList>();

        public OAMDataListGroup(Stream stream, int index)
        {
            Index = index;
            Pointer = stream.Position;
            Console.WriteLine("--Reading OAM Data List Pointer Table (Group) at 0x" + stream.Position.ToString("X2"));

            int firstOAMEntryPointer = int.MaxValue;

            while (stream.Position < firstOAMEntryPointer + Pointer)
            {
                int oamDataEntryPointer = BNSAFile.ReadIntegerFromStream(stream);
                firstOAMEntryPointer = Math.Min(firstOAMEntryPointer, oamDataEntryPointer); //should only be triggered by the first pointer as it goes ascending.
                long nextPosition = stream.Position; //Address of next pointer in the list
                stream.Seek(oamDataEntryPointer + Pointer, SeekOrigin.Begin);
                OAMDataList oamDataList = new OAMDataList(stream);
                OAMDataLists.Add(oamDataList);
                if (nextPosition < firstOAMEntryPointer + Pointer)
                {
                    //Read the next 4 bytes in the pointer table as its a new pointer
                    stream.Seek(nextPosition, SeekOrigin.Begin);
                }
            }
        }

        /// <summary>
        /// Constructs an OAM Data List Group from a group index.
        /// </summary>
        /// <param name="oamDataListsBasepath">Base path for the file</param>
        /// <param name="groupIndex">First integer in filename</param>
        public OAMDataListGroup(string oamDataListsBasepath, int groupIndex)
        {
            Index = groupIndex;
            string baseOAMDataEntryName = oamDataListsBasepath + groupIndex + "-";
            int nextListIndex = 0;
            OAMDataLists = new List<OAMDataList>();
            while (File.Exists(baseOAMDataEntryName + nextListIndex + "-0.bin")) //group-list-firstentry.bin
            {
                //Console.WriteLine("Reading MiniFrame " + baseAnimName + nextFrameIndex + ".bin");
                OAMDataList oamDataListEntry = new OAMDataList(baseOAMDataEntryName, groupIndex, nextListIndex);
                OAMDataLists.Add(oamDataListEntry);
                nextListIndex++;
            }
        }

        internal void Export(string outputDirectory, int index)
        {
            this.Index = index;
            int i = 0;
            foreach (OAMDataList oamDataList in OAMDataLists)
            {
                //Console.WriteLine("--Resolving Frame " + i + " references");
                oamDataList.Export(outputDirectory, index, i);
                i++;
            }
        }
    }
}
