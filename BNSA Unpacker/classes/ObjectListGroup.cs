using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSA_Unpacker.classes
{
    class ObjectListGroup
    {
        public long Pointer;
        public List<ObjectList> ObjectLists = new List<ObjectList>();
        public ObjectListGroup(FileStream stream)
        {
            Pointer = stream.Position;
            Console.WriteLine("--Reading Object List Group at 0x" + stream.Position.ToString("X2"));

            int firstObjectEntryPointer = int.MaxValue;

            while (stream.Position < firstObjectEntryPointer + Pointer)
            {
                int objectEntryPointer = BNSAFile.ReadIntegerFromStream(stream);
                firstObjectEntryPointer = Math.Min(firstObjectEntryPointer, objectEntryPointer); //should only be triggered by the first pointer as it goes ascending.
                long nextPosition = stream.Position; //Address of next pointer in the list

                ObjectList objectList = new ObjectList(stream);

                ObjectLists.Add(objectList);
                if (nextPosition < firstObjectEntryPointer + Pointer)
                {
                    //Read the next 4 bytes in the pointer table as its a new pointer
                    stream.Seek(nextPosition, SeekOrigin.Begin);
                }
            }
        }



    }
}
