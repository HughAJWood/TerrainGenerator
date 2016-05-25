using System;
using System.Drawing;
using System.Drawing.Imaging;
using TerrainGenerator.General;

namespace TerrainGenerator.NoiseGeneration
{
    internal class DiamondSquare
    {
        private readonly int _h;
        private readonly int _w;
        private readonly Bitmap _image;
        internal DiamondSquare(Bitmap image)
        {
            _image = image;
            _w = image.Width;
            _h = image.Height;
        }

        internal Bitmap AlgorithmMain(int stepsize, double scale)
        {
            Console.WriteLine("Rendering DiamondSquare noise map");
            var halfstep = stepsize / 2;
            for (var y = 0; y < _h; y += stepsize)
            {
                for (var x = 0; x < _w; x += stepsize)
                {
                    SetSample(x, y, (byte)(Rng.Roll(64, -64) * scale));
                }
            }

            while (stepsize > 1)
            {
                for (var y = halfstep; y < _h + halfstep; y += stepsize)
                {
                    for (var x = halfstep; x < _w + halfstep; x += stepsize)
                    {
                        SampleSquare(x, y, stepsize, (byte) (Rng.Roll(64, -64) * scale));
                    }
                }
                
                for (var y = 0; y < _h; y += stepsize)
                {
                    for (var x = 0; x < _w; x += stepsize)
                    {
                        SampleDiamond(x + halfstep, y, stepsize, (byte) (Rng.Roll(64, -64) * scale));
                        SampleDiamond(x, y + halfstep, stepsize, (byte) (Rng.Roll(64, -64) * scale));
                    }
                }
                stepsize /= 2;
                scale /= 1.5;
                halfstep = stepsize / 2;
            }
            return _image;
        }
        private byte Sample(int x, int y)
        {
            if (x >= _w) x = x - _w;
            if (y >= _h) y = y - _h;
            if (x < 0) x = _w + x;
            if (y < 0) y = _h + y;
            return _image.GetPixel(x, y).R;
        }
        private void SetSample(int x, int y, byte b)
        {
            if (x >= _w) x = x - _w;
            if (y >= _h) y = y - _h;
            if (x < 0) x = _w + x;
            if (y < 0) y = _h + y;
            _image.SetPixel(x, y, Color.FromArgb(b,b,b));
        }
        private void SampleSquare(int x, int y, int size, byte value)
        {
            var hs = size / 2;

            // a     b 
            //
            //    x
            //
            // c     d

            var a = Sample(x - hs, y - hs);
            var b = Sample(x + hs, y - hs);
            var c = Sample(x - hs, y + hs);
            var d = Sample(x + hs, y + hs);
            var newColor = (a + b + c + d) / 4;
            var tempColor = newColor + value;
            if (tempColor > 0 && tempColor < 255)
            {
                if (newColor > 128)
                {
                    newColor = newColor - (byte) Rng.Roll(30);
                }
                else
                {
                    newColor = newColor + (byte)Rng.Roll(30);
                }
            }
            SetSample(x, y, (byte)newColor);

        }
        private void SampleDiamond(int x, int y, int size, byte value)
        {
            var hs = size / 2;

            //   c
            //
            //a  x  b
            //
            //   d

            var a = Sample(x - hs, y);
            var b = Sample(x + hs, y);
            var c = Sample(x, y - hs);
            var d = Sample(x, y + hs);

            var newColor = (a + b + c + d) / 4;
            var tempColor = newColor + value;
            if (tempColor > 0 && tempColor < 255) newColor = tempColor;
            SetSample(x, y, (byte)newColor);
        }

    }
}
