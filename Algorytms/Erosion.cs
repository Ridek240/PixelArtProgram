using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace PixelArtProgram.Algorytms
{
    class Dilatation : AlgorytmBase
    {
        public Bitmap Function(Bitmap bitmap, int[,] mask)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapDataIn = LockBitmap32(bitmap, ref data);
            byte[] bitmapDataout = new byte[data.Stride * data.Height];
            int stride = data.Stride;

            int maskLength = (int)mask.GetLength(0);
            int w = (maskLength - 1) / 2;

            for (int i = 0; i < bitmapDataIn.Length; i += 4)
            {
                int outputGrayscale = 0;
                bool allHigh = true;

                for (int a = i - (w * stride); a <= i + (w * stride); a += stride)
                {
                    for (int b = a - (w * 4); b <= a + (w * 4); b += 4)
                    {
                        int index = b;

                        if (index < 0 || index >= bitmapDataIn.Length) continue;

                        int value = (bitmapDataIn[index + 0] + bitmapDataIn[index + 1] + bitmapDataIn[index + 2]) / 3;

                        if (value < 255)
                        {
                            allHigh = false;
                        }
                    }
                }

                if (allHigh)
                {
                    outputGrayscale = 255;
                }

                bitmapDataout[i + 0] = (byte)outputGrayscale;
                bitmapDataout[i + 1] = (byte)outputGrayscale;
                bitmapDataout[i + 2] = (byte)outputGrayscale;

                bitmapDataout[i + 3] = (byte)255;
            }

            Marshal.Copy(bitmapDataout, 0, data.Scan0, bitmapDataout.Length);
            bitmap.UnlockBits(data);
            return bitmap;
        }
    }
}
