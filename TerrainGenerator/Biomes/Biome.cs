using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace TerrainGenerator.Biomes
{
    internal class Biome
    {
        private readonly BiomeNames[,] _biomes =
        {
            { BiomeNames.TropicalRainForest, BiomeNames.TropicalRainForest, BiomeNames.TropicalSeasonalForest, BiomeNames.TropicalSeasonalForest, BiomeNames.Grassland, BiomeNames.SubtropicalDesert },
            { BiomeNames.TemperateRainForest, BiomeNames.TemperateDeciduousForest, BiomeNames.TemperateDeciduousForest, BiomeNames.Grassland, BiomeNames.Grassland, BiomeNames.TemperateDesert },
            { BiomeNames.Taiga, BiomeNames.Taiga, BiomeNames.Shrubland, BiomeNames.Shrubland, BiomeNames.TemperateDesert, BiomeNames.TemperateDesert },
            { BiomeNames.Snow, BiomeNames.Snow, BiomeNames.Snow, BiomeNames.Tundra, BiomeNames.Bare, BiomeNames.Scorched }
        };

        public Color Color
        {
            get
            {
                var biome = _elevation == -1 ? BiomeNames.Sea : _biomes[_elevation - 1, _moistureLevel - 1];
                switch (biome)
                {
                    case BiomeNames.Sea:
                        return Color.DarkBlue;
                    case BiomeNames.Snow:
                        return Color.White;
                    case BiomeNames.Tundra:
                        return Color.PaleGreen;
                    case BiomeNames.Bare:
                        return Color.Gray;
                    case BiomeNames.Scorched:
                        return Color.SandyBrown;
                    case BiomeNames.Taiga:
                        return Color.ForestGreen;
                    case BiomeNames.Shrubland:
                        return Color.YellowGreen;
                    case BiomeNames.TemperateDesert:
                        return Color.Brown;
                    case BiomeNames.TemperateRainForest:
                        return Color.LimeGreen;
                    case BiomeNames.TemperateDeciduousForest:
                        return Color.DarkGreen;
                    case BiomeNames.Grassland:
                        return Color.SpringGreen;
                    case BiomeNames.TropicalRainForest:
                        return Color.LightSeaGreen;
                    case BiomeNames.TropicalSeasonalForest:
                        return Color.DarkSeaGreen;
                    case BiomeNames.SubtropicalDesert:
                        return Color.DarkSalmon;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        // Get the current biomes name
        private string _biomeName = "";

        public string BiomeName
        {
            get
            {
                if (_elevation == -1)
                {
                    _biomeName = BiomeNames.Sea.ToString();
                } 
                else if (_biomeName == "")
                {
                    _biomeName = _biomes[_elevation - 1, _moistureLevel - 1].ToString();
                    _biomeName = Regex.Replace(_biomeName, "([a-z])([A-Z])", "$1 $2");
                }
                return _biomeName;
            }
        }

        /// <summary>
        /// 1~6 Dry~Wet
        /// </summary>
        private int _moistureLevel = 2;

        /// <summary>
        /// 6~1 Dry~Wet
        /// </summary>
        public int MoistureLevel
        {
            get { return _moistureLevel; }
            set
            {
                if (value == 100)
                {
                    _moistureLevel = value;
                    _elevation = -1;
                    return;
                }
                if (value > 6)
                {
                    throw new ArgumentException("Moisture level maximum 6");
                }
                if (value < 1)
                {
                    throw new ArgumentException("Moisture level minimum 1");
                }
                _moistureLevel = value;
            }
        }

        /// <summary>
        /// 1~4 Low~High
        /// </summary>
        private int _elevation = 1;

        /// <summary>
        /// 1~4 Low~High
        /// </summary>
        internal int Elevation
        {
            get { return _elevation; }

            set
            {
                if (value == -1) return;
                if (value > 4)
                {
                    throw new ArgumentException("Elevation level maximum 4");
                }
                if (value < 1)
                {
                    throw new ArgumentException("Elevation level minimum 1");
                }
                _elevation = value;
            }
        }
    }
}
