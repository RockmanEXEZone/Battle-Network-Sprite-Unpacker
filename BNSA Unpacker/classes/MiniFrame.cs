using System;
using System.IO;

namespace BNSA_Unpacker.classes
{
    public class MiniFrame
    {
        public byte[] Memory;
        public long Pointer;
        public byte OAMDataListIndex;
        public byte FrameDelay;
        public byte Flags;
        public Boolean EndFrame;
        public Boolean Loops;
        public Boolean IsValid = true; //only end frames can be invalid... ish.
        public OAMDataList ResolvedOAMDataList;

        /// <summary>
        /// Constructs a mini-frame object, part of a mini-animation.
        /// </summary>
        /// <param name="stream">Stream to construct a miniframe from</param>
        public MiniFrame(Stream stream)
        {
            Pointer = stream.Position;
            OAMDataListIndex = (byte)stream.ReadByte();
            FrameDelay = (byte)stream.ReadByte();
            Flags = (byte)stream.ReadByte();

            //Convenience Booleans
            EndFrame = (Flags & 0x80) != 0;
            Loops = (Flags & 0x40) != 0;

            //Memory for export
            Memory = new byte[3];
            stream.Seek(Pointer, SeekOrigin.Begin);
            stream.Read(Memory, 0, 0x3);

            if (EndFrame)
            {
                byte endSignature = (byte) stream.ReadByte();
                endSignature &= (byte)stream.ReadByte();
                endSignature &= (byte)stream.ReadByte();
                if (endSignature != 0xFF)
                {
                    IsValid = false; //did not end with FF FF FF
                } else
                {
                    IsValid = true;
                }
            }
        }

        /// <summary>
        /// Constructs a MiniFrame object by reading in a miniframe file (binary) from disk.
        /// Sets the endframe and loops variable.
        /// </summary>
        /// <param name="filePath">File to read</param>
        public MiniFrame(string filePath)
        {
            Memory = File.ReadAllBytes(filePath);
            Flags = Memory[2];
            EndFrame = (Flags & 0x80) != 0;
            Loops = (Flags & 0x40) != 0;
        }

        /// <summary>
        /// Maps the OAM Data List Index to the proper data list object
        /// </summary>
        /// <param name="parsedBNSA">Parsed BNSA File to link against</param>
        internal void ResolveReferences(BNSAFile parsedBNSA, Frame frame)
        {
            Console.Write("Resolving OAMIndex " + OAMDataListIndex);
            ResolvedOAMDataList = frame.ResolvedOAMDataListGroup.OAMDataLists[OAMDataListIndex];
            if (ResolvedOAMDataList == null)
            {
                Console.WriteLine("... Failed to resolve: " + OAMDataListIndex);
            } else
            {
                Console.WriteLine("... OK");
            }
        }

        /// <summary>
        /// Writes out the miniframe binary data (3 bytes) to disk, in the format groupindex-ingroupindex-frameindex.
        /// </summary>
        /// <param name="outputPath">Project output directory</param>
        /// <param name="miniAnimGroupIndex">MiniAnimGroup Index</param>
        /// <param name="miniAnimIndex">Index of this mini animation in the mini anim group</param>
        /// <param name="frameIndex">Index of this frame in the sub-minianim.</param>
        internal void Export(string outputPath, int miniAnimGroupIndex, int miniAnimIndex, int frameIndex)
        {
            File.WriteAllBytes(outputPath + @"\minianim" + miniAnimGroupIndex + "-" + miniAnimIndex + "-"+frameIndex+".bin", Memory);
        }

        internal void ResolveReferences(BNSAXMLFile parsedBNSA, Frame owningFrame)
        {
            Console.Write("----Resolving MiniFrame OAMList Index " + OAMDataListIndex);
            ResolvedOAMDataList = owningFrame.ResolvedOAMDataListGroup.OAMDataLists[OAMDataListIndex];
            if (ResolvedOAMDataList == null)
            {
                Console.WriteLine("... Failed to resolve: " + OAMDataListIndex);
            }
            else
            {
                Console.WriteLine("... OK");
            }
        }
    }
}
