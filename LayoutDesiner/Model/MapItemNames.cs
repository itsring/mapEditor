using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapEditor.Model
{
    internal static class MapItemNames
    {
        public const string none = "NONE";
        public const string node = "NODE";
        public const string edge = "EDGE";
        public const string lotate = "LOTATE";
        public const string station = "STATION";


        public static string[] getNames()
        {
            return new string[] { none, node, edge, lotate, station };
        }
    }
}
