using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Algorytms
{
    class Bernsen : AlgorytmBase
    {
        public Bitmap Fuction(Bitmap bitmap, int w = 2, int l = 15)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapDataIn = LockBitmap24(bitmap, ref data);
            byte[] bitmapDataout = new byte[data.Stride * data.Height];

            Marshal.Copy(data.Scan0, bitmapDataout, 0, bitmapDataout.Length);

            int dy = data.Height, dx = data.Stride;
            for (int i = 0; i < bitmapDataout.Length; i++)
                bitmapDataout[i] = byte.MaxValue;

            for (int i = w; i < dx - w; i++)
            {
                for (int j = w; j < dy - w; j++)
                {
                    List<double> neighbours = GetMask(bitmapDataIn, i, j, w, data);

                    float wMin = (float)neighbours.Min();
                    float wMax = (float)neighbours.Max();

                    float wBTH = (wMin + wMax) / 2;
                    float localContrast = wMax - wMin;

                    int index = i + j * data.Stride;

                    float aaa = bitmapDataIn[index] +
                        bitmapDataIn[index + 1] +
                        bitmapDataIn[index + 2];
                    aaa /= 3;

                    // A
                    //bitmapDataout[index] =
                    //    bitmapDataout[index + 1] =
                    //    bitmapDataout[index + 2] =
                    //    aaa < wBTH ? byte.MinValue : byte.MaxValue;

                    // B
                    if (localContrast < l)
                    {
                        bitmapDataout[index] =
                        bitmapDataout[index + 1] =
                        bitmapDataout[index + 2] =
                        wBTH >= 128 ? byte.MaxValue : byte.MinValue;
                    }
                    else
                    {
                        bitmapDataout[index] =
                        bitmapDataout[index + 1] =
                        bitmapDataout[index + 2] =
                        aaa > wBTH ? byte.MaxValue : byte.MinValue;
                    }
                }
            }

            Marshal.Copy(bitmapDataout, 0, data.Scan0, bitmapDataout.Length);
            bitmap.UnlockBits(data);

            return bitmap;
        }
        public List<double> GetMask(byte[] bitmapData, int i, int j, int w, System.Drawing.Imaging.BitmapData data)
        {
            List<double> neighbours = new List<double>();
            // Extract the neighbourhood area
            for (int x = i - w; x < i + w; x++)
            {
                for (int y = j - w; y < j + w; y++)
                {
                    float bbb = bitmapData[x + y * data.Stride] + bitmapData[x + y * data.Stride + 1] + bitmapData[x + y * data.Stride + 2];
                    bbb /= 3;
                    neighbours.Add(bbb);
                }
            }
            return neighbours;
        }
    }
}
