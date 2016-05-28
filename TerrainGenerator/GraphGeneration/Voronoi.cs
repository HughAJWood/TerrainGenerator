using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Aspose.ThreeD.Entities;
using Aspose.ThreeD.Utilities;
using TerrainGenerator.Biomes;
using TerrainGenerator.General;

namespace TerrainGenerator.GraphGeneration
{
    public enum MapType
    {
        Radial,
        Square,
        RandomTile,
        RandomRadial,
        RandomIsland
    }
    internal class Voronoi
    {
        private readonly List<Point> _pointTable = new List<Point>();
        internal readonly int X, Y, Spacing;
        private readonly Bitmap _image;
        private readonly Bitmap _colorMap;
        private readonly Bitmap _perlin;
        private readonly List<Biome> _biome = new List<Biome>();
        private readonly List<List<Point>> _regions = new List<List<Point>>();
        private readonly int _moisture;
        public int Iterations { get; set; } = 1;
        public bool DoShowPoints { get; set; } = false;
        public MapType MapType { get; set; } = MapType.RandomIsland;
        public List<VNode> Nodes { get; } = new List<VNode>();
        public List<Edge> Edges { get; } = new List<Edge>();
        private int _vnodeId = -1;
        public Mesh Mesh { get; } = new Mesh("voronoiMesh");

        private int VNodeId
        {
            get
            {
                _vnodeId++;
                return _vnodeId;
            }

        }

        internal Voronoi(int x, int y, Bitmap perlin, int spacing = 10, int moisture = 3)
        {
            X = x;
            Y = y;
            Spacing = spacing;
            _image = new Bitmap(x,y);
            _colorMap = new Bitmap(x,y);
            _perlin = perlin;
            _moisture = moisture;
        }

        public Bitmap Render()
        {
            CreatePointTable();
            switch (MapType)
            {
                case MapType.Radial:
                    CreateColorsRadial();
                    break;
                case MapType.Square:
                    throw new NotImplementedException("Square map not implemented");
                case MapType.RandomTile:
                    throw new NotImplementedException("Random Tile map not implemented");
                case MapType.RandomRadial:
                    throw new NotImplementedException("Random Radial map not implemented");
                case MapType.RandomIsland:
                    CreateColors();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Value" + "doesn't exist for MapType");
            }

            GenerateRelaxation();
            CalculatePolys();
            _perlin.Save($"noiseWithSea{_moisture}.jpg", ImageFormat.Jpeg);
            return _image;
        }

        private void CalculatePolys()
        {
            var i = 0;
            var nodesForEdges = new List<VNode>();
            foreach (var region in _regions)
            {
                foreach (var point in region)
                {
                    if (point == null)
                    {
                        continue;
                    }
                    // We calculate where the voronoi node corners are, and pass them into the region
                    var diff = ColoursDifferent(point);
                    if (diff > 1)
                    {
#if DEBUG
                        RenderNodePointsOnImage(point);
#endif
                        // Points are 2D, so to translate to 3D they are set to the Z property of the 3D node
                        var node = new VNode
                        {
                            X = point.X,
                            Z = point.Y
                        };
                        // Weld points within 1 unit from each other by determining if we already have one that exists
                        // then using the existing node instead
                        // A weld is calculated by determining the X,Z distance to see if we have a node stored already
                        // that is within 2 units of the node we are creating
                        VNode existingNode = Nodes.FirstOrDefault(n => n.Adjacent(node));
                        if (existingNode == null)
                        {
                            // Optimise node creation by skipping perlin lookup and ID iteration for only when needed
                            node.ID = VNodeId;
                            node.Y = _perlin.GetPixel(point.X, point.Y).R;
                            Nodes.Add(node);
                            Mesh.ControlPoints.Add(new Vector4(node.X, node.Y, node.Z, 1));
                        }
                        else
                        {
                            node = existingNode;
                        }
                        nodesForEdges.Add(node);
                    }
                }
                //if (i++ > 20) return;
                AddNodesToMeshAsPolys(nodesForEdges);
                nodesForEdges.Clear();
            }
        }

        private void RenderNodePointsOnImage(Point point)
        {
            _image.SetPixel(point.X, point.Y, Color.Red);
            if (point.X + 1 < X) _image.SetPixel(point.X + 1, point.Y, Color.Red);
            if (point.X - 1 > 0) _image.SetPixel(point.X - 1, point.Y, Color.Red);
            if (point.Y - 1 > 0) _image.SetPixel(point.X, point.Y - 1, Color.Red);
            if (point.X + 1 < X && point.Y - 1 > 0) _image.SetPixel(point.X + 1, point.Y - 1, Color.Red);
            if (point.X - 1 > 0 && point.Y - 1 > 0) _image.SetPixel(point.X - 1, point.Y - 1, Color.Red);
            if (point.Y + 1 < Y) _image.SetPixel(point.X, point.Y + 1, Color.Red);
            if (point.X + 1 < X && point.Y + 1 < Y) _image.SetPixel(point.X + 1, point.Y + 1, Color.Red);
            if (point.X - 1 > 0 && point.Y + 1 < Y) _image.SetPixel(point.X - 1, point.Y + 1, Color.Red);
        }

