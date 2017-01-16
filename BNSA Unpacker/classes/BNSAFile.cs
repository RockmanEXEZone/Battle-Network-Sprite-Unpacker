using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public List<OAMDataListGroup> ObjectListGroups = new List<OAMDataListGroup>();

        public Boolean ValidBNSA = false;
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
                while (bnsaStream.Position < ProbablyPaletteStartPointer)
                {
                    //Read Tilesets
                    Tileset ts = new Tileset(bnsaStream);
                    Tilesets.Add(ts); //Might need some extra checking...
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

                //Read Object Lists
                Console.WriteLine("Reading Object Lists data at 0x" + bnsaStream.Position.ToString("X2"));
                while (bnsaStream.Position < bnsaStream.Length)
                {
                    //Validate next data is a mini animation.
                    //long listPosition = bnsaStream.Position;
                    OAMDataListGroup group = new OAMDataListGroup(bnsaStream);
                    //if (!group.IsValid)
                    //{
                    //    //End of Mini-Anims
                    //    bnsaStream.Seek(groupStartPos, SeekOrigin.Begin);
                    //    break;
                    //}
                    ObjectListGroups.Add(group);
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
                Console.WriteLine("...Reached end of the file.");
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
        public void resolveReferences()
        {
            int i = 0;
            foreach (Animation animation in Animations)
            {
                Console.WriteLine("Resolving References in Animation " + i);
                animation.ResolveReferences(this);
                i++;
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
