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
        List<OAMDataList> OAMDataLists;
        List<Animation> Animations;
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
                    string oamPath = oamDataListsBasepath + f.OAMDataListIndex + "-0.bin";
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

            //Read Palettes
            Palettes = new List<Palette>();
            int index = 0;
            for (int i = 0; i < 16; i++)
            {
                string palettePath = palettesBasepath + index.ToString().PadLeft(2, '0') + ".bin";
                if (File.Exists(palettePath))
                {
                    Palette palette = new Palette(palettePath);
                    Palettes.Add(palette);
                }
                else
                {
                    break; //No more palettes
                }

                index++;
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
            Console.WriteLine("Found " + (maxindex + 1) + " OAM Data Lists");
            OAMDataLists = new List<OAMDataList>();
            for (int i = 0; i <= maxindex; i++)
            {
                Console.WriteLine("Parsing OAM Data List " + i);
                OAMDataList list = new OAMDataList(oamDataListsBasepath,i);
                OAMDataLists.Add(list);
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
            try
            {
                FileStream stream = new FileStream(destinationFile, FileMode.Create);
                //Write Header
                string tilesetBasepath = Directory.GetParent(FilePath).FullName + "\\tilesets\\";

                FileInfo largestTileset = new DirectoryInfo(tilesetBasepath).EnumerateFiles()
                           .OrderByDescending(f => f.Length)
                           .FirstOrDefault();
                //byte largestTileCount = (byte)Math.Ceiling((decimal)largestTileset.Length / 0x20);
                byte largestTileCount = (byte) (largestTileset.Length / 0x20);

                stream.WriteByte(largestTileCount);
                stream.WriteByte(0x0);
                stream.WriteByte(0x1);
                stream.WriteByte((byte) Animations.Count);


                stream.Close();
            }
            catch (NotFiniteNumberException e)
            {

            }
        }
    }
}
