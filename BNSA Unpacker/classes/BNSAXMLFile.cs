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
    class BNSAXMLFile
    {
        private bool IsValid = false;
        private string FilePath;
        List<Palette> Palettes;
        List<MiniAnimGroup> MiniAnimationGroups;
        List<OAMDataListGroup> OAMDataListGroups;
        List<Animation> Animations;
        List<Tileset> Tilesets;
        /// <summary>
        /// Reads a project XML file and relinks all the parts for file rebuilding
        /// </summary>
        /// <param name="filePath">XML file to parse</param>
        public BNSAXMLFile(string filePath)
        {
            this.FilePath = filePath;
            Console.WriteLine("Loading project file: " + filePath);
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            //Parse Process:
            //1. Read animation tree
            //2. Read tileset folder
            //3. Read minianim folder
            //4. Read palette folder
            //5. Read oamdatalists folder
            //6. Link all objects

            //Build lists, mark pointer placeholders
            //Byte align after lists are built
            //insert pointers
            //merge into single file

            string tilesetBasepath = Directory.GetParent(filePath).FullName + "\\tilesets\\tileset";
            string palettesBasepath = Directory.GetParent(filePath).FullName + "\\palettes\\palette";
            string oamDataListsBasepath = Directory.GetParent(filePath).FullName + "\\oamdatalists\\oamdatalist";
            string miniAnimsBasepath = Directory.GetParent(filePath).FullName + "\\minianims\\minianim";



            XmlNodeList animationNodes = doc.DocumentElement.SelectNodes("/bnsafile/animations/animation");
            Console.WriteLine("Found " + animationNodes.Count + " animations");
            Animations = new List<Animation>();
            foreach (XmlNode animationNode in animationNodes)
            {
                //Read Animation XML
                Animation animation = new Animation(animationNode);
                //Verify references exist in filesystem
                foreach (Frame f in animation.Frames)
                {
                    //Verify Tileset
                    string tilesetPath = tilesetBasepath + f.TilesetIndex.ToString().PadLeft(3, '0') + ".bin";
                    if (!File.Exists(tilesetPath))
                    {
                        IsValid = false;
                        Console.WriteLine("Tileset file missing: " + tilesetPath + ", referenced by Anim " + animation.Index + " frame " + f.Index);
                        return;
                    }

                    //Verify OAM
                    string oamPath = oamDataListsBasepath + f.OAMDataListIndex + "-0-0.bin";
                    if (!File.Exists(oamPath))
                    {
                        IsValid = false;
                        Console.WriteLine("OAM Data List file missing: " + oamPath + ", referenced by Anim " + animation.Index + " frame " + f.Index);
                        return;
                    }

                    //Verify MiniAnim
                    string minianimFrame1 = miniAnimsBasepath + f.MiniAnimationIndex + "-0-0.bin";
                    if (!File.Exists(minianimFrame1))
                    {
                        IsValid = false;
                        Console.WriteLine("MiniAnimation file missing: " + minianimFrame1 + ", referenced by Anim " + animation.Index + " frame " + f.Index);
                        return;
                    }
                }
                Animations.Add(animation);
                Console.WriteLine("OK - animation contains all direct binary references");
            }
            //Read Tilesets
            Tilesets = new List<Tileset>();
            int index = 0;
            for (int i = 0; i < 999; i++)
            {
                string tilesetPath = tilesetBasepath + index.ToString().PadLeft(3, '0') + ".bin";
                if (File.Exists(tilesetPath))
                {
                    Tileset tileset = new Tileset(tilesetPath, i);
                    Tilesets.Add(tileset);
                }
                else
                {
                    break; //No more tilesets
                }
                index++;
            }
            Console.WriteLine("Read " + Tilesets.Count + " tilesets");

            //Read Palettes
            Palettes = new List<Palette>();
            for (int i = 0; i < 16; i++)
            {
                string palettePath = palettesBasepath + i.ToString().PadLeft(2, '0') + ".bin";
                if (File.Exists(palettePath))
                {
                    Palette palette = new Palette(palettePath);
                    Palettes.Add(palette);
                }
                else
                {
                    break; //No more palettes
                }
            }
            Console.WriteLine("Read " + Palettes.Count + " palettes");

            //Read MiniAnims
            string[] miniAnimFiles = Directory.GetFiles(Directory.GetParent(filePath).FullName + "\\minianims");
            int maxindex = 0;
            foreach (string file in miniAnimFiles)
            {
                string fname = Path.GetFileNameWithoutExtension(file);
                fname = fname.Substring(8);
                int dashIndex = fname.IndexOf('-');
                fname = fname.Substring(0, dashIndex);
                maxindex = Math.Max(maxindex, Int32.Parse(fname));
            }
            Console.WriteLine("Found " + (maxindex + 1) + " mini animations");
            MiniAnimationGroups = new List<MiniAnimGroup>();
            for (int i = 0; i <= maxindex; i++)
            {
                int subindex = 0;
                Console.WriteLine("Parsing Group " + i);
                MiniAnimGroup group = new MiniAnimGroup();
                while (File.Exists(miniAnimsBasepath + maxindex + "-" + subindex + "-0.bin"))
                {
                    //MiniAnimation List
                    MiniAnim animation = new MiniAnim(miniAnimsBasepath, i, subindex);
                    group.MiniAnimations.Add(animation);
                    subindex++;
                }
                MiniAnimationGroups.Add(group);
            }

            //Read OAM Data
            string[] oamListEntries = Directory.GetFiles(Directory.GetParent(filePath).FullName + "\\oamdatalists");
            maxindex = 0;
            foreach (string file in oamListEntries)
            {
                string fname = Path.GetFileNameWithoutExtension(file);
                fname = fname.Substring(11);
                int dashIndex = fname.IndexOf('-');
                fname = fname.Substring(0, dashIndex);
                maxindex = Math.Max(maxindex, Int32.Parse(fname));
            }
            Console.WriteLine("Found " + (maxindex + 1) + " OAM Data List Groups");
            OAMDataListGroups = new List<OAMDataListGroup>();
            for (int i = 0; i <= maxindex; i++)
            {
                Console.WriteLine("Parsing OAM Data List Group " + i);
                OAMDataListGroup list = new OAMDataListGroup(oamDataListsBasepath, i);
                OAMDataListGroups.Add(list);
            }
        }

        /// <summary>
        /// Links parsed data.
        /// </summary>
        public void ResolveReferences()
        {

        }

        /// <summary>
        /// Constructs a BNSA File from the parsed data this object contains
        /// </summary>
        /// <param name="destinationFile">File to write output to</param>
        public void RepackBNSA(string destinationFile)
        {
            Console.WriteLine("Repacking BNSA");

            long palettesPointer;

            try
            {
                FileStream fstream = new FileStream(destinationFile, FileMode.Create);
                BinaryWriter stream = new BinaryWriter(fstream);
                //Write Header
                string tilesetBasepath = Directory.GetParent(FilePath).FullName + "\\tilesets\\";

                FileInfo largestTileset = new DirectoryInfo(tilesetBasepath).EnumerateFiles()
                           .OrderByDescending(f => f.Length)
                           .FirstOrDefault();
                //byte largestTileCount = (byte)Math.Ceiling((decimal)largestTileset.Length / 0x20);
                byte largestTileCount = (byte)(largestTileset.Length / 0x20);

                stream.Write(largestTileCount);
                stream.Write((byte)0x0);
                stream.Write((byte)0x1);
                stream.Write((byte)Animations.Count);

                //Animations Pointer Table
                int currentOffset = Animations.Count * 4;
                foreach (Animation anim in Animations)
                {
                    Console.WriteLine("Write Animation Pointer 0x" + currentOffset.ToString("X2"));
                    stream.Write(currentOffset);
                    currentOffset += anim.Frames.Count * 0x14;
                }

                //Write Frames
                foreach (Animation anim in Animations)
                {
                    Console.WriteLine("Writing Animation to 0x" + stream.BaseStream.Position.ToString("X2"));

                    foreach (Frame frame in anim.Frames)
                    {
                        Console.WriteLine("--Writing Frame to 0x" + stream.BaseStream.Position.ToString("X2"));
                        stream.Write(frame.GenerateMemoryNoPointers(stream));
                    }
                }

                //Write Tilesets
                foreach (Tileset tileset in Tilesets)
                {
                    Console.WriteLine("Write Tileset " + tileset.Index + " to 0x" + stream.BaseStream.Position.ToString("X2"));
                    tileset.Pointer = stream.BaseStream.Position;
                    stream.Write(tileset.BitmapSize);
                    stream.Write(tileset.BitmapData);
                }

                //Write Palettes
                palettesPointer = stream.BaseStream.Position;
                Console.WriteLine("Writing Palettes Block at 0x" + palettesPointer.ToString("X2"));
                stream.Write(0x20); //Palettes Size
                foreach (Palette palette in Palettes)
                {
                    Console.WriteLine("--Writing Palette at 0x" + stream.BaseStream.Position.ToString("X2"));
                    stream.Write(palette.Memory);
                }

                //WriteMiniAnims
                long miniAnimsStartPointer = stream.BaseStream.Position;
                foreach (MiniAnimGroup group in MiniAnimationGroups)
                {
                    long groupPointerTablePointer = stream.BaseStream.Position;
                    //Write AnimGroup Pointer Table
                    currentOffset = group.MiniAnimations.Count * 4; //End of pointer table offset
                    foreach (MiniAnim miniAnim in group.MiniAnimations)
                    {
                        Console.WriteLine("Write MiniAnimGroup SubAnim Pointer 0x" + currentOffset.ToString("X2"));
                        stream.Write(currentOffset);
                        currentOffset += miniAnim.MiniFrames.Count * 0x3 + 3;
                        currentOffset += 1; //Round Up padding calc
                        while ((groupPointerTablePointer + currentOffset) % 4 != 0)
                        {
                            currentOffset++;
                        }
                    }

                    Console.WriteLine("Writing MiniAnimationGroup to 0x" + stream.BaseStream.Position.ToString("X2"));
                    group.Pointer = stream.BaseStream.Position;
                    foreach (MiniAnim miniAnim in group.MiniAnimations)
                    {
                        Console.WriteLine("--Writing MiniAnimation to 0x" + stream.BaseStream.Position.ToString("X2"));
                        foreach (MiniFrame mf in miniAnim.MiniFrames)
                        {
                            stream.Write(mf.Memory);
                        }
                        //AnimTerminator
                        stream.Write((byte)0xFF);
                        stream.Write((byte)0xFF);
                        stream.Write((byte)0xFF);

                        //Round Up Padding
                        stream.Write((byte)0x00);
                        while (stream.BaseStream.Position % 4 != 0)
                        {
                            stream.Write((byte)0x00);
                        }
                    }
                }
                //Write OAM Datalists
                long objectListsStartPointer = stream.BaseStream.Position;
                Console.WriteLine("OAMDataLists Block: 0x" + stream.BaseStream.Position.ToString("X2"));

                foreach (OAMDataListGroup list in OAMDataListGroups)
                {
                    long groupPointerTablePointer = stream.BaseStream.Position;
                    //Write AnimGroup Pointer Table
                    currentOffset = list.OAMDataLists.Count * 4; //End of pointer table offset
                    foreach (OAMDataList dataList in list.OAMDataLists)
                    {
                        Console.WriteLine("Write OAM Data List Pointer 0x" + currentOffset.ToString("X2"));
                        stream.Write(currentOffset);
                        //currentOffset += dataList.Count * 0x5 + 5;
                        currentOffset += 1; //Round Up padding calc
                        while ((groupPointerTablePointer + currentOffset) % 4 != 0)
                        {
                            currentOffset++;
                        }
                    }

                    Console.WriteLine("Writing OAMDataList to 0x" + stream.BaseStream.Position.ToString("X2"));
                    list.Pointer = stream.BaseStream.Position;
                    foreach (OAMDataList dataList in list.OAMDataLists)
                    {
                        Console.WriteLine("--Writing OAMDataList " + dataList.Index + " to 0x" + stream.BaseStream.Position.ToString("X2"));
                        foreach (OAMDataListEntry listEntry in dataList.OAMDataListEntries)
                        {
                            Console.WriteLine("----Writing OAMDataListEntry "+listEntry.Index+" to 0x" + stream.BaseStream.Position.ToString("X2"));
                            stream.Write(listEntry.Memory);
                        }
                    }
                    //List Terminator
                    stream.Write((byte)0xFF);
                    stream.Write((byte)0xFF);
                    stream.Write((byte)0xFF);
                    stream.Write((byte)0xFF);
                    stream.Write((byte)0xFF);

                    //Round Up Padding
                    stream.Write((byte)0x00);
                    while (stream.BaseStream.Position % 4 != 0)
                    {
                        stream.Write((byte)0x00);

                    }
                }

                stream.Close();
            }
            catch (NotFiniteNumberException e)
            {

            }
        }
    }
}
