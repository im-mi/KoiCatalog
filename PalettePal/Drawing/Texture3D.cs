using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace PalettePal
{
    public class Texture3D
    {
        public Vector3i Size =>
            new Vector3i(Pixels.GetLength(0), Pixels.GetLength(1), Pixels.GetLength(2));
        private Vector3[,,] Pixels { get; }

        public Texture3D(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            int[] bitmapArray;
            using (var image = Image.FromStream(stream))
            using (var bitmap = new Bitmap(image))
            {
                BitmapData bitmapData = null;
                try
                {
                    bitmapData = bitmap.LockBits(
                        new Rectangle(Point.Empty, bitmap.Size),
                        ImageLockMode.ReadOnly,
                        PixelFormat.Format32bppArgb);

                    Trace.Assert(bitmapData.Stride % sizeof(int) == 0);
                    bitmapArray = new int[(bitmapData.Stride / sizeof(int)) * bitmapData.Height];
                    Marshal.Copy(bitmapData.Scan0, bitmapArray, 0, bitmapArray.Length);

                    Pixels = new Vector3[
                        bitmapData.Height,
                        bitmapData.Height,
                        bitmapData.Width / bitmapData.Height
                    ];
                }
                finally
                {
                    if (bitmapData != null)
                        bitmap.UnlockBits(bitmapData);
                }
            }

            var size = Size;
            var w = size[0];
            var h = size[1];
            var d = size[2];
            var stride = w * d;

            for (var i = 0; i < bitmapArray.Length; i++)
            {
                var x = i % w;
                var y = i / stride;
                y = (h - 1) - y; // Invert y axis
                var z = (i / w) % d;

                var color = Color.FromUint((uint)bitmapArray[i]);
                var rgb = ColorConversion.ColorToRgb(color);
                Pixels[x, y, z] = rgb;
            }
        }

        public Vector3 GetPixel(Vector3i pixel)
        {
            return Pixels[pixel.X, pixel.Y, pixel.Z];
        }

        public Vector3 SamplePixel(Vector3 pixel)
        {
            var iPixel = new Vector3i();
            for (var i = 0; i < pixel.Length; i++)
            {
                iPixel[i] = (int)Math.Round(pixel[i]);
            }
            return SamplePixel(iPixel);
        }

        public Vector3 SamplePixel(Vector3i pixel)
        {
            if (Pixels.Length == 0)
                return Vector3.Zero;
            
            var size = Size;
            for (var i = 0; i < pixel.Length; i++)
            {
                pixel[i] = MathEx.Clamp(pixel[i], 0, size[i] - 1);
            }

            return GetPixel(pixel);
        }

        public Vector3 SampleNearest(Vector3 uvw)
        {
            return SamplePixel(uvw * Size);
        }

        public Vector3 SampleLinear(Vector3 uvw)
        {
            uvw = uvw * Size - 0.5f;
            var pixel = new Vector3i();
            for (var i = 0; i < pixel.Length; i++)
                pixel[i] = (int)Math.Floor(uvw[i]);
            var ratio = uvw - pixel;

            return Vector3.Lerp(
                Vector3.Lerp(
                    Vector3.Lerp(
                        SamplePixel(pixel + new Vector3i(0, 0, 0)),
                        SamplePixel(pixel + new Vector3i(1, 0, 0)),
                        ratio[0]),
                    Vector3.Lerp(
                        SamplePixel(pixel + new Vector3i(0, 1, 0)),
                        SamplePixel(pixel + new Vector3i(1, 1, 0)),
                        ratio[0]),
                    ratio[1]),
                Vector3.Lerp(
                    Vector3.Lerp(
                        SamplePixel(pixel + new Vector3i(0, 0, 1)),
                        SamplePixel(pixel + new Vector3i(1, 0, 1)),
                        ratio[0]),
                    Vector3.Lerp(
                        SamplePixel(pixel + new Vector3i(0, 1, 1)),
                        SamplePixel(pixel + new Vector3i(1, 1, 1)),
                        ratio[0]),
                    ratio[1]),
                ratio[2]);
        }
    }
}
