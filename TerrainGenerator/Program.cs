using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using TerrainGenerator.General;
using TerrainGenerator.GraphGeneration;
using TerrainGenerator.NoiseGeneration;
using Point = TerrainGenerator.GraphGeneration.Point;

namespace TerrainGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            const int x = 1024;
            const int y = 1024;
            const int spacing = 64;

            //var perlin = new PerlinNoiseMap(new Bitmap(size, size));
            //var image = perlin.Render();
            //image.Save($"perlin.jpg", ImageFormat.Jpeg);
            //var voronoi = new Voronoi(size, image, spacing);
            //image = voronoi.Render();
            //image.Save($"perlinTerrain.jpg", ImageFormat.Jpeg);
            
            var diamondSquare = new DiamondSquare(new Bitmap(x, y));
            var image = diamondSquare.AlgorithmMain(128, 1);
            var voronoi = new Voronoi(x, y, image, spacing);
            image = voronoi.Render();
            image.Save($"diamondSquareTerrain.bmp", ImageFormat.Bmp);

            var lineDrawing = new Bitmap(x,y);
            using (Graphics g = Graphics.FromImage(lineDrawing))
            {
                foreach (Edge edge in voronoi.Edges)
                {
                    g.DrawLine(Pens.White, (int)edge.Item1.X, (int)edge.Item1.Y, (int)edge.Item2.X, (int)edge.Item2.Y);
                }
            }
            lineDrawing.Save($"lineDrawing.bmp", ImageFormat.Bmp);

            //image = flood(image);
        }

        private void testPolyBounding()
        {
            var image = new Bitmap(256, 256);
            var nodes = new List<Point>();
            var Spacing = 32;

            var lx = Spacing;
            while (lx < 256)
            {
                var ly = Spacing;
                while (ly < 256)
                {
                    var x = Rng.Roll(Spacing) - Spacing/2;
                    var y = Rng.Roll(Spacing) - Spacing/2;
                    x += lx;
                    y += ly;
                    nodes.Add(new Point
                    {
                        X = x,
                        Y = y
                    });
                    image.SetPixel(x, y, Color.White);
                    ly += Spacing;
                }
                lx += Spacing;
            }
            image.Save("workingOut.bmp", ImageFormat.Bmp);
        }
        private List<VNode> convexSort(List<Point> nodesForEdges, Bitmap image)
        {
            Point minX = new Point { X = 256 };
            Point minY = new Point { Y =256 };
            Point maxX = new Point();
            Point maxY = new Point();
            // Find minX, minY, maxX, maxY
            foreach (var node in nodesForEdges)
            {
                if (node.X < minX.X) minX = node;
                if (node.Y < minY.Y) minY = node;
                if (node.X > maxX.X) maxX = node;
                if (node.Y > maxY.Y) maxY = node;
            }
            using (Graphics g = Graphics.FromImage(image))
            {
                g.DrawLine(Pens.Red, minX.X, minY.Y, minX.X, maxY.Y);
                g.DrawLine(Pens.Red, minX.X, maxY.Y, maxX.X, maxY.Y);
                g.DrawLine(Pens.Red, maxX.X, maxY.Y, maxX.X, minY.Y);
                g.DrawLine(Pens.Red, maxX.X, minY.Y, minX.X, minY.Y);
            }

            return new List<VNode>();
        }
    }
}