        private void AddNodesToMeshAsPolys(List<VNode> nodesForEdges)
        {
            if (nodesForEdges.Any())
            {
                VNode midPoint;
                nodesForEdges = ConvexSort(nodesForEdges, out midPoint);
                Mesh.ControlPoints.Add(new Vector4(midPoint.X, midPoint.Y, midPoint.Z, 1));

                // Create voronoi region 1 tri at a time
                for (var index = 0; index < nodesForEdges.Count; index++)
                {
                    var n = nodesForEdges[index];
                    var nextNodeIndex = index + 1;
                    if (nextNodeIndex == nodesForEdges.Count) nextNodeIndex = 0;

                    // Add tri poly
                    Mesh.CreatePolygon(new[] {n.ID, nodesForEdges[nextNodeIndex].ID, midPoint.ID});
                }
            }
        }

        /// <summary>
        /// Sort points by determining bounding box centre then sorting by the angle from the bounding box 12 o'clock line.
        /// </summary>
        /// <param name="nodesForEdges">The bounding points as determined by reading the image</param>
        /// <param name="midPoint">Returns the centre of the bounding box to act as the centre of the region</param>
        /// <returns></returns>
        private List<VNode> ConvexSort(List<VNode> nodesForEdges, out VNode midPoint)
        {
            // We are flipping Z and Y to go from 2D to 3D
            var minX = new VNode { X = X };
            var minY = new VNode { Y = 255 };
            var minZ = new VNode { Z = Y };
            var maxX = new VNode();
            var maxY = new VNode();
            var maxZ = new VNode();
            // Find minX, minY, maxX, maxY
            foreach (var node in nodesForEdges)
            {
                if (node.X < minX.X) minX = node;
                if (node.Y < minY.Y) minY = node;
                if (node.Z < minZ.Z) minZ = node;

                if (node.X > maxX.X) maxX = node;
                if (node.Y > maxY.Y) maxY = node;
                if (node.Z > maxX.Z) maxZ = node;
            }
            midPoint = new VNode
            {
                ID = VNodeId,
                X = (minX.X + maxX.X) / 2,
                Y = (minY.Y + maxY.Y) / 2,
                Z = (minZ.Z + maxZ.Z) / 2
            };
            VNode middlePoint = midPoint;
            nodesForEdges = new List<VNode>(nodesForEdges.OrderByDescending(p =>
            {
                var angle = GetAngleDegree(middlePoint, p);
                return angle;
            }));

            return nodesForEdges;
        }

        public static double GetAngleDegree(VNode origin, VNode target)
        {
            var n = 270 - Math.Atan2(origin.Z - target.Z, origin.X - target.X) * 180 / Math.PI;
            return n % 360;
        }

        private int ColoursDifferent(Point p)
        {
            var x = p.X;
            var y = p.Y;
            var diffCount = 0;
            var edge = false;
            var excludedSamples = new List<Color> { _colorMap.GetPixel(x, y) };
            var allSamples = new List<Color>();
            for (var xx = x - 1; xx <= x + 1; xx++)
            {
                if (xx < 0 || xx >= X)
                {
                    edge = true;
                    continue;
                }
                for (var yy = y - 1; yy <= y + 1; yy++)
                {
                    if (yy < 0 || yy >= Y)
                    {
                        edge = true;
                        continue;
                    }
                    var sample = _colorMap.GetPixel(xx, yy);
                    allSamples.Add(sample);
                    if (!excludedSamples.Contains(sample))
                    {
                        excludedSamples.Add(sample);
                        diffCount++;
                    }
                }
            }
            if (edge) diffCount++;
            return diffCount;
        }

        private void GenerateRelaxation()
        {
            for (var i = 1; i <= Iterations; i++)
            {
                Console.WriteLine("Generating diagram pass " + i);
                Console.WriteLine("Rendering Voronoi Map");
                CreateSites();
                if (DoShowPoints)
                {
                    ShowPoints();
                }

                if (Iterations > 1 && i < Iterations)
                {
                    CalculateNewPoints();
                }
            }
        }

        private void ShowPoints()
        {
            foreach (var point in _pointTable)
            {
                _image.SetPixel(point.X, point.Y, Color.Black);
            }
        }

        private void CalculateNewPoints()
        {
            var newPointTable = new List<Point>();
            var i = 0;
            foreach (var region in _regions)
            {
                if (region.Any())
                {
                    var point = GetCentrePoint(region, _pointTable[i]);
                    newPointTable.Add(point);
                }
                i++;
            }
            _pointTable.Clear();
            _pointTable.AddRange(newPointTable);
        }

        public Point GetCentrePoint(List<Point> region, Point origin)
        {
            var avx = region.Sum(p => p.X)/region.Count;
            var avy = region.Sum(p => p.Y)/region.Count;

            return new Point {X = avx, Y = avy};
        }

