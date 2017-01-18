using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BNSA_Unpacker.classes
{
    class Frame
    {
        public int Index;
        public byte[] Memory;

        //Used when parsing a BNSA file
        public int TilesetPointer;
        public int PalettePointer; //don't think this is useful really.
        public int MiniAnimationPointer;
        public int OAMDataListPointer;

        //Used when parsing a BNSA XML file
        public int TilesetIndex;
        public int MiniAnimationIndex;
        public int OAMDataListIndex;

        //Convenience
        public byte FrameDelay;
        public byte Flags;
        public Boolean EndFrame = false;
        public Boolean Loops = false;

        // Linked objects
        public long Pointer;
        public Tileset ResolvedTileset;
        public MiniAnimGroup ResolvedMiniAnimGroup;
        public OAMDataList ResolvedOAMDataList;

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
            OAMDataListPointer = BNSAFile.ReadIntegerFromStream(stream) + 0x4;
            FrameDelay = (byte)stream.ReadByte();
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
        /// Generates a frame object from a XML node
        /// </summary>
        /// <param name="node">XML Node to create a frame object from</param>
        public Frame(int animationIndex, XmlNode node)
        {
            Index = Int32.Parse(node.Attributes["index"].Value);
            TilesetIndex = Int32.Parse(node.SelectSingleNode(BNSAFile.TilesetXMLNodeName).InnerText);
            OAMDataListIndex = Int32.Parse(node.SelectSingleNode(BNSAFile.OAMDataListXMLNodeName).InnerText);
            MiniAnimationIndex = Int32.Parse(node.SelectSingleNode(BNSAFile.MiniAnimationXMLNodeName).InnerText);

            Console.WriteLine("Read Index " + Index);
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
            foreach (OAMDataList oamDataList in parsedBNSA.OAMDataLists)
            {
                if (oamDataList.Pointer == OAMDataListPointer)
                {
                    ResolvedOAMDataList = oamDataList;
                    break;
                }
            }
            foreach (MiniAnimGroup minianimgroup in parsedBNSA.MiniAnimGroups)
            {
                if (minianimgroup.Pointer == MiniAnimationPointer)
                {
                    ResolvedMiniAnimGroup = minianimgroup;
                    ResolvedMiniAnimGroup.ResolveReferences(parsedBNSA,this);
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
            }
            else
            {
                Console.WriteLine("----/!\\ Failed to Resolve Tileset Pointer 0x"+TilesetPointer.ToString("X2"));
            }

            if (ResolvedMiniAnimGroup != null)
            {
                Console.WriteLine("----Resolved MiniAnim Reference");
            }
            else
            {
                Console.WriteLine("----/!\\ Failed to Resolve MiniAnim  0x" + MiniAnimationPointer.ToString("X2"));
            }

            if (ResolvedOAMDataList != null)
            {
                Console.WriteLine("----Resolved OAM Data List Reference");
            }
            else
            {
                Console.WriteLine("----/!\\ Failed to Resolve OAM Data List 0x" + OAMDataListPointer.ToString("X2"));
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

        internal XmlNode GenerateNode(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.CreateElement("frame");
            XmlAttribute attribute = xmlDoc.CreateAttribute("index");
            attribute.Value = Index.ToString();
            node.Attributes.Append(attribute);

            //subnodes
            XmlNode tilesetnode = xmlDoc.CreateElement(BNSAFile.TilesetXMLNodeName);
            tilesetnode.InnerText = ResolvedTileset.Index.ToString();
            XmlNode oamlistindexnode = xmlDoc.CreateElement(BNSAFile.OAMDataListXMLNodeName);
            oamlistindexnode.InnerText = ResolvedOAMDataList.Index.ToString();
            XmlNode minianimgroupnode = xmlDoc.CreateElement(BNSAFile.MiniAnimationXMLNodeName);
            minianimgroupnode.InnerText = ResolvedMiniAnimGroup.Index.ToString();

            node.AppendChild(tilesetnode);
            node.AppendChild(oamlistindexnode);
            node.AppendChild(minianimgroupnode);

            //convenience stuff (This is not read back in... most likely)
            XmlNode framedelaynode = xmlDoc.CreateElement("convenience-framedelay");
            framedelaynode.InnerText = FrameDelay.ToString();
            node.AppendChild(framedelaynode);

            if (EndFrame)
            {
                XmlNode endnode = xmlDoc.CreateElement("convenience-loops");
                endnode.InnerText = Loops.ToString();
                node.AppendChild(endnode);
            }
            return node;
        }

        internal void Export(string outputPath, int animationIndex, int frameIndex)
        {
            this.Index = frameIndex;
            File.WriteAllBytes(outputPath + @"\frame" + animationIndex + "-" + frameIndex + ".bin", Memory);
        }
    }
}