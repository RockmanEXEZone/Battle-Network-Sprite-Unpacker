using System;
using System.Collections.Generic;
using System.IO;

namespace BNSA_Unpacker.classes
{
    public class Palette
    {
        public long Pointer;
        public byte[] Memory;
        public int[] Colors; //ARGB

        public Palette(Stream stream)
        {
            Pointer = stream.Position;
            Memory = new byte[0x20];
            stream.Read(Memory, 0, 0x20);
            Colors = new int[16];
        }

        /// <summary>
        /// Constructs a palette object from a palette binary file
        /// </summary>
        /// <param name="palettePath">Path to palette file</param>
        public Palette(string palettePath)
        {
            Memory = File.ReadAllBytes(palettePath);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="palette"></param>
        public Palette(Palette palette)
        {
            Pointer = palette.Pointer;
            Memory = new byte[0x20];
            Colors = new int[16];
            Array.Copy(palette.Memory, Memory, palette.Memory.Length);
            Array.Copy(palette.Colors, Colors, palette.Colors.Length);
        }

        /// <summary>
        /// Reads the Palette memory and generates ARGB color values.
        /// </summary>
        public void GenerateColorsFromPalette()
        {
            for (int i = 0; i < 16; i++)
            {
                short paletteColor16bit = BitConverter.ToInt16(new byte[2] { (byte)Memory[i * 2], (byte)Memory[i * 2 + 1] }, 0);
                Colors[i] = bgr2argb(paletteColor16bit);
            }
            Colors[0] = System.Drawing.Color.Transparent.ToArgb();
        }

        public void ConvertWorkingColorsTo15BitBGR()
        {
            int i = 0;
            foreach (int color in Colors)
            {
                byte r = (byte)Math.Round((double)(((color & 0xFF0000) >> 16) * 15 / 255));
                byte g = (byte)Math.Round((double)(((color & 0xFF00) >> 8) * 15 / 255));
                byte b = (byte)Math.Round((double)((color & 0xFF) * 15 / 255));
                Colors[i] = b << 10 | g << 5 | g;
                i++;
            }
        }

        /// <summary>
        /// Converts BGR to Alpha RGB for displaying in a windows bitmap
        /// </summary>
        /// <param name="bgr">bbggrr short value.</param>
        /// <returns></returns>
        private int bgr2argb(short bgr)
        {
            byte a = 0xFF,
                 r = (byte)((bgr & 0x1F) << 3),
                 g = (byte)(((bgr >> 5) & 0x1F) << 3),
                 b = (byte)(((bgr >> 10) & 0x1F) << 3);
            return (a << 24) | (r << 16) | (g << 8) | b;
        }

        /// <summary>
        /// Writes this palettes's binary to disk in its own file.
        /// </summary>
        /// <param name="outputPath">Directory to put .bin into</param>
        /// <param name="index">Palette index, as part of the filename</param>
        public void Export(string outputPath, int index)
        {
            File.WriteAllBytes(outputPath + @"\palette" + index.ToString().PadLeft(2, '0') + ".bin", Memory);
        }
    }
}
