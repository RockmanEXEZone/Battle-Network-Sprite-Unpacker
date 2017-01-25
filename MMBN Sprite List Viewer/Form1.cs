using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Imaging;
using BNSA_Unpacker.classes;
using System.Runtime.InteropServices;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using ImageMagick;

namespace MMBN_Sprite_List_Viewer
{
    public partial class Form1 : Form
    {
        public Byte[] ROM;
        public Byte[] BNSAMemory;
        public BNSAFile BNSA;

        string filename;
        string lastsearched = "";
        int offset = 0;
        int startoffset = 0;
        int endoffset = 0;
        #region ROM CODE DECLARATIONS
        const String BN6USROMCODES = "BR6PBR6EBR5EBR5P";
        const String BN6JPROMCODES = "BR6JBR5J";
        const String BN5TPUSROMCODES = "BRBE";
        const String BN5TCUSROMCODES = "BRKE";
        const String BN5TPJPROMCODES = "BRBJ";
        const String BN5TCJPROMCODES = "BRKJ";
        const String BN45JPROMCODES = "BR4J";
        const String BN4BUSROMCODES = "B4BE";
        const String BN4RUSROMCODES = "B4WE";
        const String BN4BJPROMCODES = "B4BJ";
        const String BN4RJPROMCODES = "B4WJ";
        const String BN3WUSROMCODES = "A6BE";
        const String BN3BUSROMCODES = "A3XE";
        const String BN3WJPROMCODES = "A6BJ";
        const String BN3BJPROMCODES = "A3XJ";
        const String BN2USROMCODES = "AE2E";
        const String BN2JPROMCODES = "AE2J";
        const String BN1USROMCODES = "AREE";
        const String BN1EUROMCODES = "AREP";
        const String BN1JPROMCODES = "AREJ";
        private string bnsaFilePath;
        private BNSAFile ActiveBNSA;

        #endregion
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets a tile from a tileset given a tile number
        /// </summary>
        /// <param name="tilesetData"></param>
        /// <param name="tileNumber"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private byte[] getTile(byte[] tilesetData, int tileNumber, int width, int height)
        {
            int tileSizeInBytes = ((height * width) + (height * width) % 2 + 1) / 2; //Round up if odd, divide whole result by 2.
            byte[] graphics = new byte[tileSizeInBytes];
            int tileStartIndex = tileNumber * 0x20 + 4;
            System.Array.Copy(tilesetData, tileStartIndex, graphics, 0, tileSizeInBytes); //no idea if this is right
            return graphics;
        }

        private static int bgr2argb(short bgr)
        {
            byte a = 0xFF,
                 r = (byte)((bgr & 0x1F) << 3),
                 g = (byte)(((bgr >> 5) & 0x1F) << 3),
                 b = (byte)(((bgr >> 10) & 0x1F) << 3);
            return (a << 24) | (r << 16) | (g << 8) | b;
        }

        /// <summary>
        /// Converts a palette binary array into a list of palette colors to use wile drawing.
        /// </summary>
        /// <param name="bnPaletteBinary">Battle Network Binary Palette Data</param>
        /// <returns>Array of alpha-rgb values</returns>
        private int[] getUsablePaletteColors(byte[] bnPaletteBinary)
        {
            int[] paletteData = new int[16];
            for (int i = 0; i < 16; i++)
            {
                short paletteColor16bit = BitConverter.ToInt16(new byte[2] { (byte)bnPaletteBinary[i * 2], (byte)bnPaletteBinary[i * 2 + 1] }, 0);
                paletteData[i] = bgr2argb(paletteColor16bit);
            }
            paletteData[0] = Color.Transparent.ToArgb();
            return paletteData;
        }

        //    int[] pal = new int[16];
        //    fixed (byte* pointer = &source[0])
        //    {
        //        for (int i = 0; i < 16; i++)
        //        {
        //            pal[i] = bgr2argb((*(short*)(offset + pointer + (i * sizeof(short)))));
        //        }
        //        pal[0] = Color.Transparent.ToArgb();
        //        return pal;
        //    }
        //}

        //private static unsafe int[] getpalette(byte[] source, int offset)
        //{
        //    int[] pal = new int[16];
        //    fixed (byte* pointer = &source[0])
        //    {
        //        for (int i = 0; i < 16; i++)
        //        {
        //            pal[i] = bgr2argb((*(short*)(offset + pointer + (i * sizeof(short)))));
        //        }
        //        pal[0] = Color.Transparent.ToArgb();
        //        return pal;
        //    }
        //}

        /// <summary>
        /// Reads tileset data and applies the palette colors... i think?
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="graphics"></param>
        /// <param name="palette"></param>
        /// <returns></returns>
        private static unsafe Bitmap Read4BBP(int width, int height, byte[] graphics, int[] palette)
        {
            Bitmap img = new Bitmap(width, height);
            BitmapData bitdat = img.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            int* pointer = (int*)bitdat.Scan0.ToPointer();
            int position = 0;
            for (int currentY = 0; currentY < height; currentY += 8) //Each pixel row
            {
                for (int currentX = 0; currentX < width; currentX += 8) //Each pixel column
                {
                    for (int ty = 0; ty < 8; ty++)
                    {
                        for (int tx = 0; tx < 8; tx++, position++)
                        {
                            byte focus;
                            try
                            {
                                focus = graphics[position];
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Exception in Read4BBP");
                                img.UnlockBits(bitdat);
                                return img;
                            }
                            byte pixil2 = (byte)((uint)focus >> 4);
                            byte pixil1 = (byte)((uint)focus & 0x0F);
                            if (pixil1 == 0) { *(pointer + currentX + ((currentY + ty) * width) + tx) = 0x00000000; }
                            else
                            {
                                *(pointer + currentX + ((currentY + ty) * width) + tx) = palette[pixil1];
                            }
                            tx++;
                            if (pixil2 == 0) { *(pointer + currentX + ((currentY + ty) * width) + tx) = 0x00000000; }
                            else
                            {
                                *(pointer + currentX + ((currentY + ty) * width) + tx) = palette[pixil2];
                            }
                        }
                    }
                }
            }
            img.UnlockBits(bitdat);
            return img;
        }

        public unsafe int getint32(byte[] ROM, int offset)
        {
            fixed (byte* pointer = &ROM[offset])
            {
                return *(int*)pointer;
            }
        }

