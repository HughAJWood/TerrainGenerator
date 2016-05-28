using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrainGenerator.GraphGeneration;

namespace TerrainGenerator.General
{
    static class Helpers
    {
        internal static bool Adjacent(this VNode a, VNode b, int tolerance = 3)
        {
            var distance = Math.Abs(a.X - b.X) + Math.Abs(a.Z - b.Z);
            return distance < tolerance;
        }
    }
}
