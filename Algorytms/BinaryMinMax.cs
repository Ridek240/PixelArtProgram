using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Algorytms
{
    class BinaryMinMax : AlgorytmBase
    {
        public Bitmap Function(Bitmap bitmap, int width, int height)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapDataIn = LockBitmap24(bitmap, ref data);
            byte[] bitmapDataout = new byte[data.Stride * data.Height];

            Marshal.Copy(data.Scan0, bitmapDataout, 0, bitmapDataout.Length);

            int dy = data.Height, dx = data.Stride / 3;

            int wX = (width - 1) / 2;
            int wY = (height - 1) / 2;

            for (int j = 0; j < dy; j++)
            {
                for (int i = 0; i < dx; i++)
                {
                    byte rMin = byte.MaxValue, rMax = byte.MinValue;
                    byte gMin = byte.MaxValue, gMax = byte.MinValue;
                    byte bMin = byte.MaxValue, bMax = byte.MinValue;
                    for (int x = i - wX; x <= i + wX; x++)
                    {
                        for (int y = j - wY; y <= j + wY; y++)
                        {
                            if (x < 0 || x >= dx ||
                                y < 0 || y >= dy) continue;

                            int index = (x) * 3 + (y) * data.Stride;
                            byte r = bitmapDataIn[index];
                            byte g = bitmapDataIn[index + 1];
                            byte b = bitmapDataIn[index + 2];
                            if (rMin > r) rMin = r;
                            if (rMax < r) rMax = r;

                            if (gMin > g) gMin = g;
                            if (gMax < g) gMax = g;

                            if (bMin > b) bMin = b;
                            if (bMax < b) bMax = b;
                        }
                    }

                    float thresholdR = (float)Math.Clamp(rMax - rMin, 0, 255) / (float)Math.Clamp(rMax + rMin, 0, 255);
                    float thresholdG = (float)Math.Clamp(gMax - gMin, 0, 255) / (float)Math.Clamp(gMax + gMin, 0, 255);
                    float thresholdB = (float)Math.Clamp(bMax - bMin, 0, 255) / (float)Math.Clamp(bMax + bMin, 0, 255);

                    int idx = i * 3 + j * data.Stride;

                    bitmapDataout[idx    ] = bitmapDataIn[idx    ] > thresholdR ? byte.MaxValue : byte.MinValue;
                    bitmapDataout[idx + 1] = bitmapDataIn[idx + 1] > thresholdG ? byte.MaxValue : byte.MinValue;
                    bitmapDataout[idx + 2] = bitmapDataIn[idx + 2] > thresholdB ? byte.MaxValue : byte.MinValue;
                }
            }

            Marshal.Copy(bitmapDataout, 0, data.Scan0, bitmapDataout.Length);
            bitmap.UnlockBits(data);

            return bitmap;
        }
    }
}
