using System;

namespace TerrainGenerator.GraphGeneration
{
    internal class Edge : Tuple<VNode, VNode>
    {
        public Edge(VNode item1, VNode item2) : base(item1, item2)
        {
        }
    }
}
