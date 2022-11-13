using System;

using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Algorytms
{
    public class Otsu : BinaryTreshhold
    {
        public Bitmap OtsuMethod(Bitmap bitmap)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapData = LockBitmap24(bitmap, ref data);
            int[] histogramValues = GetHistogram(bitmapData);

            float weightedSumMax = 0;
            for (int i = 0; i < 256; i++)
            {
                weightedSumMax += i * histogramValues[i];
            }

            int total = data.Height * data.Width;
            int sumBefore = 0;
            int threshold = 0;
            float weightedSumBefore = 0;
            float maxvalue = 0;

            for (int i = 0; i < 256; i++)
            {
                sumBefore += histogramValues[i];
                if (sumBefore <= 0) continue;

                int sumAfter = total - sumBefore;
                if (sumAfter <= 0) break;

                weightedSumBefore += (float)(i * histogramValues[i]);

                float mB = weightedSumBefore / sumBefore;
                float mF = (weightedSumMax - weightedSumBefore) / sumAfter;

                float varBetween = (float)sumBefore * sumAfter * (mB - mF) * (mB - mF);
                if (varBetween > maxvalue)
                {
                    maxvalue = varBetween;
                    threshold = i;
                }
            }
            bitmap.UnlockBits(data);
            return BinaryThreshold(bitmap, (byte)threshold, true);
        }
    }
}
