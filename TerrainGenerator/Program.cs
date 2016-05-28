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
            asposeTest(voronoi);
        }
        static void asposeTest(Voronoi voronoi)
        {
            // For complete examples and data files, please go to https://github.com/aspose-3d/Aspose.3D-for-.NET
            
            // initialize a Scene object
            Scene scene = new Scene();
            // create a Box model
            scene.RootNode.CreateChildNode("main", voronoi.Mesh);
            // save drawing in the FBX format
            scene.Save("test.fbx", FileFormat.FBX7500ASCII);
        }
    }
}
