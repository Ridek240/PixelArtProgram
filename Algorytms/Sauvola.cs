using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Algorytms
{
    class Sauvola : AlgorytmBase
    {
        public Bitmap Function(Bitmap bitmap, int w = 2, float k = 0.2f, int R = 125)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapDataIn = LockBitmap24(bitmap, ref data);
            byte[] bitmapDataout = new byte[data.Stride * data.Height];

            Marshal.Copy(data.Scan0, bitmapDataout, 0, bitmapDataout.Length);

            int dy = data.Height, dx = data.Stride;
            //int imgSize = data.Height * data.Width;
            //imgN = copy(img);

            // Calculate the radius of the neighbourhood
            //int w = (n - 1) / 2;

            // Process the image
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
                    float wBTH = wBmn * (1 - k * (1 - wBstd / R));

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

            return bitmap;
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
    }
}
