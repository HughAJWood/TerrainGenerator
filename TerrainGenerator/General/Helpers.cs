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
        internal static bool Adjacent(this VNode a, VNode b, double tolerance = 3.0)
        {
            var xd = a.X - b.X;
            var yd = a.Z - b.Z;
            var distance = Math.Sqrt(xd*xd + yd*yd);
            return distance < tolerance;
        }
    }
}
