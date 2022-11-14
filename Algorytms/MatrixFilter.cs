using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Algorytms
{
    class MatrixFilter : AlgorytmBase
    {
        public Bitmap Function(Bitmap bitmap, float[,] matrix)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapDataIn = LockBitmap24(bitmap, ref data);
            byte[] bitmapDataout = new byte[data.Stride * data.Height];

            Marshal.Copy(data.Scan0, bitmapDataout, 0, bitmapDataout.Length);

            int dy = data.Height, dx = data.Stride;

            int w = (matrix.Length - 1) / 2;

            for (int i = w + 1; i < dx - w; i++)
            {
                for (int j = w + 1; j < dy - w; j++)
                {
                    List<double> neighbours = new List<double>();
                    float pixelValue = 0;
                    // Extract the neighbourhood area
                    for (int x = i - w; x < i + w; x++)
                    {
                        for (int y = j - w; y < j + w; y++)
                        {
                            float bbb = bitmapDataIn[x + y * data.Stride] + bitmapDataIn[x + y * data.Stride + 1] + bitmapDataIn[x + y * data.Stride + 2];
                            bbb /= 3;
                            pixelValue += bbb * matrix[x, y];
                            //neighbours.Add(bbb);
                        }
                    }

                    bitmapDataout[i + j * data.Stride] =
                        bitmapDataout[i + j * data.Stride + 1] =
                        bitmapDataout[i + j * data.Stride + 2] =
                        (byte)pixelValue;
                }
            }

            Marshal.Copy(bitmapDataout, 0, data.Scan0, bitmapDataout.Length);
            bitmap.UnlockBits(data);

            return RemoveColorPixels(bitmap);
        }

        private Bitmap RemoveColorPixels(Bitmap bitmap)
        {
            Bitmap output = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int x = 0; x < bitmap.Height; x++)
                {
                    if(bitmap.GetPixel(i, x) == Color.FromArgb(255, 255,255,255))
                    {
                        output.SetPixel(i, x, bitmap.GetPixel(i, x));
                    }
                    else
                    {
                        output.SetPixel(i, x, Color.FromArgb(255, 0,0,0));
                    }
                }
            }

            return output;
        }
    }
}