        private Rectangle getsize(byte[] spritefile, int offset, int animation, int frame)
        {
            int animationoffset = offset + getint32(spritefile, offset + (4 * animation));
            int opointer = offset + getint32(spritefile, animationoffset + (5 * 4 * frame) + 4 + 4 + 4);

            int num = getint32(spritefile, opointer);
            int minx = 0,
                maxx = 0,
                miny = 0,
                maxy = 0;
            while ((uint)getint32(spritefile, opointer + num) != 0xFFFFFFFF)
            {
                sbyte xpos = (sbyte)spritefile[opointer + (num) + 1];
                sbyte ypos = (sbyte)spritefile[opointer + (num) + 2];
                byte flip = spritefile[opointer + (num) + 3];
                byte multiplier = spritefile[opointer + (num) + 4];
                Size section = new Size(0, 0);
                byte sizes = (byte)(flip & 0x0F);
                switch (sizes)
                {
                    #region modifysize
                    case 0x00:
                        section.Width = 8;
                        section.Height = 8;
                        break;
                    case 0x01:
                        section.Width = 16;
                        section.Height = 16;
                        break;
                    case 0x02:

                        section.Width = 32;
                        section.Height = 32;
                        break;
                    case 0x03:
                        section.Width = 64;
                        section.Height = 64;
                        break;
                    default:
                        section.Width = 8;
                        section.Height = 8;
                        break;
                }
                switch (multiplier & 0x0F)
                {
                    case 1:
                        switch (sizes)
                        {
                            case 0:
                                section.Width *= 2;
                                break;
                            case 1:
                                section.Width *= 2;
                                section.Height /= 2;
                                break;
                            default:
                                section.Height /= 2;
                                break;
                        }
                        break;
                    case 2:
                        switch (sizes)
                        {
                            case 0:
                                section.Height *= 2;
                                break;
                            case 1:
                                section.Width /= 2;
                                section.Height *= 2;
                                break;
                            default:
                                section.Width /= 2;
                                break;
                        }
                        break;
                        #endregion
                }
                if (minx > xpos)
                {
                    minx = xpos;
                }
                if (maxx < xpos + section.Width)
                {
                    maxx = xpos + section.Width;
                }
                if (miny > ypos)
                {
                    miny = ypos;
                }
                if (maxy < ypos + section.Height)
                {
                    maxy = ypos + section.Height;
                }
                num += 5;
            }
            return new Rectangle(minx, miny, maxx - minx, maxy - miny);
        }

