using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace BNSA_Unpacker.classes
{
    class Animation
    {
        public int Index;
        private int Pointer;
        public List<Frame> Frames;

        /// <summary>
        /// Animation Object, created from a pointer in the BNSA archive. The passed in stream will be used to load data. The stream position will be modified, so save it before you call this.
        /// </summary>
        /// <param name="bnsa">BNSA Object. Will be used so the probably palette pointer can be updated.</param>
        /// <param name="ptr">Pointer (from start of file) to the animation data.</param>
        /// <param name="stream">File stream to read read from</param>
        public Animation(BNSAFile bnsa, int ptr, FileStream stream)
        {
            Pointer = ptr;
            Pointer += 4; //Skip Header
            Console.WriteLine("Animation Pointer 0x" + Pointer.ToString("X8")); //"X8" = 8 byte hex output
            stream.Seek(Pointer, SeekOrigin.Begin);
            Frames = new List<Frame>();
            int frameindex = 0;
            while (true)
            {
                //Read Frame Until Stop or Loop at Pos + 0x12
                Console.WriteLine("--Reading Frame " + frameindex + " at 0x" + stream.Position.ToString("X2"));
                Frame frame = new Frame(stream);
                bnsa.ProbablyPaletteStartPointer = Math.Min(bnsa.ProbablyPaletteStartPointer, frame.PalettePointer); //Find first Palette Pointer
                Frames.Add(frame);
                if (frame.EndFrame) //End of animation indicator
                {
                    break;
                }
                frameindex++;
            }
        }

        /// <summary>
        /// Constructs an Animation object from an XML node
        /// </summary>
        /// <param name="animationNode">Node to generate from</param>
        public Animation(XmlNode animationNode)
        {
            Index = Int32.Parse(animationNode.Attributes["index"].Value);
            Frames = new List<Frame>();

            XmlNodeList frameNodes = animationNode.SelectNodes("frame");
            Console.WriteLine("Found " + frameNodes.Count + " frames in animation");
            foreach (XmlNode node in frameNodes)
            {
                Frame f = new Frame(Index, node);
                Frames.Add(f);
            }
        }

        /// <summary>
        /// Changes binary file pointers and indexes to object references from the parsed BNSA file.
        /// This calls ResolveReferences() on each frame in the animation
        /// </summary>
        internal void ResolveReferences(BNSAFile parsedBNSA)
        {
            int i = 0;
            foreach (Frame frame in Frames)
            {
                Console.WriteLine("--Resolving Frame " + i + " references");
                frame.ResolveReferences(parsedBNSA);
                i++;
            }
        }

        internal void ResolveReferences(BNSAXMLFile parsedBNSA)
        {
            int i = 0;
            foreach (Frame frame in Frames)
            {
                Console.WriteLine("--Resolving Frame " + i + " references");
                frame.ResolveReferences(parsedBNSA);
                i++;
            }
        }

        /// <summary>
        /// Recursively outputs frames into the frames directory, as frame[animindex]-[frameindex].bin.
        /// </summary>
        /// <param name="outputDirectory">Directory to output data into. Do not include the frames subdirectory.</param>
        /// <param name="animationIndex">Index of this animation</param>
        public void Export(string outputDirectory, int animationIndex)
        {
            Index = animationIndex;
            int i = 0;
            foreach (Frame frame in Frames)
            {
                //Console.WriteLine("--Resolving Frame " + i + " references");
                frame.Export(outputDirectory, animationIndex, i);
                i++;
            }
        }

        internal List<XmlNode> GetChildNodes(XmlDocument xmlDoc)
        {
            List<XmlNode> nodes = new List<XmlNode>();
            foreach (Frame frame in Frames)
            {
                nodes.Add(frame.GenerateNode(xmlDoc));
            }
            return nodes;
        }
    }
}