        void CreateColors()
        {
            var p = new Point
            {
                X = X/2, Y = Y/2
            };
            var maxDistance = DistanceSqrd(p, 0, 0);
            var onePerlinDistance = 255/(double)maxDistance;
            foreach (var point in _pointTable)
            {
                _regions.Add(new List<Point>());
                int perlinLevel = _perlin.GetPixel(point.X, point.Y).R;
                if (perlinLevel == 0) perlinLevel = 1;

                var distance = DistanceSqrd(point, p.X, p.Y);
                var distanceNormalised = onePerlinDistance * distance * 1.8;

                if (perlinLevel < distanceNormalised)
                {
                    // Sea
                    var b = new Biome {MoistureLevel = 100};
                    _biome.Add(b);
                }
                else
                {
                    int aboveSeaLevel;
                    if (perlinLevel < 128)
                    {
                        aboveSeaLevel = 2;
                    }
                    else if (perlinLevel < 170)
                    {
                        aboveSeaLevel = 3;
                    }
                    else
                    {
                        aboveSeaLevel = 4;
                    }
                    var b = new Biome
                    {
                        Elevation = aboveSeaLevel, MoistureLevel = _moisture
                    };
                    _biome.Add(b);
                }
            }
        }

        void CreateColorsRadial()
        {
            var p = new Point
            {
                X = X/2, Y = Y/2
            };
            var maxDistance = DistanceSqrd(p, 0, Y/2);
            var minDistance = maxDistance/8;
            var distances = new[]
            {
                minDistance*7, minDistance*6, minDistance*5, minDistance*4, minDistance*3, minDistance*2, minDistance, minDistance/2
            };
            foreach (var point in _pointTable)
            {
                _regions.Add(new List<Point>());
                var distance = DistanceSqrd(point, p.X, p.Y);
                var perlinLevel = (int) Math.Round(_perlin.GetPixel(point.X, point.Y).R/(double) 64, 0);
                if (perlinLevel == 0) perlinLevel = 1;
                if (perlinLevel == 1 || distance > distances[1])
                {
                    // Sea
                    var b = new Biome {MoistureLevel = 100};
                    _biome.Add(b);
                }
                else if (distance > distances[2])
                {
                    var b = new Biome
                    {
                        Elevation = perlinLevel, MoistureLevel = 1
                    };
                    _biome.Add(b);
                }
                else if (distance > distances[3])
                {
                    var b = new Biome
                    {
                        Elevation = perlinLevel, MoistureLevel = 2
                    };
                    _biome.Add(b);
                }
                else if (distance > distances[4])
                {
                    var b = new Biome
                    {
                        Elevation = perlinLevel, MoistureLevel = 3
                    };
                    _biome.Add(b);
                }
                else if (distance > distances[5])
                {
                    var b = new Biome
                    {
                        Elevation = perlinLevel, MoistureLevel = 1
                    };
                    _biome.Add(b);
                }
                else if (distance > distances[6])
                {
                    var b = new Biome
                    {
                        Elevation = 3, MoistureLevel = 1
                    };
                    _biome.Add(b);
                }
                else if (distance < distances[7])
                {
                    var b = new Biome
                    {
                        Elevation = 4, MoistureLevel = 1
                    };
                    _biome.Add(b);
                }
                else
                {
                    var b = new Biome
                    {
                        Elevation = perlinLevel, MoistureLevel = perlinLevel > 4 ? 4 : perlinLevel
                    };
                    _biome.Add(b);
                }
            }
        }

        internal void CreatePointTable()
        {
            var lx = Spacing;
            while (lx < X)
            {
                var ly = Spacing;
                while (ly < Y)
                {
                    var x = Rng.Roll(Spacing) - Spacing/2;
                    var y = Rng.Roll(Spacing) - Spacing/2;
                    x += lx;
                    y += ly;
                    _pointTable.Add(new Point {X = x, Y = y});
                    ly += Spacing;
                }
                lx += Spacing;
            }
        }

        readonly object _bmpLock = new object();
        internal void CreateSites()
        {
            Enumerable.Range(0, Y)
                .AsParallel()
                .ForAll(hh =>
                {
                    for (var ww = 0; ww < X; ww++)
                    {
                        var ind = -1;
                        var dist = Int32.MaxValue;
                        for (var it = 0; it < _pointTable.Count; it++)
                        {
                            var p = _pointTable[it];
                            var d = DistanceSqrd(p, ww, hh);
                            if (d < dist)
                            {
                                dist = d;
                                ind = it;
                            }
                        }

                        if (ind > -1)
                        {
                            _regions[ind].Add(new Point { X = ww, Y = hh });
                            var c = _biome[ind].Color;

                            lock (_bmpLock)
                            {
                                _colorMap.SetPixel(ww,hh,Color.FromArgb(ind));
                                _image.SetPixel(ww, hh, c);
                            }
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException("Value " + "Ind cannot be below 0");
                        }
                    }
                });
        }

        private int DistanceSqrd(Point point, int ww, int hh)
        {
            var xd = ww - point.X;
            var yd = hh - point.Y;
            return xd*xd + yd*yd;
        }
    }
}
