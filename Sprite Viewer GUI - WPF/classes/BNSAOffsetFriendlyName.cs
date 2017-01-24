using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprite_Viewer_GUI___WPF.classes
{
    /// <summary>
    /// Class of objects that is displayed in the offset list.
    /// </summary>
    class BNSAOffsetFriendlyName
    {
        public string FriendlyName;
        public int Offset;

        public override string ToString()
        {
            return FriendlyName;
        }
    }
}
