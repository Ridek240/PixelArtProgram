using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Algorytms
{
    class Median : AlgorytmBase
    {
        public Bitmap Function(Bitmap bitmap, int w = 2)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapDataIn = LockBitmap24(bitmap, ref data);
            byte[] bitmapDataout = new byte[data.Stride * data.Height];

            Marshal.Copy(data.Scan0, bitmapDataout, 0, bitmapDataout.Length);

            int dy = data.Height, dx = data.Stride / 3;

            for (int i = w + 1; i < dx - w; i++)
            {
                for (int j = w + 1; j < dy - w; j++)
                {
                    //List<double> neighbours = new List<double>();
                    List<double> neighboursR = new List<double>();
                    List<double> neighboursG = new List<double>();
                    List<double> neighboursB = new List<double>();
                    // Extract the neighbourhood area
                    for (int x = i - w; x < i + w; x++)
                    {
                        for (int y = j - w; y < j + w; y++)
                        {
                            //float bbb = bitmapDataIn[x + y * data.Stride] + bitmapDataIn[x + y * data.Stride + 1] + bitmapDataIn[x + y * data.Stride + 2];
                            //bbb /= 3;
                            //neighbours.Add(bbb);
                            neighboursR.Add(bitmapDataIn[x * 3 + y * data.Stride]);
                            neighboursG.Add(bitmapDataIn[x * 3 + y * data.Stride + 1]);
                            neighboursB.Add(bitmapDataIn[x * 3 + y * data.Stride + 2]);
                        }
                    }

                    // Calculate the mean of the neighbourhood region

                    float wBmnR = (float)Med(neighboursR);
                    float wBmnG = (float)Med(neighboursG);
                    float wBmnB = (float)Med(neighboursB);

                    bitmapDataout[i * 3 + j * data.Stride] = (byte)wBmnR;
                    bitmapDataout[i * 3 + j * data.Stride + 1] = (byte)wBmnG;
                    bitmapDataout[i * 3 + j * data.Stride + 2] = (byte)wBmnB;
                }
            }

            Marshal.Copy(bitmapDataout, 0, data.Scan0, bitmapDataout.Length);
            bitmap.UnlockBits(data);

            return bitmap;
        }

        public double Med(List<double> numbers)
        {
            if (numbers.Count == 0)
                return 0;

            numbers = numbers.OrderBy(n => n).ToList();

            var halfIndex = numbers.Count() / 2;

            if (numbers.Count() % 2 == 0)
                return (numbers[halfIndex] + numbers[halfIndex - 1]) / 2.0;

            return numbers[halfIndex];
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
