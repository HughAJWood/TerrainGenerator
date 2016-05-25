using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIConvexHull;

namespace TerrainGenerator.GraphGeneration
{
    class Vertex : IVertex
    {
        public double[] Coordinates { get; set; }

        double[] IVertex.Position => Coordinates;

        public Vertex(double x, double y)
        {
            Coordinates = new double[] { x, y };
        }
    }
}