        private Bitmap DrawSprite(Frame frame)
        {
            Bitmap picture = new Bitmap(256, 256);
            Palette palette = ActiveBNSA.OriginalPalettes[Convert.ToInt32(paletteIndexUpDown.Value)];
            OAMDataListGroup frameData = frame.ResolvedOAMDataListGroup;
            int[] paletteData = paletteData = getUsablePaletteColors(palette.Memory);



            //int animationoffset = offset + getint32(spritefile, offset + (4 * animationIndex));
            //int gpointer = offset + getint32(spritefile, animationoffset + (5 * 4 * frameIndex));
            //int ppointer = offset + getint32(spritefile, animationoffset + (5 * 4 * frameIndex) + 4) + (int)(numericUpDown3.Value * 0x20);
            UpdatePalette(paletteData);
            //int opointer = offset + getint32(spritefile, animationoffset + (5 * 4 * frameIndex) + 4 + 4 + 4); //Pointer to OAM Data
            UpdateInfoBox(frame);
            Graphics g = Graphics.FromImage(picture);
            //int num = getint32(spritefile, opointer + subFrameIndex * 4);

            int i = 0;
            foreach (OAMDataListEntry entry in frameData.OAMDataLists[0].OAMDataListEntries)
            {

                //Draw Sprite into Graphics
                //Bitmap part = Read4BBP(entry.ObjectWidth, entry.ObjectHeight, frame.ResolvedTileset.Memory, paletteData);
                byte[] tileData = getTile(frame.ResolvedTileset.Memory, entry.TileNumber, entry.ObjectWidth, entry.ObjectHeight);
                Bitmap tileBitMap = Read4BBP(entry.ObjectWidth, entry.ObjectHeight, tileData, paletteData);

                if (entry.HorizontalFlip & entry.VerticalFlip)
                {
                    tileBitMap.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                }
                else if (entry.HorizontalFlip)
                {
                    tileBitMap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
                else if (entry.VerticalFlip)
                {
                    tileBitMap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                }
                Point drawPos = new Point((picture.Width / 2) + entry.X, (picture.Height / 2) + entry.Y);
                //Console.WriteLine("Drawing OAM " + i + " w TN " + entry.TileNumber + ": " + drawPos.X + "," + drawPos.Y);
                g.DrawImageUnscaled(tileBitMap, drawPos);
                i++;
            }

            //Scale image
            int scale = Decimal.ToInt32(drawScaleUpDown.Value);
            Bitmap returnpicture = resizeImage(picture, scale * picture.Width, scale * picture.Height);
            //Graphics finalgraphics = Graphics.FromImage(returnpicture);
            //finalgraphics.DrawImage(new Bitmap(256, 256, g), returnpicture.Width, returnpicture.Height);

            //g.DrawRectangle(Pens.Black, new Rectangle(0, 0, picture.Width-1, picture.Height-1));
            return returnpicture;
        }

        public static Bitmap resizeImage(Bitmap image, int new_height, int new_width)
        {
            Bitmap new_image = new Bitmap(new_width, new_height);
            Graphics g = Graphics.FromImage((Image)new_image);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(image, 0, 0, new_width, new_height);
            return new_image;
        }

        //}

        //    while (spritefile[opointer + (num)] != 0xFF)
        //    {
        //        //OAM Data List Entry
        //        byte tile = spritefile[opointer + (num)];
        //        sbyte xpos = (sbyte)spritefile[opointer + (num) + 1];
        //        sbyte ypos = (sbyte)spritefile[opointer + (num) + 2];
        //        byte flip = spritefile[opointer + (num) + 3];
        //        byte multiplier = spritefile[opointer + (num) + 4];
        //        Size section = new Size(0, 0);
        //        byte sizes = (byte)(flip & 0x0F);
        //        switch (sizes)
        //        {
        //            #region modifysize
        //            case 0x00:
        //                section.Width = 8;
        //                section.Height = 8;
        //                break;
        //            case 0x01:
        //                section.Width = 16;
        //                section.Height = 16;
        //                break;
        //            case 0x02:

        //                section.Width = 32;
        //                section.Height = 32;
        //                break;
        //            case 0x03:
        //                section.Width = 64;
        //                section.Height = 64;
        //                break;
        //            default:
        //                section.Width = 8;
        //                section.Height = 8;
        //                break;
        //        }
        //        switch (multiplier & 0x0F)
        //        {
        //            case 1:
        //                switch (sizes)
        //                {
        //                    case 0:
        //                        section.Width *= 2;
        //                        break;
        //                    case 1:
        //                        section.Width *= 2;
        //                        section.Height /= 2;
        //                        break;
        //                    default:
        //                        section.Height /= 2;
        //                        break;
        //                }
        //                break;
        //            case 2:
        //                switch (sizes)
        //                {
        //                    case 0:
        //                        section.Height *= 2;
        //                        break;
        //                    case 1:
        //                        section.Width /= 2;
        //                        section.Height *= 2;
        //                        break;
        //                    default:
        //                        section.Width /= 2;
        //                        break;
        //                }
        //                break;
        //                #endregion

        //        //}
        //        Bitmap part = Read4BBP(section.Width, section.Height, getgraphics(spritefile, gpointer + (0x20 * tile) + 4, section.Width, section.Height), getpalette(spritefile, ppointer + ((multiplier >> 4) * 0x20) + 4));
        //        byte rotation = (byte)(flip & 0xF0);
        //        switch (rotation)
        //        {
        //            case 0x40:
        //                part.RotateFlip(RotateFlipType.RotateNoneFlipX);
        //                break;
        //            case 0x80:
        //                part.RotateFlip(RotateFlipType.RotateNoneFlipY);
        //                break;
        //            case 0xC0:
        //                part.RotateFlip(RotateFlipType.RotateNoneFlipXY);
        //                break;
        //        }
        //        //g.DrawImageUnscaled(part, new Point((picture.Width  /2) + finalx, (picture.Height / 2) + finaly));
        //        //g.DrawRectangle(Pens.Red, new Rectangle(128, 128, 1, 1));
        //        g.DrawImageUnscaled(part, new Point((picture.Width / 2) + xpos, (picture.Height / 2) + ypos));
        //        num += 5;
        //    }
        //    //Scale image
        //    int scale = Decimal.ToInt32(drawScaleUpDown.Value);
        //    Bitmap returnpicture = new Bitmap(256 * scale, 256 * scale);
        //    Graphics finalgraphics = Graphics.FromImage(returnpicture);
        //    finalgraphics.DrawImage(new Bitmap(256, 256, g), returnpicture.Width, returnpicture.Height);

        //    //g.DrawRectangle(Pens.Black, new Rectangle(0, 0, picture.Width-1, picture.Height-1));
        //    return picture;

        private void UpdateInfoBox(Frame frame)
        {
            infoListbox.Items.Clear();

            infoListbox.Items.Add("Current Frame Info:");
            //infoListbox.Items.Add("Start of Sprite File:\t0x" + offset.ToString("X8"));
            //infoListbox.Items.Add("Animation Pointer:\t0x" + animationpointer.ToString("X8"));
            infoListbox.Items.Add("Tileset Index:\t" + frame.ResolvedTileset.Index);
            infoListbox.Items.Add("Tileset Pointer:\t0x" + frame.ResolvedTileset.Pointer.ToString("X8"));
            infoListbox.Items.Add("# of Tiles Used:\t" + frame.ResolvedOAMDataListGroup.OAMDataLists[0].OAMDataListEntries.Count);

            //infoListbox.Items.Add("Palette Pointer:\t0x" + ppointer.ToString("X8"));

            infoListbox.Items.Add("OAM Data List Idx:\t" + frame.ResolvedOAMDataListGroup.Index);
            infoListbox.Items.Add("OAM Data List Ptr:\t0x" + frame.ResolvedOAMDataListGroup.OAMDataLists[0].Pointer.ToString("X8"));

        }


        private static string GetROMID(byte[] ROM)
        {
            string ID = "";
            for (int i = 0; i < 4; i++)
            {
                ID += (char)ROM[0xAC + i];
            }
            return ID;
        }
        private void openROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "MEGAMAN BN ROM|*.GBA";
            if (open.ShowDialog() != DialogResult.Cancel)
                OpenROM(open.FileName);
        }

        private void OpenROM(string open)
        {
            romSpritePointersListbox.Items.Clear();
            ROM = File.ReadAllBytes(open);
            string ROMID = GetROMID(ROM);
            List<string> lbi = new List<string>();
            startoffset = 0;
            endoffset = 0;
            if (BN6USROMCODES.Contains(ROMID))
            {
                startoffset = 0x031CEC;
                endoffset = 0x0329A8;

            }
            else if (BN6JPROMCODES.Contains(ROMID))
            {
                startoffset = 0x32CA8;
                endoffset = 0x33968;

            }
            else if (BN5TPUSROMCODES.Contains(ROMID))
            {
                startoffset = 0x32750;
                endoffset = 0x331B0;

            }
            else if (BN5TCUSROMCODES.Contains(ROMID))
            {
                startoffset = 0x32754;
                endoffset = 0x331B4;


            }
            else if (BN5TPJPROMCODES.Contains(ROMID))
            {
                startoffset = 0x326E8;
                endoffset = 0x33148;


            }
            else if (BN5TCJPROMCODES.Contains(ROMID))
            {
                startoffset = 0x326EC;
                endoffset = 0x3314C;


            }
            else if (BN45JPROMCODES.Contains(ROMID))
            {
                startoffset = 0x2B39C;
                endoffset = 0x2BC7C;

            }
            else if (BN4BUSROMCODES.Contains(ROMID))
            {
                startoffset = 0x27968;
                endoffset = 0x28308;

            }
            else if (BN4RUSROMCODES.Contains(ROMID))
            {
                startoffset = 0x27964;
                endoffset = 0x28304;


            }
            else if (BN4BJPROMCODES.Contains(ROMID))
            {
                startoffset = 0x27880;
                endoffset = 0x28220;

            }
            else if (BN4RJPROMCODES.Contains(ROMID))
            {
                startoffset = 0x2787C;
                endoffset = 0x2821C;

            }
            else if (BN3WUSROMCODES.Contains(ROMID))
            {
                startoffset = 0x247A0;
                endoffset = 0x25474;


            }
            else if (BN3BUSROMCODES.Contains(ROMID))
            {
                startoffset = 0x24788;
                endoffset = 0x2545C;

            }
            else if (BN3WJPROMCODES.Contains(ROMID))
            {
                startoffset = 0x248F8;
                endoffset = 0x251CC;

            }
            else if (BN3BJPROMCODES.Contains(ROMID))
            {
                startoffset = 0x248e0;
                endoffset = 0x251bc;


            }
            else if (BN2USROMCODES.Contains(ROMID))
            {
                startoffset = 0x1E9FC;
                endoffset = 0x1F1D0;


            }
            else if (BN2JPROMCODES.Contains(ROMID))
            {
                startoffset = 0x1E888;
                endoffset = 0x1F05C;

            }
            else if (BN1USROMCODES.Contains(ROMID))
            {
                startoffset = 0x12690;
                endoffset = 0x12BF0;

            }
            else if (BN1EUROMCODES.Contains(ROMID))
            {
                startoffset = 0x1269C;
                endoffset = 0x12BFC;

            }
            else if (BN1JPROMCODES.Contains(ROMID))
            {
                startoffset = 0x12614;
                endoffset = 0x12B74;

            }
            for (int i = startoffset; i < endoffset; i += 4)
            {
                lbi.Add(getint32(ROM, i).ToString("X8"));
            }
            romSpritePointersListbox.Items.AddRange(lbi.ToArray());
            romSpritePointersListbox.SelectedIndex = 0;
            filename = open;
            this.Text = "Megaman BattleNetwork Sprite List Viewer ~ By Greiga Master (" + Path.GetFileName(filename) + ")";
        }

