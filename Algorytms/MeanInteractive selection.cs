using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace PixelArtProgram.Algorytms
{
    class MeanInteractiveSelection : BinaryTreshhold
    {
        public Bitmap Function(Bitmap bitmap)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapData = LockBitmap32(bitmap, ref data);
            int[] histogramValues = GetHistogram(bitmapData);
            int total = data.Height * data.Width;

            int[] bitmapdataint = bitmapData.Select(x => (int)x).ToArray();

            int init = bitmapdataint.Sum() / bitmapData.Length;
            int delta = 1;
            bitmap.UnlockBits(data);

            while(delta>0)
            {
                int mean1 = 0;
                int mean2 = 0;
                int sum1 = 0;
                int sum2 = 0;

                for(int i = 0; i < 255; i++)
                {
                    if(i<=init)
                    {
                        mean1 += histogramValues[i] * i;
                        sum1 += histogramValues[i];
                    }
                    else
                    {
                        mean2 += histogramValues[i] * i;
                        sum2 += histogramValues[i];
                    }
                }

                mean1 /= sum1;
                mean2 /= sum2;
                delta = init;
                init = (mean1 + mean2) / 2;
                delta = Math.Abs(delta - init);

            }



            return BinaryThreshold(bitmap, (byte)init, true); ;
        }
    }
}
