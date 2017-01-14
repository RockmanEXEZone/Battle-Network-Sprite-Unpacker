﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace BNSA_Unpacker
{
    class Program
    {
        static List<int> usedPalettes = new List<int>();

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("A tool to unpack Battle Network Sprite Archive (BNSA) files.");
                Console.WriteLine("bnsa-unpacker -e [path to bnsa]");

            }
            else
            {

                string command = args[0];
                string path = args[1];

                if (command == "-e")
                {
                    byte[] inputFileBytes = File.ReadAllBytes(path);
                    int currentOffset = 4;

                    List<byte[]> archiveParts = new List<byte[]>();

                    //Animation pointers
                    int animPointersLength = inputFileBytes[3] * 4;
                    byte[] animPointers = new byte[animPointersLength];
                    Array.Copy(inputFileBytes, currentOffset, animPointers, 0, animPointersLength);
                    currentOffset += animPointersLength;

                    //Frame data
                    int frameDataLength = findFrameDataEnd(inputFileBytes, currentOffset) - currentOffset;
                    byte[] frameData = new byte[frameDataLength];
                    Array.Copy(inputFileBytes, currentOffset, frameData, 0, frameDataLength);
                    currentOffset += frameDataLength;

                    //Tileset data
                    int tilesetDataLength = findTilesetDataEnd(inputFileBytes, currentOffset) - currentOffset;
                    byte[] tilesetData = new byte[tilesetDataLength];
                    Array.Copy(inputFileBytes, currentOffset, tilesetData, 0, tilesetDataLength);
                    List<int> tilesetOffsetList = getTilesetOffsetList(tilesetData, currentOffset - 4);
                    List<byte[]> tilesetsList = splitTilesets(tilesetData);
                    currentOffset += tilesetDataLength;

                    //Palette data
                    int paletteDataLength = findPaletteDataEnd(inputFileBytes, currentOffset) - currentOffset;
                    byte[] paletteData = new byte[paletteDataLength];
                    Array.Copy(inputFileBytes, currentOffset, paletteData, 0, paletteDataLength);
                    List<int> paletteOffsetList = getPaletteOffsetList(paletteData, currentOffset + 4);
                    List<byte[]> paletteList = splitPalettes(paletteData);
                    currentOffset += paletteDataLength;

                    //Mini-Animations data
                    int miniAnimDataLength = findMiniAnimDataEnd(inputFileBytes, currentOffset) - currentOffset;
                    byte[] miniAnimData = new byte[miniAnimDataLength];
                    Array.Copy(inputFileBytes, currentOffset, miniAnimData, 0, miniAnimDataLength);
                    List<int> miniAnimDataOffsetList = getMiniAnimOffsetList(miniAnimData, currentOffset);
                    List<byte[]> miniAnimsList = splitMiniAnims(miniAnimData);
                    currentOffset += miniAnimDataLength;

                    //Object list data
                    int objectListDataLength = findObjectListDataEnd(inputFileBytes, currentOffset) - currentOffset;
                    byte[] objectListData = new byte[objectListDataLength];
                    Array.Copy(inputFileBytes, currentOffset, objectListData, 0, objectListDataLength);
                    List<int> objectListOffsetList = getObjectListOffsetList(objectListData, currentOffset);
                    List<byte[]> objectListsList = splitObjectLists(objectListData);
                    currentOffset += objectListDataLength;




                    //XML document for animation frames format
                    //XmlDocument frameDoc = new XmlDocument();
                    //XmlNode animNode = frameDoc.CreateElement("animation");
                    //frameDoc.AppendChild(animNode);

                    //XmlNode frameNode = frameDoc.CreateElement("frame");
                    //XmlAttribute tilesetAttr = frameDoc.CreateAttribute("tileset");
                    //tilesetAttr.Value = tilesetsList[0].ToString();
                    //frameNode.Attributes.Append(tilesetAttr);
                    //animNode.AppendChild(frameNode);

                    //frameDoc.Save()




                    //Add arrays to list for export
                    archiveParts.Add(animPointers);
                    archiveParts.Add(frameData);
                    archiveParts.Add(tilesetData);
                    archiveParts.Add(paletteData);
                    archiveParts.Add(miniAnimData);
                    archiveParts.Add(objectListData);

                    //create directories and save
                    string unpackedPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);
                    string framesPath = unpackedPath + @"\\frames";
                    string tilesetsPath = unpackedPath + @"\\tilesets";
                    string palettesPath = unpackedPath + @"\\palettes";
                    string miniAnimsPath = unpackedPath + @"\\minianims";
                    string objectListsPath = unpackedPath + @"\\objectlists";

                    Directory.CreateDirectory(unpackedPath);
                    Directory.CreateDirectory(framesPath);
                    Directory.CreateDirectory(tilesetsPath);
                    Directory.CreateDirectory(palettesPath);
                    Directory.CreateDirectory(miniAnimsPath);
                    Directory.CreateDirectory(objectListsPath);

                    //raw parts export
                    File.WriteAllBytes(framesPath + "\\frames.bin", archiveParts[1].ToArray());
                    File.WriteAllBytes(tilesetsPath + "\\tilesets.bin", archiveParts[2].ToArray());
                    File.WriteAllBytes(palettesPath + "\\palettes.bin", archiveParts[3].ToArray());
                    File.WriteAllBytes(miniAnimsPath + "\\minianims.bin", archiveParts[4].ToArray());
                    File.WriteAllBytes(objectListsPath + "\\objectlists.bin", archiveParts[5].ToArray());


                    //export tiles
                    for (int i = 0; i < tilesetsList.Count; i++)
                    {
                        File.WriteAllBytes(tilesetsPath + "\\tiles" + i + ".bin", tilesetsList[i]);
                    }

                    //export palettes
                    for (int i = 0; i < paletteList.Count; i++)
                    {
                        File.WriteAllBytes(palettesPath + "\\palette" + i + ".bin", paletteList[i]);
                    }

                    //export minianims
                    for (int i = 0; i < miniAnimsList.Count; i++)
                    {
                        File.WriteAllBytes(miniAnimsPath + "\\minianim" + i + ".bin", miniAnimsList[i]);
                    }

                    //export objectlists
                    for (int i = 0; i < objectListsList.Count; i++)
                    {
                        File.WriteAllBytes(objectListsPath + "\\objectlists" + i + ".bin", objectListsList[i]);
                    }



                    using (XmlWriter writer = XmlWriter.Create(framesPath + "\\animations.xml"))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("animation");

                        for (int i = 0; i < frameData.Count(); i++)
                        {
                            writer.WriteStartElement("frame" + i.ToString());
                            //writer.WriteElementString("tileset", getMatchingTileset(frameData, tilesetOffsetList, i));
                            writer.WriteElementString("tileset", "tileset0.bin");
                            writer.WriteElementString("palette", "palette0.bin");
                            writer.WriteElementString("minianim", "minianim0.bin");
                            writer.WriteElementString("objectlist", "objectlist0.bin");
                            writer.WriteElementString("delay", 1.ToString());
                            writer.WriteElementString("flags", "loop");

                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }






                    //frameDoc.Save(framesPath + "\\animations.xml");

                    Console.WriteLine("Sprite unpacked into " + unpackedPath);
                }
            }
        }

        public static int findFrameDataEnd(byte[] archiveFile, int startOffset)
        {
            int endOffset = 0;
            int position = startOffset;

            while (endOffset == 0)
            {
                if (verifyValidFrame(archiveFile, position) == true)
                {
                    position += 0x12;
                    //add to used palettes list to compare with later
                    usedPalettes.Add(BitConverter.ToInt32(archiveFile, position - 0x0E) + 4);
                    int currentFlag = BitConverter.ToInt16(archiveFile, position);
                    if (currentFlag == 0x00 || currentFlag == 0x40 || currentFlag == 0x80 || currentFlag == 0xC0)
                    {
                        position += 2;
                    }
                }
                else
                {
                    endOffset = position;
                }
            }

            return endOffset;
        }

        public static bool verifyValidFrame(byte[] archiveFile, int offset)
        {
            bool isValid = true;

            uint tileset = BitConverter.ToUInt32(archiveFile, offset);
            if (tileset < offset)
            {
                isValid = false;
            }

            uint palette = BitConverter.ToUInt32(archiveFile, offset + 4);
            if (palette < offset)
            {
                isValid = false;
            }

            uint miniAnim = BitConverter.ToUInt32(archiveFile, offset + 8);
            if (miniAnim < offset)
            {
                isValid = false;
            }

            uint objList = BitConverter.ToUInt32(archiveFile, offset + 0x0C);
            if (objList < offset)
            {
                isValid = false;
            }

            uint delay = BitConverter.ToUInt16(archiveFile, offset + 0x10);
            if (delay > 0x0100)
            {
                isValid = false;
            }

            uint flags = BitConverter.ToUInt16(archiveFile, offset + 0x12);
            if (flags > 0x0100)
            {
                isValid = false;
            }

            return isValid;
        }

        public static int findTilesetDataEnd(byte[] archiveFile, int startOffset)
        {
            int endOffset = 0;
            int position = startOffset;

            while (endOffset == 0)
            {
                if (verifyValidTilesetSize(archiveFile, position) == true)
                {
                    int size = BitConverter.ToInt32(archiveFile, position);
                    position += size + 4;
                    //if (size == 0x20)
                    //{
                    //    endOffset = position;
                    //}
                }
                else
                {
                    endOffset = position;
                }

            }

            return endOffset;
        }

        public static bool verifyValidTilesetSize(byte[] archiveFile, int offset)
        {
            bool isValid = true;

            uint size = BitConverter.ToUInt32(archiveFile, offset);
            if (size % 0x20 != 0)
            {
                isValid = false;
            }
            if (size == 0x20)
            {
                int guessPalette0 = usedPalettes.Min();
                if (guessPalette0 - offset >= 0 && guessPalette0 - offset <= 0x04)
                {
                    uint test = BitConverter.ToUInt32(archiveFile, (int)(offset + size));
                    if (test >= 0x00010000)
                    {
                        isValid = false;
                    }
                }

            }
            return isValid;
        }

        public static List<int> getTilesetOffsetList(byte[] tilesetBytes, int offset)
        {
            List<int> offsetList = new List<int>();

            for (int i = 0; i < tilesetBytes.Length;)
            {
                offsetList.Add(offset + i);
                int size = BitConverter.ToInt32(tilesetBytes, i);
                i += size + 4;
            }

            return offsetList;
        }

        public static int findPaletteDataEnd(byte[] archiveFile, int startOffset)
        {
            int endOffset = 0;
            int position = startOffset + 4;

            while (endOffset == 0)
            {
                if (BitConverter.ToInt32(archiveFile, position) != 0x00000004)
                {
                    position += 0x20;
                }
                else
                {
                    if (BitConverter.ToInt16(archiveFile, position + 4) == 0x0100)
                    {
                        endOffset = position;
                    }
                }
            }
            return endOffset;
        }

        public static List<int> getPaletteOffsetList(byte[] paletteBytes, int offset)
        {
            List<int> offsetList = new List<int>();
            offset += 4;

            for (int i = 0; i < paletteBytes.Length;)
            {
                offsetList.Add(offset + i);
                i += 0x20;
            }

            return offsetList;
        }

        public static int findMiniAnimDataEnd(byte[] archiveFile, int startOffset)
        {
            //TODO: this will fail for mugshots
            int endOffset = 0;
            int position = startOffset;

            while (endOffset == 0)
            {
                int headerDefault = BitConverter.ToInt32(archiveFile, position);
                int last1 = 0;
                int last2 = 0;
                int last3 = 0;
                if (headerDefault == 0x04)
                {
                    last1 = archiveFile[position + 7];
                    last2 = archiveFile[position + 8];
                    last3 = archiveFile[position + 9];
                    if (last1 == 0xFF && last2 == 0xFF && last3 == 0xFF)
                    {
                        position += 0x0A;

                        //pad position to 4
                        if (position % 4 == 0)
                        {
                            position += 4;
                        }
                        while (position % 4 != 0)
                        {
                            position++;
                        }
                    }
                    else
                    {
                        endOffset = position;
                    }
                }
            }
            return endOffset;
        }

        public static List<int> getMiniAnimOffsetList(byte[] miniAnimBytes, int offset)
        {
            List<int> offsetList = new List<int>();

            for (int i = 0; i < miniAnimBytes.Length;)
            {
                offsetList.Add(offset + i);
                int pointer1 = BitConverter.ToInt32(miniAnimBytes, i);
                i += pointer1 + 8;
            }

            return offsetList;
        }

        public static int findObjectListDataEnd(byte[] archiveFile, int startOffset)
        {
            int endOffset = 0;
            int position = startOffset;

            while (endOffset == 0)
            {
                int objectHeader = BitConverter.ToInt32(archiveFile, position);
                int last1 = 0;
                int last2 = 0;
                int last3 = 0;
                int last4 = 0;
                int last5 = 0;

                bool objectEnd = false;
                position += objectHeader;
                while (objectEnd == false)
                {
                    last1 = archiveFile[position];
                    last2 = archiveFile[position + 1];
                    last3 = archiveFile[position + 2];
                    last4 = archiveFile[position + 3];
                    last5 = archiveFile[position + 4];
                    if (last1 == 0xFF && last2 == 0xFF && last3 == 0xFF && last4 == 0xFF && last5 == 0xFF)
                    {
                        objectEnd = true;
                    }
                    position += 5;
                }

                //less than/equal to file length
                if (position != archiveFile.Length)
                {
                    //pad position to 4
                    position = roundUp4(position);
                }
                else
                {
                    endOffset = position;
                }

                if (position == archiveFile.Length)
                {
                    endOffset = position;
                }
            }

            return endOffset;
        }

        public static List<int> getObjectListOffsetList(byte[] objectListBytes, int offset)
        {
            List<int> offsetList = new List<int>();

            for (int i = 0; i < objectListBytes.Length;)
            {
                offsetList.Add(offset + i);
                int pointer1 = BitConverter.ToInt32(objectListBytes, i);

                int last1 = 0;
                int last2 = 0;
                int last3 = 0;
                int last4 = 0;
                int last5 = 0;

                bool objectEnd = false;
                i += pointer1;
                while (objectEnd == false)
                {
                    last1 = objectListBytes[i];
                    last2 = objectListBytes[i + 1];
                    last3 = objectListBytes[i + 2];
                    last4 = objectListBytes[i + 3];
                    last5 = objectListBytes[i + 4];
                    if (last1 == 0xFF && last2 == 0xFF && last3 == 0xFF && last4 == 0xFF && last5 == 0xFF)
                    {
                        objectEnd = true;
                    }
                    i += 5;
                }
                if (i != objectListBytes.Length)
                {
                    //pad position to 4
                    if (i % 4 == 0)
                    {
                        i += 4;
                    }
                    while (i % 4 != 0)
                    {
                        i++;
                    }
                }

                //i += i;
            }

            return offsetList;
        }

        public static List<byte[]> splitTilesets(byte[] tilesetBytes)
        {
            List<byte[]> tilesets = new List<byte[]>();

            for (int i = 0; i < tilesetBytes.Length;)
            {
                int length = BitConverter.ToInt32(tilesetBytes, i) + 4;
                byte[] tiles = new byte[length];
                Array.Copy(tilesetBytes, i, tiles, 0, length);
                tilesets.Add(tiles);
                i += length;
            }

            return tilesets;
        }

        public static List<byte[]> splitPalettes(byte[] paletteBytes)
        {
            List<byte[]> palettes = new List<byte[]>();

            for (int i = 0; i < paletteBytes.Length;)
            {
                if (i == 0)
                {
                    i += 4;
                }
                byte[] palette = new byte[0x20];
                Array.Copy(paletteBytes, i, palette, 0, 0x20);
                palettes.Add(palette);
                i += 0x20;
            }

            return palettes;
        }

        public static List<byte[]> splitMiniAnims(byte[] miniAnimBytes)
        {
            List<byte[]> miniAnims = new List<byte[]>();

            for (int i = 0; i < miniAnimBytes.Length;)
            {
                int pointer = BitConverter.ToInt32(miniAnimBytes, i);
                int length = pointer;
                int last1 = 0;
                int last2 = 0;
                int last3 = 0;
                bool objectEnd = false;

                while (objectEnd == false)
                {
                    last1 = miniAnimBytes[i + length];
                    last2 = miniAnimBytes[i + length + 1];
                    last3 = miniAnimBytes[i + length + 2];

                    if (last1 == 0xFF && last1 == 0xFF && last3 == 0xFF)
                    {
                        length += 3;
                        byte[] miniAnim = new byte[length];
                        Array.Copy(miniAnimBytes, i, miniAnim, 0, length);
                        miniAnims.Add(miniAnim);
                        objectEnd = true;
                        i += length;
                        i = roundUp4(i);
                    }
                    else
                    {
                        length += 3;
                    }
                }
            }

            return miniAnims;
        }

        public static List<byte[]> splitObjectLists(byte[] objectListBytes)
        {
            List<byte[]> objectLists = new List<byte[]>();

            for (int i = 0; i < objectListBytes.Length;)
            {
                int pointer = BitConverter.ToInt32(objectListBytes, i);
                int length = pointer;
                int last1 = 0;
                int last2 = 0;
                int last3 = 0;
                int last4 = 0;
                int last5 = 0;
                bool objectEnd = false;

                while (objectEnd == false)
                {
                    last1 = objectListBytes[i + length];
                    last2 = objectListBytes[i + length + 1];
                    last3 = objectListBytes[i + length + 2];
                    last4 = objectListBytes[i + length + 3];
                    last5 = objectListBytes[i + length + 4];

                    if (last1 == 0xFF && last2 == 0xFF && last3 == 0xFF && last4 == 0xFF && last5 == 0xFF)
                    {
                        length += 5;
                        byte[] objectList = new byte[length];
                        Array.Copy(objectListBytes, i, objectList, 0, length);
                        objectLists.Add(objectList);
                        objectEnd = true;
                        i += length;
                        i = roundUp4(i);
                    }
                    else
                    {
                        length += 5;
                    }
                }
            }
            return objectLists;
        }


        public static string getMatchingTileset(byte[] frameData, List<int>tilesetsList, int i)
        {
            string filename = "";

            int currentTileset = BitConverter.ToInt32(frameData, i*5);
            int fileIndex = 0;

            for (int k = 0; k < tilesetsList.Count; k++)
            {
                int offset = tilesetsList[k];
                if (offset == i)
                {
                    fileIndex = k;
                    break;
                }
            }

            filename = "tileset" + fileIndex.ToString() + ".bin";
            return filename;
        }





        public static int roundUp4(int num)
        {
            if (num % 4 == 0)
            {
                num += 4;
            }
            while (num % 4 != 0)
            {
                num++;
            }
            return num;
        }

    }
}