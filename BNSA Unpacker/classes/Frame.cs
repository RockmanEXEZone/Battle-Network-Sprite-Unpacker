using System;
using System.IO;
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
        public OAMDataListGroup ResolvedOAMDataListGroup;

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
            FrameDelay = Byte.Parse(node.SelectSingleNode(BNSAFile.FrameDelayXMLNodeName).InnerText);

            EndFrame = false;
            Loops = false;
            XmlNode loopNode = node.SelectSingleNode(BNSAFile.LoopsXMLNodeName);
            if (loopNode != null)
            {
                EndFrame = true;
                Loops = Boolean.Parse(loopNode.InnerText);
            }

            Flags = 0;
            XmlNode flagsNode = node.SelectSingleNode(BNSAFile.FlagsXMLNodeName);
            if (flagsNode != null)
            {
                Flags = Byte.Parse(flagsNode.InnerText);
            }

            if (EndFrame)
            {
                Flags |= 0x80;
                if (Loops)
                {
                    Flags |= 0x40;
                }
            }

            Console.WriteLine("Read Index " + Index);
        }

        /// <summary>
        /// Converts pointers to references to other parsed BNSA objects. This allows us to export references rather than hard coded values for easy recompilation and editing of the outputted XML.
        /// </summary>
        /// <param name="parsedBNSA">BNSA XML File that has been parsed</param>
        public void ResolveReferences(BNSAXMLFile parsedBNSA)
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
                if (tileset.Index == TilesetIndex)
                {
                    ResolvedTileset = tileset;
                    break;
                }
            }
            foreach (OAMDataListGroup oamDataListGroup in parsedBNSA.OAMDataListGroups)
            {
                if (oamDataListGroup.Index == OAMDataListIndex)
                {
                    ResolvedOAMDataListGroup = oamDataListGroup;
                    break;
                }
            }
            foreach (MiniAnimGroup minianimgroup in parsedBNSA.MiniAnimationGroups)
            {
                if (minianimgroup.Index == MiniAnimationIndex)
                {
                    ResolvedMiniAnimGroup = minianimgroup;
                    ResolvedMiniAnimGroup.ResolveReferences(parsedBNSA, this);
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
                Console.WriteLine("----/!\\ Failed to Resolve Tileset Pointer 0x" + TilesetPointer.ToString("X2"));
            }

            if (ResolvedMiniAnimGroup != null)
            {
                Console.WriteLine("----Resolved MiniAnim Reference");
            }
            else
            {
                Console.WriteLine("----/!\\ Failed to Resolve MiniAnim 0x" + MiniAnimationPointer.ToString("X2"));
            }

            if (ResolvedOAMDataListGroup != null)
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
            foreach (OAMDataListGroup oamDataList in parsedBNSA.OAMDataListGroups)
            {
                if (oamDataList.Pointer == OAMDataListPointer)
                {
                    ResolvedOAMDataListGroup = oamDataList;
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
                Console.WriteLine("----/!\\ Failed to Resolve MiniAnim 0x" + MiniAnimationPointer.ToString("X2"));
            }

            if (ResolvedOAMDataListGroup != null)
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
            oamlistindexnode.InnerText = ResolvedOAMDataListGroup.Index.ToString();
            XmlNode minianimgroupnode = xmlDoc.CreateElement(BNSAFile.MiniAnimationXMLNodeName);
            minianimgroupnode.InnerText = ResolvedMiniAnimGroup.Index.ToString();
            XmlNode framedelaynode = xmlDoc.CreateElement(BNSAFile.FrameDelayXMLNodeName);
            framedelaynode.InnerText = FrameDelay.ToString();
        
            XmlNode flagsnode = xmlDoc.CreateElement(BNSAFile.FlagsXMLNodeName);
            flagsnode.InnerText = Flags.ToString();

            node.AppendChild(tilesetnode);
            node.AppendChild(oamlistindexnode);
            node.AppendChild(minianimgroupnode);
            node.AppendChild(framedelaynode);
            if ((Flags&0x3F) != 0)
            {
                //Unusual flags
                node.AppendChild(flagsnode);
            }

            if (EndFrame)
            {
                XmlNode endnode = xmlDoc.CreateElement(BNSAFile.LoopsXMLNodeName);
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

        internal byte[] GenerateMemoryNoPointers(BinaryWriter stream)
        {
            Pointer = stream.BaseStream.Position;
            byte[] nopointerMemory = new byte[20];
            nopointerMemory[0x10] = FrameDelay;
            nopointerMemory[0x12] = Flags;


            return nopointerMemory;
        }
    }
}