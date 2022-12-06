using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace PixelArtProgram.Algorytms
{
    class Hitandmisss :AlgorytmBase
    {
        public Bitmap Function(Bitmap bitmap, int[,] maskHit, int[,] maskMiss)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] r = LockBitmap32(bitmap,  ref data);
            byte[] w = new byte[data.Stride * data.Height];
            int stride = data.Stride;
            const int bpp = 3;

            int border = data.Stride + bpp;
            int[] offsets =
            {
                 stride - bpp,  stride,  stride + bpp,
                        - bpp,       0,           bpp,
                -stride - bpp, -stride, -stride + bpp
            };

            for (int i = border; i < r.Length - border - bpp; i += bpp+1)
            {
                bool hit = true;

                for (int j = 0; j < offsets.Length; j++)
                {
                    int o = offsets[j];
                    int val = r[i + o + 0] + r[i + o + 1] + r[i + o + 2] > 128 * 3 ? 1 : 0;

                    if (maskHit[j / 3, j % 3] == val)
                        hit = false;
                }

                w[i + 0] = w[i + 1] = w[i + 2] =
                    hit ? byte.MaxValue : byte.MinValue;
            }

            Marshal.Copy(w, 0, data.Scan0, w.Length);
            bitmap.UnlockBits(data);
            return bitmap;
        }
        public Bitmap Function2(Bitmap bitmap, int[,] maskHit, int[,] maskMiss)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapDataIn = LockBitmap32(bitmap, ref data);
            byte[] bitmapDataout = new byte[data.Stride * data.Height];
            int stride = data.Stride;

            int maskLength = (int)maskHit.GetLength(0);
            int w = (maskLength - 1) / 2;

            for (int i = 0; i < bitmapDataIn.Length; i += 4)
            {
                int outputGrayscale = 0;
                bool allHit = true;
                bool allMiss = true;

                int y = 0;
                int x = 0;

                for (int a = i - (w * stride); a <= i + (w * stride); a += stride)
                {
                    for (int b = a - (w * 4); b <= a + (w * 4); b += 4)
                    {
                        int index = b;

                        if (index < 0 || index >= bitmapDataIn.Length) continue;

                        int value = (bitmapDataIn[index + 0] + bitmapDataIn[index + 1] + bitmapDataIn[index + 2]) / 3;

                        if (value < 255 && maskHit[x, y] == 1)
                        {
                            allHit = false;
                        }

                        y++;
                    }

                    y = 0;
                    x++;
                }

                y = 0;
                x = 0;

                for (int a = i - (w * stride); a <= i + (w * stride); a += stride)
                {
                    for (int b = a - (w * 4); b <= a + (w * 4); b += 4)
                    {
                        int index = b;

                        if (index < 0 || index >= bitmapDataIn.Length) continue;

                        int value = (bitmapDataIn[index + 0] + bitmapDataIn[index + 1] + bitmapDataIn[index + 2]) / 3;

                        if (value > 0 && maskMiss[x, y] == 1)
                        {
                            allMiss = false;
                        }

                        y++;
                    }

                    y = 0;
                    x++;
                }


                if (allHit && allMiss)
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

