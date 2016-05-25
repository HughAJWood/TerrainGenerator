using System;

namespace TerrainGenerator.General
{
    internal static class Rng
    {
        // Somewhat better code...
        private static readonly Random _rng;

        static Rng()
        {
            var seconds = unchecked((int) (DateTime.Now.Ticks/TimeSpan.TicksPerSecond));
            _rng = new Random(seconds);
        }

        internal static int Roll(int max, int min = 0)
        {
            return _rng.Next(min, max);
        }

        internal static Random Raw()
        {
            return _rng;
        }
    }
}
