using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Aspose.ThreeD;
using Aspose.ThreeD.Entities;
using Aspose.ThreeD.Utilities;
using TerrainGenerator.General;
using TerrainGenerator.GraphGeneration;
using TerrainGenerator.NoiseGeneration;

namespace TerrainGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            const int x = 1024;
            const int y = 1024;
            const int spacing = 64;
            
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
            asposeTest(voronoi);
        }
        static void asposeTest(Voronoi voronoi)
        {
            // For complete examples and data files, please go to https://github.com/aspose-3d/Aspose.3D-for-.NET
            
            var tris = new Mesh("landMesh");
            var vectors = voronoi.Nodes.Select(n => new Vector4(n.X, n.Y, n.Z, 1));

            var vector4s = vectors as Vector4[] ?? vectors.ToArray();
            tris.ControlPoints.AddRange(vector4s);
            
            for (var  i = 0; i < vector4s.Count(); i+=3)
            {
                tris.CreatePolygon(new []{ i, i+1, i+2 });
            }

            // initialize a Scene object
            Scene scene = new Scene();
            // create a Box model
            scene.RootNode.CreateChildNode("main", tris);
            // save drawing in the FBX format
            scene.Save("test.fbx", FileFormat.FBX7500ASCII);
        }
    }
}