        private void romSpritePointersListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (romSpritePointersListbox.SelectedIndex == -1)
            {
                return;
            }

            animationIndexUpDown.Value = 0;
            frameIndexUpDown.Value = 0;
            paletteIndexUpDown.Value = 0;

            if (romSpritePointersListbox.SelectedItem.ToString().Substring(0, 2) == "88")
            {
                //Archive is compressed
                BNSAMemory = Nintenlord.GBA.Compressions.LZ77.Decompress(ROM, (int.Parse((string)romSpritePointersListbox.SelectedItem, System.Globalization.NumberStyles.HexNumber) & 0x00FFFFFF));
                Array.Copy(BNSAMemory, 4, BNSAMemory, 0, BNSAMemory.Length - 4);
                offset = 8;
            }
            else
            {
                //Not compressed
                //Find the end of the BNSA.
                offset = (int.Parse((string)romSpritePointersListbox.SelectedItem, System.Globalization.NumberStyles.HexNumber) & 0x00FFFFFF) + 4;
                int numAnimations = 0;
                if (getint32(ROM, offset) / 4 != 0)
                {
                    numAnimations = (getint32(ROM, offset) / 4) - 1;
                }
                else
                {
                    numAnimations = (getint32(ROM, offset) / 4);
                }

                int startoffset = offset - 4;
                int framecount = getmaxframes(numAnimations);
                int animationoffset = offset + getint32(ROM, offset + (4 * numAnimations));
                int opointer = offset + getint32(ROM, animationoffset + (5 * 4 * framecount) + 4 + 4 + 4);
                int num = getint32(ROM, opointer);
                num = getint32(ROM, opointer + num - 4);
                while ((uint)getint32(ROM, (opointer + num)) != 0xFFFFFFFF)
                {
                    num += 5;
                }
                num += 5;
                BNSAMemory = new byte[(opointer + num) - startoffset];
                Array.Copy(ROM, startoffset, BNSAMemory, 0, (opointer + num) - startoffset);
            }
            File.WriteAllBytes(@"C:\users\michael\bnsa\bnsabytes.bnsa",BNSAMemory);
            ActiveBNSA = new BNSAFile(BNSAMemory);
            ActiveBNSA.ResolveReferences();
            UpdateImage();
            //    offset = (int.Parse((string)romSpritePointersListbox.SelectedItem, System.Globalization.NumberStyles.HexNumber) & 0x00FFFFFF) + 4;



            ////Update bnsa file in memory

            //UpdateImage();
            //numericUpDown1_ValueChanged(null, null);
        }

