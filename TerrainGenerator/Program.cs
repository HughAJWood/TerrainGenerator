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
            // The constant values for the algorithm work best as powers of 2
            const int x = 512;
            const int y = 512;
            const int spacing = 32;
            
            var diamondSquare = new DiamondSquare(new Bitmap(x, y));
            var image = diamondSquare.AlgorithmMain(128, 1);
            using (var voronoi = new Voronoi(x, y, image, spacing))
            {
                image = voronoi.Render();
                image.Save("diamondSquareTerrain.bmp", ImageFormat.Bmp);

                // Examples of Aspose.ThreeD: https://github.com/aspose-3d/Aspose.3D-for-.NET

                // If you have a hint to load the Aspose.3D module please ignore,
                // I can't find a DLL that satisifies this, but it still runs fine with any
                // Aspose.3D .net DLL

                // initialize a Scene object
                Scene scene = new Scene();
                // Add our Voronoi mesh to the scene
                scene.RootNode.CreateChildNode("main", voronoi.Mesh);
                // save drawing in the FBX format
                scene.Save("test.fbx", FileFormat.FBX7500Binary);
            }
        }
    }
}
