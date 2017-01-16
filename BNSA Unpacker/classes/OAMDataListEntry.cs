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

        public byte TileNumber;
        public sbyte XOrigin;
        public sbyte YOrigin;
        public byte Flagset1, Flagset2;

        //Convenience
        public byte ObjectWidth, ObjectHeight, PaletteIndex;
        public Boolean HorizontalFlip;
        public Boolean VerticalFlip;
        public Boolean EndOfListEntry = false;

        /// <summary>
        /// Constructs a mini-frame object, part of a mini-animation.
        /// </summary>
        /// <param name="stream">Stream to construct a miniframe from</param>
        public OAMDataListEntry(FileStream stream)
        {
            Console.Write("------Reading Object List Entry at 0x" + stream.Position.ToString("X2"));

            TileNumber = (byte)stream.ReadByte();
            XOrigin = (sbyte)stream.ReadByte();
            YOrigin = (sbyte)stream.ReadByte();
            Flagset1 = (byte)stream.ReadByte();
            Flagset2 = (byte)stream.ReadByte();
            if ((TileNumber & XOrigin & YOrigin & Flagset1 & Flagset2) == 0xFF)
            {
                EndOfListEntry = true; //this is an object list entry that marks the end of the current list
                Console.WriteLine("... End List Marker");
                return;
            } else
            {
                Console.WriteLine(); //end line
            }

            //Convenience Setup
            HorizontalFlip = (Flagset1 & 0x40) != 0; //bit 6
            VerticalFlip = (Flagset1 & 0x80) != 0; //bit 7
            byte size = (byte)(Flagset1 & 0x3); //bits 0 and 1
            byte shape = (byte)(Flagset2 & 0x3); //bits 0 and 1

            PaletteIndex = (byte) (Flagset2 >> 4); //bits 4-7

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
    }
}