        //Updates the Palette Box
        private void UpdatePalette(int[] paletteBinary)
        {
            Bitmap bit = new Bitmap(8 * 16, 8);
            Graphics g = Graphics.FromImage(bit);
            for (int i = 0; i < 16; i++)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(paletteBinary[i])), new Rectangle(i * 8, 0, 8, 8));
            }
            pictureBox2.Image = bit;
        }

        /// <summary>
        /// Updates the displayed image by redrawing the sprite
        /// </summary>
        private void UpdateImage()
        {
            if (ActiveBNSA == null)
            {
                return;
            }
            int animationIndex = Convert.ToInt32(animationIndexUpDown.Value);
            int frameIndex = Convert.ToInt32(frameIndexUpDown.Value);
            Frame frame = ActiveBNSA.Animations[animationIndex].Frames[frameIndex];
            pictureBox1.Image = DrawSprite(frame);
            //Do not remove! Huge memory spike if you do. Should probably cache the sprites at their current drawing level.
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void animationIndexUpDown_Changed(object sender, EventArgs e)
        {
            frameIndexUpDown.Value = 0;
            //paletteIndexUpDown.Value = 0;
            frameIndexUpDown.Maximum = ActiveBNSA.Animations[(int)animationIndexUpDown.Value].Frames.Count - 1;
            animateCheckbox.Enabled = ActiveBNSA.Animations[(int)animationIndexUpDown.Value].Frames.Count - 1 > 0;
            UpdateImage();
        }

        private int getmaxframes(int animation)
        {
            int frame = 0;
            if (romSpritePointersListbox.SelectedItem.ToString().Substring(0, 2) == "88")
            {
                byte[] spritefile = Nintenlord.GBA.Compressions.LZ77.Decompress(ROM, (int.Parse((string)romSpritePointersListbox.SelectedItem, System.Globalization.NumberStyles.HexNumber) & 0x00FFFFFF));
                int offset = 8;
                while (true)
                {
                    int somedata = (getint32(spritefile, offset + getint32(spritefile, offset + (4 * (int)animation)) + (5 * 4 * frame) + 4 + 4 + 4 + 4));
                    if (((somedata & 0x00FF0000) >> 16) == 0xC0 || ((somedata & 0x00FF0000) >> 16) == 0x80 || ((somedata & 0x000000FF) == 0x01 && ((somedata & 0x00FF0000) >> 16) == 0x08))
                    {
                        break;
                    }
                    frame++;
                }
            }
            else
            {
                byte[] memory = ROM;
                int offset = (int.Parse((string)romSpritePointersListbox.SelectedItem, System.Globalization.NumberStyles.HexNumber) & 0x00FFFFFF) + 4;
                while (true)
                {
                    int somedata = (getint32(memory, offset + getint32(memory, offset + (4 * (int)animation)) + (5 * 4 * frame) + 4 + 4 + 4 + 4));
                    if (((somedata & 0x00FF0000) >> 16) == 0xC0 || ((somedata & 0x00FF0000) >> 16) == 0x80 || ((somedata & 0x000000FF) == 0x01 && ((somedata & 0x00FF0000) >> 16) == 0x08))
                    {
                        break;
                    }
                    frame++;
                }
            }
            return frame;
        }

        private void frameIndexUpDown_Changed(object sender, EventArgs e)
        {
            UpdateImage();
        }

        private void paletteUpDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateImage();
        }

        private void dumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Battle Network Sprite Archive |*.bnsa";
            if (save.ShowDialog() != DialogResult.Cancel)
            {
                bool wpadding = true;
                //Always dump with padding.

                //if (MessageBox.Show("Do you wish to dump this with padding?", "Hold It!!!", MessageBoxButtons.YesNo) == DialogResult.No)
                //{
                //    wpadding = false;
                //}
                if (romSpritePointersListbox.SelectedItem.ToString().Substring(0, 2) == "88")
                {
                    byte[] spritefile = Nintenlord.GBA.Compressions.LZ77.Decompress(ROM, (int.Parse((string)romSpritePointersListbox.SelectedItem, System.Globalization.NumberStyles.HexNumber) & 0x00FFFFFF));
                    if (wpadding)
                    {
                        byte[] tosave = new byte[spritefile.Length - 4];
                        Array.Copy(spritefile, 4, tosave, 0, tosave.Length);
                        File.WriteAllBytes(save.FileName, tosave);
                    }
                    else
                    {
                        byte[] tosave = new byte[spritefile.Length - 8];
                        Array.Copy(spritefile, 8, tosave, 0, tosave.Length);
                        File.WriteAllBytes(save.FileName, tosave);
                    }
                }
                else
                {
                    //int offset = (int.Parse((string)listBox1.SelectedItem, System.Globalization.NumberStyles.HexNumber) & 0x00FFFFFF);
                    int startoffset = offset - 4;
                    int framecount = getmaxframes((int)animationIndexUpDown.Maximum);
                    int animationoffset = offset + getint32(ROM, offset + (4 * (int)animationIndexUpDown.Maximum));
                    int opointer = offset + getint32(ROM, animationoffset + (5 * 4 * framecount) + 4 + 4 + 4);
                    int num = getint32(ROM, opointer);
                    num = getint32(ROM, opointer + num - 4);
                    while ((uint)getint32(ROM, (opointer + num)) != 0xFFFFFFFF)
                    {
                        num += 5;
                    }
                    num += 5;
                    byte[] tosave = new byte[(opointer + num) - startoffset];
                    Array.Copy(ROM, startoffset + (!wpadding ? 4 : 0), tosave, 0, (opointer + num) - startoffset);
                    File.WriteAllBytes(save.FileName, tosave);
                }
            }
        }

        private void createSpriteStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "PNGIMAGE |*.png";
            if (save.ShowDialog() != DialogResult.Cancel)
            {
                bool multiline = true;
                if (MessageBox.Show("Do you want a new line for each animation?", "Hold It!!!", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    multiline = false;
                }
                List<Bitmap> frames = new List<Bitmap>();
                byte[] source;
                int offset;
                if (romSpritePointersListbox.SelectedItem.ToString().Substring(0, 2) == "88")
                {
                    source = Nintenlord.GBA.Compressions.LZ77.Decompress(ROM, (int.Parse((string)romSpritePointersListbox.SelectedItem, System.Globalization.NumberStyles.HexNumber) & 0x00FFFFFF));
                    offset = 8;
                }
                else
                {
                    offset = (int.Parse(romSpritePointersListbox.SelectedItem.ToString(), System.Globalization.NumberStyles.HexNumber) & 0x00FFFFFF) + 4;
                    source = ROM;
                }
                List<int> breaks = new List<int>();
                int realframecount = 0;
                Rectangle r = new Rectangle(int.MaxValue, int.MaxValue, 0, 0);
                int len1 = 0,
                    len2 = 0;
                for (int i = 0; i < (getint32(source, offset) / 4); i++)
                {
                    int frame = 0;
                    while (true)
                    {
                        Rectangle rec = getsize(source, offset, i, frame);
                        if (rec.Width > r.Width)
                        {
                            r.Width = rec.Width;
                        }
                        if (rec.Height > r.Height)
                        {
                            r.Height = rec.Height;
                            r.Height += 1;
                        }
                        if (r.Y > rec.Y)
                        {
                            r.Y = rec.Y;
                        }
                        int somedata = (getint32(source, offset + getint32(source, offset + (4 * i)) + (5 * 4 * frame) + 4 + 4 + 4 + 4));
                        if (((somedata & 0x00FF0000) >> 16) == 0xC0 || ((somedata & 0x00FF0000) >> 16) == 0x80 || ((somedata & 0x000000FF) == 0x01 && ((somedata & 0x00FF0000) >> 16) == 0x08))
                        {
                            frame++;
                            break;
                        }
                        frame++;
                    }
                    frame = 0;
                    while (true)
                    {
                        Rectangle rec = getsize(source, offset, i, frame);
                        if (r.X > rec.X)
                        {
                            r.X = rec.X;
                            if (r.Width + r.X > len1)
                            {
                                len1 = r.Width + r.X;
                            }
                            if (-r.X - r.X > len2)
                            {
                                len2 = -r.X - r.X;
                            }
                        }
                        int somedata = (getint32(source, offset + getint32(source, offset + (4 * i)) + (5 * 4 * frame) + 4 + 4 + 4 + 4));
                        if (((somedata & 0x00FF0000) >> 16) == 0xC0 || ((somedata & 0x00FF0000) >> 16) == 0x80 || ((somedata & 0x000000FF) == 0x01 && ((somedata & 0x00FF0000) >> 16) == 0x08))
                        {
                            frame++;
                            break;
                        }
                        frame++;
                    }
                }
                r.X = -((len1 + len2) / 3);
                r.Width -= (r.X);
                r.Width += (r.X / 5) * 3;
                GetFrames(frames, source, offset, breaks, ref realframecount);
                string filename = save.FileName.Substring(0, save.FileName.Length - 4) + " FC[" + realframecount.ToString() + "] AC[" + (getint32(source, offset) / 4).ToString() + "]" + " W[" + r.Width.ToString() + "] H[" + r.Height.ToString() + "] .png";
                ToAnimationStrip(frames, multiline, breaks, false).Save(filename);
            }
        }

        public void GetFrames(List<Bitmap> frames, byte[] source, int offset, List<int> breaks, ref int realframecount)
        {
            throw new NotImplementedException("Not implemented in BNSA");
            for (int i = 0; i < (getint32(source, offset) / 4); i++)
            {
                int frame = 0;
                while (true)
                {
                    //       Rectangle rec = getsize(source, offset, i, frame);
                    //frames.Add(DrawSprite(source, offset, i, frame, 0));
                    int somedata = (getint32(source, offset + getint32(source, offset + (4 * i)) + (5 * 4 * frame) + 4 + 4 + 4 + 4));
                    if (((somedata & 0x00FF0000) >> 16) == 0xC0 || ((somedata & 0x00FF0000) >> 16) == 0x80 || ((somedata & 0x000000FF) == 0x01 && ((somedata & 0x00FF0000) >> 16) == 0x08))
                    {
                        frame++;
                        realframecount++;
                        break;
                    }
                    frame++;
                    realframecount++;
                }
                breaks.Add(realframecount);
            }
        }
        public Bitmap ToAnimationStrip(List<Bitmap> fms, bool usebreaks, List<int> breaks, bool box)
        {
            int width = fms[0].Width,
                height = fms[0].Height;

            int mostwidths = 0;
            int xadd = 0,
                yadd = 0;
            for (int i = 0; i < fms.Count; i++)
            {
                if (breaks.Contains(i) && usebreaks)
                {
                    yadd++;
                    xadd = 0;
                }
                xadd++;
                if (xadd > mostwidths)
                {
                    mostwidths = xadd;
                }
            }
            Bitmap tosave = new Bitmap(width * mostwidths, height * (usebreaks ? breaks.Count : 1));
            Graphics g = Graphics.FromImage(tosave);
            if (box)
            {
                int num = 0;
                for (int y = 0; y < tosave.Height; y += height)
                {
                    for (int x = 0; x < tosave.Width; x += width)
                    {
                        g.DrawString(num.ToString(), this.Font, Brushes.Black, new Point(x, y));
                        g.DrawLine(new Pen(Color.FromArgb(255, 0, 0)), new Point(x, 0), new Point(x, tosave.Height));
                        g.DrawLine(new Pen(Color.FromArgb(255, 0, 0)), new Point(0, y), new Point(tosave.Width, y));
                        num++;
                    }
                }
            }
            yadd = 0;
            xadd = 0;
            for (int i = 0; i < fms.Count; i++)
            {
                if (breaks.Contains(i) && usebreaks)
                {
                    yadd++;
                    xadd = 0;
                }
                g.DrawImageUnscaled(fms[i], new Point(width * xadd, height * yadd));
                xadd++;
            }

            return tosave;
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int bottom = 0;
            for (int i = ROM.Length - 1; i >= 0; i--)
            {
                if (ROM[i] != 0xFF)
                {
                    bottom = i;
                    while (bottom % 4 != 0)
                    {
                        bottom++;
                    }
                    break;
                }
            }
            Replace_Settings rs = new Replace_Settings(bottom);
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "Battle Network Sprite Archive|*.bnsa";
            if (op.ShowDialog() != DialogResult.Cancel)
            {
                List<byte> file = new List<byte>(File.ReadAllBytes(op.FileName));
                if (rs.ShowDialog() != DialogResult.Cancel)
                {
                    if (rs.compress)
                    {
                        unchecked
                        {
                            int pos = (romSpritePointersListbox.SelectedIndex * 4) + startoffset;
                            if (!rs.hasheader)
                            {
                                file.InsertRange(0, new byte[] { 0, 0, 0, 0 });
                            }

                            file.InsertRange(0, BitConverter.GetBytes(file.Count << 4));
                            file = new List<byte>(Nintenlord.GBA.Compressions.LZ77.Compress(file.ToArray()));
                            Array.Copy(file.ToArray(), 0, ROM, rs.returnoffset, file.Count);
                            writeint32(ROM, pos, (int)0x88000000 + (rs.returnoffset));
                            romSpritePointersListbox.SelectedValue = (0x88000000 + (rs.returnoffset)).ToString("X8");
                            romSpritePointersListbox.Items[romSpritePointersListbox.SelectedIndex] = (0x88000000 + (rs.returnoffset)).ToString("X8");
                        }
                    }
                    else
                    {
                        int pos = (romSpritePointersListbox.SelectedIndex * 4) + startoffset;
                        Array.Copy(file.ToArray(), 0, ROM, rs.returnoffset, file.Count);
                        writeint32(ROM, pos, 0x08000000 + (rs.returnoffset - (!rs.hasheader ? 4 : 0)));
                        romSpritePointersListbox.SelectedValue = (0x08000000 + (rs.returnoffset - (!rs.hasheader ? 4 : 0))).ToString("X8");
                        romSpritePointersListbox.Items[romSpritePointersListbox.SelectedIndex] = (0x08000000 + (rs.returnoffset - (!rs.hasheader ? 4 : 0))).ToString("X8");
                    }
                }
                UpdateImage();
            }
        }
        private unsafe static void writeint32(byte[] ROM, int offset, int val)
        {
            fixed (byte* pointer = &ROM[offset])
            {
                *(int*)pointer = val;
            }
        }
        private void exportCurrentFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "PNG IMAGE |*.PNG";
            if (save.ShowDialog() != DialogResult.Cancel)
            {
                //int offset = (int.Parse(listBox1.SelectedItem.ToString(), System.Globalization.NumberStyles.HexNumber) & 0x00FFFFFF) + 4;
                Bitmap b = DrawSprite(ActiveBNSA.Animations[(int)animationIndexUpDown.Value].Frames[(int)frameIndexUpDown.Value]);
                int padding = 2;
                //Crop
                var pixelsX = Enumerable.Range(0, b.Width);
                var pixelsY = Enumerable.Range(0, b.Height);

                int localleft = pixelsX.FirstOrDefault(
                            x => pixelsY.Any(y => b.GetPixel(x, y).A != 0));
                int localright = pixelsX.Reverse().FirstOrDefault(
                            x => pixelsY.Any(y => b.GetPixel(x, y).A != 0));
                int localtop = pixelsY.FirstOrDefault(
                            y => pixelsX.Any(x => b.GetPixel(x, y).A != 0));
                int localbottom = pixelsY.Reverse().FirstOrDefault(
                            y => pixelsX.Any(x => b.GetPixel(x, y).A != 0));

                int width = localright - localleft + padding * 2;
                int height = localbottom - localtop + padding * 2;
                b = CropBitmap(b, localleft - padding, localtop - padding, width, height);
                b.Save(save.FileName);
            }
        }


        private void searchlist(bool foward = true)
        {
            if (lastsearched == "")
            {
                return;
            }

            if (foward)
            {
                for (int i = romSpritePointersListbox.SelectedIndex + 1; i < romSpritePointersListbox.Items.Count; i++)
                {
                    if (romSpritePointersListbox.Items[i].ToString().ToUpper().Contains(lastsearched))
                    {
                        romSpritePointersListbox.SelectedIndex = i;
                        return;
                    }
                }
            }
            else
            {

                for (int i = romSpritePointersListbox.SelectedIndex - 1; i >= 0; i--)
                {
                    if (romSpritePointersListbox.Items[i].ToString().ToUpper().Contains(lastsearched))
                    {
                        romSpritePointersListbox.SelectedIndex = i;
                        return;
                    }
                }
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            File.WriteAllBytes(filename, ROM);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "GBA ROM |*.GBA";
            if (save.ShowDialog() != DialogResult.Cancel)
            {
                filename = save.FileName;
                saveAsToolStripMenuItem_Click(null, null);
            }
        }

        private void theRockmanEXEZoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://trezforums.com");
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() != DialogResult.Cancel)
            {
                pictureBox1.BackColor = cd.Color;
            }
        }

        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            searchlist();
        }

        private void previousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            searchlist(false);
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Search s = new Search();
            if (s.ShowDialog() == DialogResult.OK)
            {
                lastsearched = s.returnvalue.ToUpper();
                searchlist();
            }
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            About a = new About();
            a.ShowDialog();
        }

        private void romOffsetCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateImage();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OpenROM(@"E:\Google Drive\GBA-2\ASM\WhiteVanilla.gba");
            //OpenBNSA(@"E:\Google Drive\GBA-2\ASM\spritearchives\bunny.bnsa");
            paletteIndexUpDown.Maximum = ActiveBNSA.OriginalPalettes.Count - 1;
            spriteanimator.RunWorkerAsync();
        }




        private void manuelExportSpriteSheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Manuel_Export me = new Manuel_Export(this);
            me.ShowDialog();
        }

        private void spriteanimator_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int animationIndex = Convert.ToInt32(animationIndexUpDown.Value);
            if (ActiveBNSA.Animations[animationIndex].Frames.Count <= 1)
            {
                return; //theres nothing to animate.
            }

            while (animateCheckbox.Checked)
            {
                
                for (int i = 0; i <= (int)frameIndexUpDown.Maximum; i++)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        frameIndexUpDown.Value = i; //Runs on UI Thread
                    });
                    int delay = ActiveBNSA.Animations[Convert.ToInt32(animationIndexUpDown.Value)].Frames[i].FrameDelay;
                    Thread.Sleep((1000 * delay) / 60);
                }
            }

        }

        private void animateCheckboxx_CheckedChanged(object sender, EventArgs e)
        {
            if (!animateCheckbox.Checked)
            {
                spriteanimator.CancelAsync();
            }
            else
            {
                spriteanimator.RunWorkerAsync();
            }
        }

        private void repointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Search replacedialog = new Search();
            replacedialog.Text = "Repoint to";
            if (replacedialog.ShowDialog() != DialogResult.Cancel)
            {
                unchecked
                {
                    int returnoffset = int.Parse(replacedialog.returnvalue, System.Globalization.NumberStyles.HexNumber);
                    int pos = (romSpritePointersListbox.SelectedIndex * 4) + startoffset;
                    writeint32(ROM, pos, (returnoffset));
                    romSpritePointersListbox.SelectedValue = ((returnoffset)).ToString("X8");
                    romSpritePointersListbox.Items[romSpritePointersListbox.SelectedIndex] = ((returnoffset)).ToString("X8");
                }
            }
        }

        private void getCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int indexoffset = startoffset + (romSpritePointersListbox.SelectedIndex * 4) + 0x08000000;
            int catigory = 0;
            for (int i = 0x031CC4; true; i += 4)
            {
                if (getint32(ROM, i) > indexoffset)
                {
                    catigory = i - 4;
                    break;
                }
                else if (getint32(ROM, i) == indexoffset)
                {
                    catigory = i;
                    break;
                }
            }
            catigory -= 0x031CC4;
            int id = (indexoffset - getint32(ROM, 0x031CC4 + catigory)) / 4;
            MessageBox.Show("Catigory: " + catigory.ToString("X2") + "\r\n" + "ID: " + id.ToString("X2"));
        }

        private unsafe void exportStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Bitmap> frames = new List<Bitmap>();
            bool multiline = true;
            if (MessageBox.Show("Do you want a new line for each animation?", "Hold It!!!", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                multiline = false;
            }
            byte[] source;
            int offset;
            if (romSpritePointersListbox.SelectedItem.ToString().Substring(0, 2) == "88")
            {
                source = Nintenlord.GBA.Compressions.LZ77.Decompress(ROM, (int.Parse((string)romSpritePointersListbox.SelectedItem, System.Globalization.NumberStyles.HexNumber) & 0x00FFFFFF));
                offset = 8;
            }
            else
            {
                offset = (int.Parse(romSpritePointersListbox.SelectedItem.ToString(), System.Globalization.NumberStyles.HexNumber) & 0x00FFFFFF) + 4;
                source = ROM;
            }
            List<int> breaks = new List<int>();
            int realframecount = 0;
            int top = 256,
    left = 256,
    right = 0,
    bottom = 0;
            for (int i = 0; i < (getint32(source, offset) / 4); i++)
            {
                int frame = 0;
                while (true)
                {
                    int somedata = (getint32(source, offset + getint32(source, offset + (4 * i)) + (5 * 4 * frame) + 4 + 4 + 4 + 4));
                    if (((somedata & 0x00FF0000) >> 16) == 0xC0 || ((somedata & 0x00FF0000) >> 16) == 0x80 || ((somedata & 0x000000FF) == 0x01 && ((somedata & 0x00FF0000) >> 16) == 0x08))
                    {
                        frame++;
                        break;
                    }
                    frame++;
                }
            }
            GetFrames(frames, source, offset, breaks, ref realframecount);
            for (int f = 0; f < frames.Count; f++)
            {
                BitmapData bdat = frames[f].LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                int* pointer = (int*)bdat.Scan0;
                for (int h = 0; h < frames[f].Height; h++)
                {
                    for (int w = 0; w < frames[f].Width; w++)
                    {
                        if (*(pointer + (h * frames[f].Width) + w) != 0)
                        {
                            if (h < top) top = h;
                            if (h > bottom) bottom = h;
                            if (left > w) left = w;
                            if (right < w) right = w;
                        }
                    }
                }
                frames[f].UnlockBits(bdat);
            }
            while (right % 8 != 0) right++;
            while (left % 8 != 0) left--;
            while (bottom % 8 != 0) bottom++;
            while (top % 8 != 0) top--;

            int width = right - left;
            int height = bottom - top;


            int mostwidths = 0;
            int xadd = 0,
                yadd = 0;


            for (int i = 0; i < frames.Count; i++)
            {
                if (breaks.Contains(i) && multiline)
                {
                    yadd++;
                    xadd = 0;
                }
                xadd++;
                if (xadd > mostwidths)
                {
                    mostwidths = xadd;
                }
            }
            Bitmap tosave = new Bitmap(mostwidths * width, height * (multiline ? breaks.Count : 1));
            Graphics g = Graphics.FromImage(tosave);
            xadd = 0;
            yadd = 0;
            for (int i = 0; i < frames.Count; i++)
            {
                if (breaks.Contains(i) && multiline)
                {
                    yadd++;
                    xadd = 0;
                }
                g.DrawImageUnscaled(frames[i], new Point((width * xadd) - (left), (height * yadd) - (top)));
                xadd++;
            }
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "PNG|*.png";
            if (save.ShowDialog() != DialogResult.Cancel)
            {
                string filename = save.FileName.Substring(0, save.FileName.Length - 4) + " FC[" + realframecount.ToString() + "] AC[" + (getint32(source, offset) / 4).ToString() + "]" + " W[" + width.ToString() + "] H[" + height.ToString() + "] .png";
                tosave.Save(filename);
            }
        }

        private void blagToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            UpdateImage();
        }

        private void drawScaleUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (drawScaleUpDown.Value <= 0)
            {
                drawScaleUpDown.Value = 1;
            }
            UpdateImage();
        }

        private void openBNSAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Battle Network Sprite Archive|*.bnsa";
            if (open.ShowDialog() != DialogResult.Cancel)
                OpenBNSA(open.FileName);
        }

        private void OpenBNSA(string fileName)
        {
            ActiveBNSA = new BNSAFile(fileName);
            ActiveBNSA.ResolveReferences();
            animationIndexUpDown.Maximum = ActiveBNSA.Animations.Count - 1;
            frameIndexUpDown.Maximum = ActiveBNSA.Animations[0].Frames.Count - 1;
            UpdateImage();
        }

        private class GifBitmapDelayPair
        {
            public Bitmap bitmap;
            public int delay;

            public GifBitmapDelayPair(Bitmap bitmap, int delay)
            {
                this.bitmap = bitmap;
                this.delay = delay;
            }
        }

        private void exportedAnimatedGIFOfAnimationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int padding = 2;
            int animationToExport = Convert.ToInt32(animationIndexUpDown.Value);
            Animation anim = ActiveBNSA.Animations[animationToExport];
            using (MagickImageCollection collection = new MagickImageCollection())
            {
                List<GifBitmapDelayPair> sprites = new List<GifBitmapDelayPair>();
                //draw all sprites
                foreach (Frame f in anim.Frames)
                {
                    int animationDelay = Convert.ToInt32(f.FrameDelay * 1.66); //1/60 in ms;
                    sprites.Add(new GifBitmapDelayPair(DrawSprite(f), animationDelay));
                }

                //calculate corners
                int top = 256, left = 256, bottom = 0, right = 0; //inverted maximum values.
                foreach (GifBitmapDelayPair gbdp in sprites)
                {
                    Bitmap b = gbdp.bitmap;
                    var pixelsX = Enumerable.Range(0, b.Width);
                    var pixelsY = Enumerable.Range(0, b.Height);

                    int localleft = pixelsX.FirstOrDefault(
                                x => pixelsY.Any(y => b.GetPixel(x, y).A != 0));
                    int localright = pixelsX.Reverse().FirstOrDefault(
                                x => pixelsY.Any(y => b.GetPixel(x, y).A != 0));
                    int localtop = pixelsY.FirstOrDefault(
                                y => pixelsX.Any(x => b.GetPixel(x, y).A != 0));
                    int localbottom = pixelsY.Reverse().FirstOrDefault(
                                y => pixelsX.Any(x => b.GetPixel(x, y).A != 0));

                    top = Math.Min(top, localtop);
                    left = Math.Min(left, localleft);
                    right = Math.Max(right, localright);
                    bottom = Math.Max(bottom, localbottom);
                }



                //crop bitmaps
                int gifWidth = right - left + padding * 2;
                int gifHeight = bottom - top + padding * 2;
                foreach (GifBitmapDelayPair gbdp in sprites)
                {
                    gbdp.bitmap = CropBitmap(gbdp.bitmap, left - padding, top - padding, gifWidth, gifHeight);
                }

                //add bitmaps to animation
                foreach (GifBitmapDelayPair gbdp in sprites)
                {
                    MagickImage img = new MagickImage(gbdp.bitmap);
                    collection.Last().AnimationDelay = gbdp.delay;
                }


                // Reduce colors (causes flicker)
                //QuantizeSettings settings = new QuantizeSettings();
                //settings.Colors = 256;
                //collection.Quantize(settings);


                // Optionally optimize the images (images should have the same size).
                //collection.Optimize(); //causes ghosting

                //doing neither seems to cause both
                // Save gif
                collection.Write(@"C:\Users\Michael\BNSA\output.gif");
            }
        }

        public Bitmap CropBitmap(Bitmap bitmap, int cropX, int cropY, int cropWidth, int cropHeight)
        {
            Rectangle rect = new Rectangle(cropX, cropY, cropWidth, cropHeight);
            Bitmap cropped = bitmap.Clone(rect, bitmap.PixelFormat);
            return cropped;
        }
    }
}
