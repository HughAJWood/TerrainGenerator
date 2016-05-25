using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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
                g.FillRectangle(Brushes.Black, 0, 0, x, y);
                foreach (Edge edge in voronoi.Edges)
                {
                    g.DrawLine(Pens.White, (int)edge.Item1.X, (int)edge.Item1.Y, (int)edge.Item2.X, (int)edge.Item2.Y);
                }
            }
            lineDrawing.Save($"lineDrawing.bmp", ImageFormat.Bmp);

            //image = flood(image);
        }
    }
}
