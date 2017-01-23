using BNSA_Unpacker.classes;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sprite_Viewer_GUI___WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        //Rom Codes
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
        List<Button> paletteButtons;

        private BNSAFile ActiveBNSA;
        public Byte[] ROM;
        private BackgroundWorker spriteAnimatorWorker;
        private ManualResetEvent mre = new ManualResetEvent(false);


        public MainWindow()
        {
            InitializeComponent();
            paletteButtons = new List<Button>();
            paletteButtons.Add(paletteColor1Button);
            paletteButtons.Add(paletteColor2Button);
            paletteButtons.Add(paletteColor3Button);
            paletteButtons.Add(paletteColor4Button);
            paletteButtons.Add(paletteColor5Button);
            paletteButtons.Add(paletteColor6Button);
            paletteButtons.Add(paletteColor7Button);
            paletteButtons.Add(paletteColor8Button);
            paletteButtons.Add(paletteColor9Button);
            paletteButtons.Add(paletteColor10Button);
            paletteButtons.Add(paletteColor11Button);
            paletteButtons.Add(paletteColor12Button);
            paletteButtons.Add(paletteColor13Button);
            paletteButtons.Add(paletteColor14Button);
            paletteButtons.Add(paletteColor15Button);
            paletteButtons.Add(paletteColor16Button);
            RenderOptions.SetBitmapScalingMode(frameImage, BitmapScalingMode.NearestNeighbor);
            spriteAnimatorWorker = new BackgroundWorker();
            spriteAnimatorWorker.DoWork += SpriteAnimatorWork;
            spriteAnimatorWorker.WorkerSupportsCancellation = true;
            spriteAnimatorWorker.RunWorkerAsync();
            List<String> directions = new List<String>();
            directions.Add("Open a file");
            directions.Add("to view sprites");
            romSpritePointersListbox.ItemsSource = directions;
        }

        private void SpriteAnimatorWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (true)
            {
                mre.WaitOne(Timeout.Infinite);
                int delay = 100;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (animateCheckbox.IsChecked == true)
                    {
                        if ((int)frameIndexUpDown.Value + 1 > frameIndexUpDown.Maximum)
                        {
                            frameIndexUpDown.Value = 0;
                        }
                        else
                        {
                            frameIndexUpDown.Value++;
                        }

                        delay = ActiveBNSA.Animations[Convert.ToInt32(animationIndexUpDown.Value)].Frames[(int)frameIndexUpDown.Value].FrameDelay;
                    }
                }));
                Thread.Sleep((1000 * delay) / 60); //nothing like some good cross threading code -_-
            }
        }

        private void openRomMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Battle Network ROM|*.GBA";
            if (open.ShowDialog() == true)
                OpenROM(open.FileName);
        }

        private void OpenROM(string open)
        {
            frameImage.Stretch = Stretch.None;
            romSpritePointersListbox.ItemsSource = new List<String>(); //clear
            ROM = File.ReadAllBytes(open);
            string ROMID = GetROMID(ROM);
            List<string> lbi = new List<string>();
            int startoffset = 0;
            int endoffset = 0;
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
            romSpritePointersListbox.ItemsSource = lbi.ToArray();
            romSpritePointersListbox.SelectedIndex = 0;
            ChangeAnimation();
            //filename = open;
            //this.Text = "Megaman BattleNetwork Sprite List Viewer ~ By Greiga Master (" + Path.GetFileName(filename) + ")";
        }

        private void openBNSAMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Battle Network Sprite Archive|*.bnsa";
            if (open.ShowDialog() == true)
                OpenBNSA(open.FileName);
        }

        /// <summary>
        /// Opens a BNSA file from disk and loads it into the viewer
        /// </summary>
        /// <param name="fileName"></param>
        private void OpenBNSA(string fileName)
        {
            ActiveBNSA = new BNSAFile(fileName);
            ActiveBNSA.ResolveReferences();
            animationIndexUpDown.Maximum = ActiveBNSA.Animations.Count - 1;
            paletteIndexUpDown.Maximum = ActiveBNSA.Palettes.Count - 1;
            animationCountLabel.Content = "of " + animationIndexUpDown.Maximum;
            paletteCountLabel.Content = "of " + (ActiveBNSA.Palettes.Count - 1);
            //frameIndexUpDown.Maximum = ActiveBNSA.Animations[0].Frames.Count - 1;
            ChangeAnimation();
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
            paletteData[0] = System.Drawing.Color.Transparent.ToArgb();
            return paletteData;
        }

        /// <summary>
        /// Converts BGR to Alpha RGB for displaying in a windows bitmap
        /// </summary>
        /// <param name="bgr">bbggrr short value.</param>
        /// <returns></returns>
        private static int bgr2argb(short bgr)
        {
            byte a = 0xFF,
                 r = (byte)((bgr & 0x1F) << 3),
                 g = (byte)(((bgr >> 5) & 0x1F) << 3),
                 b = (byte)(((bgr >> 10) & 0x1F) << 3);
            return (a << 24) | (r << 16) | (g << 8) | b;
        }

        /// <summary>
        /// Reads tileset data and places it into a bitmap, applying the palette provided.
        /// </summary>
        /// <param name="width">Width of item being read</param>
        /// <param name="height">Height of item being read</param>
        /// <param name="graphics">Tile to read</param>
        /// <param name="palette">Palette to apply</param>
        /// <returns></returns>
        private static unsafe Bitmap Read4BBP(int width, int height, byte[] graphics, int[] palette)
        {
            Bitmap img = new Bitmap(width, height);
            BitmapData bitdat = img.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
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

        /// <summary>
        /// Draws the sprite to a 256*256 (x scale) bitmap for displaying or writing to disk
        /// </summary>
        /// <param name="frame">Frame to draw</param>
        /// <returns>Bitmap of drawn sprite</returns>
        private Bitmap DrawSprite(BNSA_Unpacker.classes.Frame frame)
        {
            Bitmap picture = new Bitmap(256, 256);
            Palette palette = ActiveBNSA.Palettes[Convert.ToInt32(paletteIndexUpDown.Value)];
            OAMDataListGroup frameData = frame.ResolvedOAMDataListGroup;
            int[] paletteData = paletteData = getUsablePaletteColors(palette.Memory);
            //int animationoffset = offset + getint32(spritefile, offset + (4 * animationIndex));
            //int gpointer = offset + getint32(spritefile, animationoffset + (5 * 4 * frameIndex));
            //int ppointer = offset + getint32(spritefile, animationoffset + (5 * 4 * frameIndex) + 4) + (int)(numericUpDown3.Value * 0x20);
            UpdatePaletteDisplay(paletteData);
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
                System.Drawing.Point drawPos = new System.Drawing.Point((picture.Width / 2) + entry.XOrigin, (picture.Height / 2) + entry.YOrigin);
                //Console.WriteLine("Drawing OAM " + i + " w TN " + entry.TileNumber + ": " + drawPos.X + "," + drawPos.Y);
                g.DrawImageUnscaled(tileBitMap, drawPos);
                i++;
            }

            //Scale image
            int scale = (int)drawScaleUpDown.Value;
            Bitmap returnpicture = resizeImage(picture, scale * picture.Width, scale * picture.Height);
            //Graphics finalgraphics = Graphics.FromImage(returnpicture);
            //finalgraphics.DrawImage(new Bitmap(256, 256, g), returnpicture.Width, returnpicture.Height);

            //g.DrawRectangle(Pens.Black, new Rectangle(0, 0, picture.Width-1, picture.Height-1));
            return returnpicture;
        }

        private void UpdateInfoBox(BNSA_Unpacker.classes.Frame frame)
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

        public static Bitmap resizeImage(Bitmap image, int new_height, int new_width)
        {
            Bitmap new_image = new Bitmap(new_width, new_height);
            Graphics g = Graphics.FromImage((System.Drawing.Image)new_image);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(image, 0, 0, new_width, new_height);
            return new_image;
        }

        /// <summary>
        /// Updates the Palette Display Box
        /// </summary>
        /// <param name="paletteColorsARGB">Palettes in ARGB</param>
        private void UpdatePaletteDisplay(int[] paletteColorsARGB)
        {
            for (int i = 1; i < 16; i++)
            {
                Button button = paletteButtons[i];
                int myColor = paletteColorsARGB[i];
                byte b = (byte)(myColor & 0xFF);
                byte g = (byte)((myColor >> 8) & 0xFF);
                byte r = (byte)((myColor >> 16) & 0xFF);
                byte a = (byte)((myColor >> 24) & 0xFF);
                button.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(a, r, g, b));
            }
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
            BNSA_Unpacker.classes.Frame frame = ActiveBNSA.Animations[animationIndex].Frames[frameIndex];
            Bitmap bm = DrawSprite(frame);

            //PresentationSource source = PresentationSource.FromVisual(this);

            //double dpiX = 96, dpiY = 96;
            //if (source != null)
            //{
            //    dpiX *= source.CompositionTarget.TransformToDevice.M11;
            //    dpiY *= source.CompositionTarget.TransformToDevice.M22;
            //}

            frameImage.Source = loadBitmap(bm);

            //Do not remove! Huge memory spike if you do. Should probably cache the sprites at their current drawing level. or something.
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Gets the ROM ID of the loaded rom
        /// </summary>
        /// <param name="ROM">ROM file as byte array</param>
        /// <returns></returns>
        private static string GetROMID(byte[] ROM)
        {
            return ASCIIEncoding.ASCII.GetString(ROM, 0xAC, 4);
        }

        /// <summary>
        /// Converts BitMap object to BitMapSource (usable by WPF Image)
        /// </summary>
        /// <param name="source">Bitmap To Convert</param>
        /// <returns></returns>
        public static BitmapSource loadBitmap(System.Drawing.Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()
                   );
            }
            finally
            {
                DeleteObject(ip);
            }
            return bs;
        }

        private void romSpritePointersListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (romSpritePointersListbox.SelectedIndex == -1)
            {
                return;
            }

            string listText = (string)romSpritePointersListbox.SelectedItem;
            int bnsaOffset;
            bool isNumeric = int.TryParse(listText, NumberStyles.HexNumber, CultureInfo.InvariantCulture
                , out bnsaOffset);
            if (isNumeric)
            {
                byte[] BNSAMemory;
                int offset = 0;
                animationIndexUpDown.Value = 0;
                frameIndexUpDown.Value = 0;
                paletteIndexUpDown.Value = 0;

                if (romSpritePointersListbox.SelectedItem.ToString().Substring(0, 2) == "88")
                {
                    //Archive is compressed
                    BNSAMemory = Nintenlord.GBA.Compressions.LZ77.Decompress(ROM, (int.Parse((string)romSpritePointersListbox.SelectedItem, System.Globalization.NumberStyles.HexNumber) & 0x00FFFFFF));
                    Array.Copy(BNSAMemory, 4, BNSAMemory, 0, BNSAMemory.Length - 4);
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
                //File.WriteAllBytes(@"C:\users\michael\bnsa\bnsabytes.bnsa", BNSAMemory);
                ActiveBNSA = new BNSAFile(BNSAMemory);
                ActiveBNSA.ResolveReferences();
                animationIndexUpDown.Maximum = ActiveBNSA.Animations.Count - 1;
                paletteIndexUpDown.Maximum = ActiveBNSA.Palettes.Count - 1;
                animationCountLabel.Content = "of " + animationIndexUpDown.Maximum;
                paletteCountLabel.Content = "of " + (ActiveBNSA.Palettes.Count - 1);
                //frameIndexUpDown.Maximum = ActiveBNSA.Animations[0].Frames.Count - 1;
                ChangeAnimation();
            }
        }

        /// <summary>
        /// Gets number of frames in the animation... will likely be removed in future version for BNSA version
        /// </summary>
        /// <param name="animation"></param>
        /// <returns></returns>
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

        private void paletteButtonClick(object sender, RoutedEventArgs e)
        {
            //Not yet implemented...
            Button callingButton = (Button)sender;
            callingButton.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(40, 40, 160));
        }

        private void paletteIndexUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateImage();
        }

        private void frameIndexUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateImage();
        }

        private void animationIndexUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ChangeAnimation();
        }

        /// <summary>
        /// Sets frame index to 0, updates the maximum amount of frames, enables/disables the animate box, and draws the sprite
        /// </summary>
        private void ChangeAnimation()
        {
            if (frameIndexUpDown != null)
            {
                frameIndexUpDown.Value = 0;
                //paletteIndexUpDown.Value = 0;
                if (ActiveBNSA != null)
                {
                    frameIndexUpDown.Maximum = ActiveBNSA.Animations[(int)animationIndexUpDown.Value].Frames.Count - 1;
                    animateCheckbox.IsEnabled = ActiveBNSA.Animations[(int)animationIndexUpDown.Value].Frames.Count - 1 > 0;
                    if (animateCheckbox.IsEnabled && animateCheckbox.IsChecked == true)
                    {
                        mre.Set();
                    }
                    else if (!animateCheckbox.IsEnabled && animateCheckbox.IsChecked == true)
                    {
                        mre.Reset();
                    }

                    frameCountLabel.Content = "of " + (ActiveBNSA.Animations[(int)animationIndexUpDown.Value].Frames.Count - 1);
                    UpdateImage();
                }
            }
        }

        private void drawScaleUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateImage();
        }

        private void frameImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                drawScaleUpDown.Value = Math.Min((int)drawScaleUpDown.Value + 1, 5);
            }
            else if (e.Delta < 0)
            {
                drawScaleUpDown.Value = Math.Max((int)drawScaleUpDown.Value - 1, 1);
            }
        }

        private void animateCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            mre.Set();
        }

        private void animateCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            mre.Reset();
        }

        private void paletteIndexUpDown_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int currentPaletteIndex = (int)paletteIndexUpDown.Value;
            if (e.Delta > 0)
            {
                //mouse wheel up
                if (currentPaletteIndex + 1 > paletteIndexUpDown.Maximum)
                {
                    return;
                }
                else
                {
                    paletteIndexUpDown.Value++;
                }
            }
            else if (e.Delta < 0)
            {
                //mouse wheel up
                if (currentPaletteIndex - 1 < 0)
                {
                    return;
                }
                else
                {
                    paletteIndexUpDown.Value--;
                }
            }
        }

        private void frameIndexUpDown_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int currentFrameIndex = (int)frameIndexUpDown.Value;
            if (e.Delta > 0)
            {
                //mouse wheel up
                if (currentFrameIndex + 1 > frameIndexUpDown.Maximum)
                {
                    return;
                }
                else
                {
                    frameIndexUpDown.Value++;
                }
            }
            else if (e.Delta < 0)
            {
                //mouse wheel up
                if (currentFrameIndex - 1 < 0)
                {
                    return;
                }
                else
                {
                    frameIndexUpDown.Value--;
                }
            }
        }

        private void animationIndexUpDown_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int currentAnimIndex = (int)animationIndexUpDown.Value;
            if (e.Delta > 0)
            {
                //mouse wheel up
                if (currentAnimIndex + 1 > animationIndexUpDown.Maximum)
                {
                    return;
                }
                else
                {
                    animationIndexUpDown.Value++;
                }
            }
            else if (e.Delta < 0)
            {
                //mouse wheel up
                if (currentAnimIndex - 1 < 0)
                {
                    return;
                }
                else
                {
                    animationIndexUpDown.Value--;
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow w = new AboutWindow();
            w.Show();
        }

        private void bgColorButtonClick(object sender, RoutedEventArgs e)
        {
            Button colorButton = (Button) sender;
            centerPanel.Background = colorButton.Background;
        }
    }
}
