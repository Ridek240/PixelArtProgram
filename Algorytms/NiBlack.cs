using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Algorytms
{
    class NiBlack : AlgorytmBase
    {
        public Bitmap Function(Bitmap bitmap, int w = 2, float k = -0.2f)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapDataIn = LockBitmap(bitmap, ref data);
            byte[] bitmapDataout = new byte[data.Stride * data.Height];

            Marshal.Copy(data.Scan0, bitmapDataout, 0, bitmapDataout.Length);

            int dy = data.Height, dx = data.Stride;

            for (int i = w + 1; i < dx - w; i++)
            {
                for (int j = w + 1; j < dy - w; j++)
                {
                    List<double> neighbours = new List<double>();
                    // Extract the neighbourhood area
                    for (int x = i - w; x < i + w; x++)
                    {
                        for (int y = j - w; y < j + w; y++)
                        {
                            float bbb = bitmapDataIn[x + y * data.Stride] + bitmapDataIn[x + y * data.Stride + 1] + bitmapDataIn[x + y * data.Stride + 2];
                            bbb /= 3;
                            neighbours.Add(bbb);
                        }
                    }
                    //block = bitmapData[i - w:i + w, j - w:j + w];

                    // Calculate the mean and standard deviation of the neighbourhood region
                    float wBmn = (float)Median(neighbours);
                    float wBstd = (float)standardDeviation(neighbours);

                    // Calculate the threshold value
                    float wBTH = (wBmn + k * wBstd);

                    // Threshold the pixel
                    float aaa = bitmapDataIn[i + j * data.Stride] +
                        bitmapDataIn[i + j * data.Stride + 1] +
                        bitmapDataIn[i + j * data.Stride + 2];
                    aaa /= 3;
                    bitmapDataout[i + j * data.Stride] =
                        bitmapDataout[i + j * data.Stride + 1] =
                        bitmapDataout[i + j * data.Stride + 2] =
                        aaa < wBTH ? byte.MinValue : byte.MaxValue;
                }
            }

            Marshal.Copy(bitmapDataout, 0, data.Scan0, bitmapDataout.Length);
            bitmap.UnlockBits(data);

            return RemoveColorPixels(bitmap);
        }

        public double Median(List<double> numbers)
        {
            if (numbers.Count == 0)
                return 0;

            numbers = numbers.OrderBy(n => n).ToList();

            var halfIndex = numbers.Count() / 2;

            if (numbers.Count() % 2 == 0)
                return (numbers[halfIndex] + numbers[halfIndex - 1]) / 2.0;

            return numbers[halfIndex];
        }

        static double standardDeviation(IEnumerable<double> sequence)
        {
            double result = 0;

            if (sequence.Any())
            {
                double average = sequence.Average();
                double sum = sequence.Sum(d => Math.Pow(d - average, 2));
                result = Math.Sqrt((sum) / sequence.Count());
            }
            return result;
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
