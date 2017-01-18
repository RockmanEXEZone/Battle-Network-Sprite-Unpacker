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
        List<Palette> Palettes;
        /// <summary>
        /// Reads a project XML file and relinks all the parts for file rebuilding
        /// </summary>
        /// <param name="filePath">XML file to parse</param>
        public BNSAXMLFile(string filePath)
        {
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
                Console.WriteLine("OK - animation contains all direct binary references");
            }

            //Read Palettes
            Palettes = new List<Palette>();
            int index = 0;
            for (int i = 0; i < 16; i++) {
                string palettePath = palettesBasepath + index.ToString().PadLeft(2, '0') + ".bin";
                if (File.Exists(palettePath))
                {
                    Palette palette = new Palette(palettePath);
                    Palettes.Add(palette);
                } else
                {
                    break; //No more palettes
                }

                index++;
            }
            Console.WriteLine("Read " + Palettes.Count + " palettes");

            //Read MiniAnims

            //Read OAM Data
        }
    }
}
