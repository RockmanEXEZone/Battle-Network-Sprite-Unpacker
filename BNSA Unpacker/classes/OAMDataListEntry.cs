using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSA_Unpacker.classes
{
    class OAMDataListEntry
    {

        public byte[] Memory;
        public long Pointer;
        public byte TileNumber;
        public sbyte XOrigin;
        public sbyte YOrigin;
        public byte Flagset1, Flagset2;

        //Convenience
        public byte ObjectWidth, ObjectHeight, PaletteIndex;
        public Boolean HorizontalFlip;
        public Boolean VerticalFlip;
        public Boolean EndOfListEntry = false;
        private string v;

        /// <summary>
        /// Constructs an OAM Data List Entry from the 5-byte binary in a BNSA file
        /// </summary>
        /// <param name="stream">Stream to construct a OAM Data Entry from</param>
        public OAMDataListEntry(FileStream stream)
        {
            Console.Write("------Reading OAM Data List Entry at 0x" + stream.Position.ToString("X2"));
            Pointer = stream.Position;

            TileNumber = (byte)stream.ReadByte();
            XOrigin = (sbyte)stream.ReadByte();
            YOrigin = (sbyte)stream.ReadByte();
            Flagset1 = (byte)stream.ReadByte();
            Flagset2 = (byte)stream.ReadByte();

            //Copy Memory for Export
            stream.Seek(Pointer, SeekOrigin.Begin);
            Memory = new byte[5];
            stream.Read(Memory, 0, 5);

            if ((TileNumber & XOrigin & YOrigin & Flagset1 & Flagset2) == 0xFF)
            {
                EndOfListEntry = true; //this is an object list entry that marks the end of the current list
                Console.WriteLine("... End List Marker");
                return;
            }
            else
            {
                Console.WriteLine(); //end line
            }

            //Convenience Setup
            HorizontalFlip = (Flagset1 & 0x40) != 0; //bit 6
            VerticalFlip = (Flagset1 & 0x80) != 0; //bit 7
            byte size = (byte)(Flagset1 & 0x3); //bits 0 and 1
            byte shape = (byte)(Flagset2 & 0x3); //bits 0 and 1

            PaletteIndex = (byte)(Flagset2 >> 4); //bits 4-7

            //Pack bytes for size switch statement.
            byte sizeShapeMatrix = (byte)(size << 0x4); //e.g. 30
            sizeShapeMatrix |= shape; //e.g. 31

            //Could probably be done better, but idk how
            switch (sizeShapeMatrix)
            {
                case 0x00:
                    ObjectWidth = 8;
                    ObjectHeight = 8;
                    break;
                case 0x01:
                    ObjectWidth = 16;
                    ObjectHeight = 8;
                    break;
                case 0x02:
                    ObjectWidth = 8;
                    ObjectHeight = 16;
                    break;

                case 0x10:
                    ObjectWidth = 16;
                    ObjectHeight = 16;
                    break;
                case 0x11:
                    ObjectWidth = 32;
                    ObjectHeight = 8;
                    break;
                case 0x12:
                    ObjectWidth = 8;
                    ObjectHeight = 32;
                    break;

                case 0x20:
                    ObjectWidth = 32;
                    ObjectHeight = 32;
                    break;
                case 0x21:
                    ObjectWidth = 32;
                    ObjectHeight = 16;
                    break;
                case 0x22:
                    ObjectWidth = 16;
                    ObjectHeight = 32;
                    break;

                case 0x30:
                    ObjectWidth = 64;
                    ObjectHeight = 64;
                    break;
                case 0x31:
                    ObjectWidth = 64;
                    ObjectHeight = 32;
                    break;
                case 0x32:
                    ObjectWidth = 32;
                    ObjectHeight = 64;
                    break;
            }

        }

        /// <summary>
        /// Constructs an OAM Data List Entry from a binary file
        /// </summary>
        /// <param name="filePath">Binary File that represents the memory of this entry</param>
        public OAMDataListEntry(string filePath)
        {
            Memory = File.ReadAllBytes(filePath);
        }

        /// <summary>
        /// Writes the binary data for this entry to a file, named oamdatalistGROUP-SUBINDEX-INDEX.bin
        /// </summary>
        /// <param name="outputDirectory">Directory to output to</param>
        /// <param name="oamDataListGroupIndex">Group Index this oam list belongs to</param>
        /// <param name="oamDataListIndex">List this entry belongs to</param>
        /// <param name="entryIndex">Index of this entry in the list</param>
        internal void Export(string outputDirectory, int oamDataListGroupIndex, int oamDataListIndex, int entryIndex)
        {
            File.WriteAllBytes(outputDirectory + @"\oamdatalist" + oamDataListGroupIndex + "-" + oamDataListIndex + "-" + entryIndex + ".bin", Memory);
        }
    }
}
