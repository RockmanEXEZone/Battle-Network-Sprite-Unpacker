using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BNSA_Unpacker.classes
{
    class BNSAFile
    {
        public long TilesetStartPointer;
        public long PaletteStartPointer;
        public long ProbablyPaletteStartPointer = long.MaxValue; //will always force min

        public int AnimationCount = 0;
        public int LargestTileset = 0;

        public List<Animation> Animations = new List<Animation>();
        public List<Tileset> Tilesets = new List<Tileset>();
        public List<Palette> Palettes = new List<Palette>();
        public List<MiniAnimGroup> MiniAnimGroups = new List<MiniAnimGroup>();
        public List<OAMDataList> OAMDataLists = new List<OAMDataList>();

        public Boolean ValidBNSA = false;

        public static readonly string MiniAnimationXMLNodeName = "minianimgroup";
        public static readonly string OAMDataListXMLNodeName = "oamdatalistindex";
        public static readonly string TilesetXMLNodeName = "tileset";

        public BNSAFile(string path)
        {
            using (FileStream bnsaStream = File.OpenRead(path))
            {
                //HEADER
                LargestTileset = bnsaStream.ReadByte();
                //Magic Number Test
                if (bnsaStream.ReadByte() != 0 || bnsaStream.ReadByte() != 1) //0x1 and 0x2 positions
                {
                    //Invalid Magic Number
                    ValidBNSA = false;
                    return;
                }
                AnimationCount = bnsaStream.ReadByte();
                Console.WriteLine("Number of Animations: " + AnimationCount);
                //Read Animation Pointers
                for (int i = 0; i < AnimationCount; i++)
                {
                    int animationPointer = ReadIntegerFromStream(bnsaStream);
                    long nextPosition = bnsaStream.Position;
                    Animation animation = new Animation(this, animationPointer, bnsaStream);
                    Animations.Add(animation);
                    if (i < AnimationCount - 1)
                    {
                        bnsaStream.Seek(nextPosition, SeekOrigin.Begin); //reset position to next pointer
                    }
                }

                //Read Tilesets
                TilesetStartPointer = bnsaStream.Position;
                Console.WriteLine("Reading Tilesets, starting at 0x" + TilesetStartPointer.ToString("X2"));
                int index = 0;
                while (bnsaStream.Position < ProbablyPaletteStartPointer)
                {
                    //Read Tilesets
                    Tileset ts = new Tileset(bnsaStream, index);
                    Tilesets.Add(ts); //Might need some extra checking...
                    index++;
                }

                //Read Palettes
                PaletteStartPointer = bnsaStream.Position;
                Console.WriteLine("Found start of Palettes at 0x" + PaletteStartPointer.ToString("X2"));
                if (ReadIntegerFromStream(bnsaStream) == 0x20)
                {
                    while (true)
                    {
                        long pos = bnsaStream.Position;

                        //Verify next item is a palette and not a minianim
                        int minianimCheckPtr1 = ReadIntegerFromStream(bnsaStream);
                        if (minianimCheckPtr1 % 4 == 0)
                        {
                            //All pointers in the minianim table are lined up on a 4-byte boundary.
                            //Indexing starts at 0. Each pointer is 4 bytes, so the first one will have to point to a 4-point value

                            //could be an pointer to 1-frame minianim, maybe... 
                            if (minianimCheckPtr1 == 4)
                            {
                                //Check for 1 pointer minianim signature
                                if (bnsaStream.ReadByte() == 0x0 && bnsaStream.ReadByte() == 0x01 && bnsaStream.ReadByte() == 0x80)
                                {
                                    bnsaStream.Seek(pos, SeekOrigin.Begin);
                                    break; //Not a palette. Next Item is a single-frame minianim group. End of Palette Block
                                }
                            }

                            //Could be a multi-miniframe mugshot, let's check the amount of pointers and the values.
                            int numberOfMiniAnimsInGroup = minianimCheckPtr1 / 4; //Pointer will go to first value after pointer table, so divide by num bytes in a pointer
                            int previousPointerToCheckAgainst = minianimCheckPtr1;
                            Boolean validMiniAnimPointerData = true;
                            for (int i = 1; i < numberOfMiniAnimsInGroup; i++)
                            {
                                int nextMiniAnimCheckPtr = ReadIntegerFromStream(bnsaStream);
                                if (nextMiniAnimCheckPtr < previousPointerToCheckAgainst)
                                {
                                    validMiniAnimPointerData = false;
                                    break;
                                }
                            }
                            if (validMiniAnimPointerData)
                            {
                                //Probably a minianim
                                bnsaStream.Seek(pos, SeekOrigin.Begin);
                                break;
                            }

                        }
                        bnsaStream.Seek(pos, SeekOrigin.Begin);
                        Console.WriteLine("Reading Palette 0x" + bnsaStream.Position.ToString("X2"));
                        Palette palette = new Palette(bnsaStream);
                        Palettes.Add(palette);
                        //Console.WriteLine("Reading next Palette 0x" + bnsaStream.Position.ToString("X2"));

                    }
                }//} else
                 //{
                 //    Console.WriteLine("Palettes should start here, but we didn't find 0x00000020! (At position  0x" + PaletteStartPointer.ToString("X2") + ")");
                 //    return;
                 //}

                //Read Mini-Animations
                Console.WriteLine("Reading MiniAnim Data at 0x" + bnsaStream.Position.ToString("X2"));
                while (true)
                {
                    //Validate next data is a mini animation.
                    long groupStartPos = bnsaStream.Position;
                    MiniAnimGroup group = new MiniAnimGroup(bnsaStream);
                    if (!group.IsValid)
                    {
                        //End of Mini-Anims
                        bnsaStream.Seek(groupStartPos, SeekOrigin.Begin);
                        break;
                    }
                    MiniAnimGroups.Add(group);
                    //Round up to the next 4 byte boundary
                    bnsaStream.ReadByte(); //pos++
                    while (bnsaStream.Position % 4 != 0)
                    {
                        bnsaStream.ReadByte(); //Official game padding. Since we are assuming these are all official, when repacking we should also follow this padding rule.
                    }
                }



                //Read OAM Data Block Lists
                Console.WriteLine("Reading OAM Data Blocks at 0x" + bnsaStream.Position.ToString("X2"));
                while (bnsaStream.Position < bnsaStream.Length)
                {
                    OAMDataList oamDataList = new OAMDataList(bnsaStream);
                    OAMDataLists.Add(oamDataList);

                    //Round up to the next 4 byte boundary
                    if (bnsaStream.Position < bnsaStream.Length) //Might end on a 4byte boundary already.
                    {
                        bnsaStream.ReadByte(); //pos++
                        while (bnsaStream.Position % 4 != 0)
                        {
                            bnsaStream.ReadByte(); //Official game padding. Since we are assuming these are all official, when repacking we should also follow this padding rule.
                        }
                    }
                }

                if (bnsaStream.Position != bnsaStream.Length)
                {
                    Console.WriteLine("...Nothing left to parse but we aren't at the end of the file!");
                }
                else
                {
                    Console.WriteLine("...Reached end of the file.");
                }
            }
        }

        /// <summary>
        /// Reads a 32-bit integer value from the stream, advancing it 4 bytes forward.
        /// </summary>
        /// <param name="stream">Stream to read</param>
        /// <returns>Integer</returns>
        public static int ReadIntegerFromStream(FileStream stream)
        {
            byte[] pointer = new byte[4]; //32-bit pointer
            stream.Read(pointer, 0, 4);
            return BitConverter.ToInt32(pointer, 0);
        }

        /// <summary>
        /// Changes pointers and indexes to use object references. Essentially builds the links in code that are done in the binary BNSA file.
        /// This method should be called after a BNSA file is parsed, but before writing the XML as the references are not yet established
        /// </summary>
        public void ResolveReferences()
        {
            int i = 0;
            foreach (Animation animation in Animations)
            {
                Console.WriteLine("Resolving References in Animation " + i);
                animation.ResolveReferences(this);
                i++;
            }

            //minianims are resolved in frame
            //i = 0;
            //foreach (MiniAnimGroup miniAnimGroup in MiniAnimGroups)
            //{
            //    Console.WriteLine("Resolving References in MiniAnimation " + i);
            //    miniAnimGroup.ResolveReferences(this);
            //    i++;
            //}
        }

        /// <summary>
        /// Unpacks the BNSA file to disk and writes the linking XML.
        /// </summary>
        /// <param name="outputPath">Directory to output data and xml to</param>
        public void Unpack(string outputFolder)
        {
            //create directories and save
            Console.WriteLine("Creating output directories");
            string framesPath = outputFolder + @"\\frames";
            string tilesetsPath = outputFolder + @"\\tilesets";
            string palettesPath = outputFolder + @"\\palettes";
            string miniAnimsPath = outputFolder + @"\\minianims";
            string oamDataListsPath = outputFolder + @"\\oamdatalists";

            Directory.CreateDirectory(outputFolder);
            Directory.CreateDirectory(framesPath);
            Directory.CreateDirectory(tilesetsPath);
            Directory.CreateDirectory(palettesPath);
            Directory.CreateDirectory(miniAnimsPath);
            Directory.CreateDirectory(oamDataListsPath);

            int i = 0;
            foreach (Tileset tileset in Tilesets)
            {
                tileset.Export(tilesetsPath, i);
                i++;
            }

            i = 0;
            foreach (Palette palette in Palettes)
            {
                palette.Export(palettesPath, i);
                i++;
            }

            i = 0;
            foreach (Animation animation in Animations)
            {
                animation.Export(framesPath, i); //yes output to frame path.
                i++;
            }

            i = 0;
            foreach (MiniAnimGroup miniAnimGroup in MiniAnimGroups)
            {
                miniAnimGroup.Export(miniAnimsPath, i);
                i++;
            }

            i = 0;
            foreach (OAMDataList oamDataList in OAMDataLists)
            {
                oamDataList.Export(oamDataListsPath, i);
                i++;
            }
        }

        /// <summary>
        /// Generates XML file that ties all the exported resources together and can be used to recompile a BNSA file with this program (-r)
        /// </summary>
        /// <param name="outputFolder">Directory to put bnsa.xml into</param>
        public void GenerateLinkingXML(string outputFolder)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode = xmlDoc.CreateElement("bnsafile");
            xmlDoc.AppendChild(rootNode);

            //ANIMATIONS
            XmlNode animationsNode = xmlDoc.CreateElement("animations");
            rootNode.AppendChild(animationsNode);
            int index = 0;
            foreach (Animation anim in Animations)
            {
                XmlNode animationNode = xmlDoc.CreateElement("animation");
                XmlAttribute attribute = xmlDoc.CreateAttribute("index");
                attribute.Value = index.ToString();
                animationNode.Attributes.Append(attribute);
                List<XmlNode> frameNodes = anim.GetChildNodes(xmlDoc);
                appendAllNodes(frameNodes, animationNode);
                animationsNode.AppendChild(animationNode);
                index++;
            }

            //PALETTES ARE REPACKED BY WHATS IN THE palettes DIRECTORY AS paletteXX.bin. Reads 00-15.
            //TILESETS ARE REPACKED BY WHATS IN THE tilesets DIRECTORY AS tilesetXXX.bin. Reads 000-999. The XXX refers to the tileset index, which is referenced by the animation-frame.

            //MiniAnimationGroups
            foreach (MiniAnimGroup minianimgroup in MiniAnimGroups)
            {

            }

            string filename = "\\sprite.xml";
            xmlDoc.Save(outputFolder + filename);

        }

        /// <summary>
        /// Convenience method to append a list of nodes to another
        /// </summary>
        /// <param name="nodesToAppend">Node list to append in order</param>
        /// <param name="nodeToAppendTo">Node to append to</param>
        private void appendAllNodes(List<XmlNode> nodesToAppend, XmlNode nodeToAppendTo)
        {
            foreach (XmlNode node in nodesToAppend)
            {
                nodeToAppendTo.AppendChild(node);
            }
        }

        //public int findTilesetDataEnd(FileStream stream)
        //{
        //    while (true)
        //    {
        //        if (verifyValidTilesetSize(archiveFile, position) == true)
        //        {
        //            int size = BitConverter.ToInt32(archiveFile, position);
        //            position += size + 4;
        //            //if (size == 0x20)
        //            //{
        //            //    endOffset = position;
        //            //}
        //        }
        //        else
        //        {
        //            endOffset = position;
        //        }

        //    }

        //    return endOffset;
        //}
    }
}
