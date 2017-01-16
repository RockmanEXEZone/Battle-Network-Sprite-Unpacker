using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSA_Unpacker.classes
{
    class Frame
    {
        public long Pointer;
        public int TilesetPointer;
        public int PalettePointer;
        public int MiniAnimationPointer;
        public int ObjectListPointer;
        public byte FrameDelay;
        public byte Flags;
        public Boolean EndFrame = false;
        public Boolean Loops = false;
        public byte[] Memory;


        //public Palette ResolvedPalette; //Seems to always point to start of palette blocks (0x20)??
        public Tileset ResolvedTileset;
        public MiniAnimGroup ResolvedMiniAnimGroup;
        public OAMDataListGroup ResolvedObjectListGroup;

        /// <summary>
        /// Creates a new frame from the next 0x20 bytes of the given stream.
        /// </summary>
        /// <param name="stream">Stream to read a frame from</param>
        public Frame(FileStream stream)
        {
            Pointer = stream.Position;
            TilesetPointer = BNSAFile.ReadIntegerFromStream(stream) + 0x4;
            PalettePointer = BNSAFile.ReadIntegerFromStream(stream) + 0x4;
            MiniAnimationPointer = BNSAFile.ReadIntegerFromStream(stream) + 0x4;
            ObjectListPointer = BNSAFile.ReadIntegerFromStream(stream) + 0x4;
            FrameDelay = (byte) stream.ReadByte();
            stream.ReadByte(); //constant 00?
            Flags = (byte)stream.ReadByte();
            stream.ReadByte(); //constant 00 ?

            //Convenience Booleans
            EndFrame = (Flags & 0x80) != 0;
            Loops = (Flags & 0x40) != 0;

            //Copy Memory for Export
            stream.Seek(Pointer, SeekOrigin.Begin);
            Memory = new byte[20];
            stream.Read(Memory, 0, 20);

        }

        /// <summary>
        /// Converts pointers to references to other parsed BNSA objects. This allows us to export references rather than hard coded values for easy recompilation and editing of the outputted XML.
        /// </summary>
        /// <param name="parsedBNSA">BNSA File that has been parsed</param>
        public void ResolveReferences(BNSAFile parsedBNSA)
        {
            //foreach (Palette palette in parsedBNSA.Palettes)
            //{
            //    if (palette.Pointer == PalettePointer)
            //    {
            //        ResolvedPalette = palette;
            //        break;
            //    }
            //}

            foreach (Tileset tileset in parsedBNSA.Tilesets)
            {
                if (tileset.Pointer == TilesetPointer)
                {
                    ResolvedTileset = tileset;
                    break;
                }
            }

            foreach (MiniAnimGroup minianimgroup in parsedBNSA.MiniAnimGroups)
            {
                if (minianimgroup.Pointer == MiniAnimationPointer)
                {
                    ResolvedMiniAnimGroup = minianimgroup;
                    break;
                }
            }

            foreach (OAMDataListGroup oamDataListGroup in parsedBNSA.OAMDataListGroups)
            {
                if (oamDataListGroup.Pointer == ObjectListPointer)
                {
                    ResolvedObjectListGroup = oamDataListGroup;
                    break;
                }
            }

            //if (ResolvedPalette != null)
            //{
            //    Console.WriteLine("----Resolved Palette Reference");
            //}
            //else
            //{
            //    Console.WriteLine("----/!\\ Failed to Resolve Palette: 0x"+PalettePointer.ToString("X2"));
            //}

            if (ResolvedTileset != null)
            {
                Console.WriteLine("----Resolved Tileset Reference");
            } else
            {
                Console.WriteLine("----/!\\ Failed to Resolve Tileset");
            }

            if (ResolvedMiniAnimGroup != null)
            {
                Console.WriteLine("----Resolved MiniAnim Reference");
            }
            else
            {
                Console.WriteLine("----/!\\ Failed to Resolve MiniAnim");
            }

            if (ResolvedObjectListGroup != null)
            {
                Console.WriteLine("----Resolved OAM Data List Reference");
            }
            else
            {
                Console.WriteLine("----/!\\ Failed to Resolve OAM Data List");
            }



            //foreach (MiniAnim tileset in parsedBNSA.MiniAnimGroups)
            //{
            //    if (minianim.Pointer == TilesetPointer)
            //    {
            //        ResolvedMiniAnim = tileset;
            //        break;
            //    }
            //}
        }

        internal void Export(string outputPath, int animationIndex, int frameIndex)
        {
            File.WriteAllBytes(outputPath + @"\frame" + animationIndex + "-"+frameIndex+".bin", Memory);
        }
    }
}