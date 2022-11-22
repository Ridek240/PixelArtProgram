using System;

using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Algorytms
{
    public class MinErrorThreshold : BinaryTreshhold
    {
        public Bitmap MinErrorThresholdMethod(Bitmap bitmap)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapData = LockBitmap24(bitmap, ref data);
            int[] histogramValues = GetHistogram(bitmapData);

            float minValue = 0;
            int threshold = 0;
            for (int i = 0; i < 256; i++)
            {
                float w0 = csum(histogramValues, i);
                float w1 = dsum(histogramValues, i);
                float u0 = csumX(histogramValues, i) / w0;
                float u1 = dsumX(histogramValues, i) / w1;
                float d0 = csumX2(histogramValues, i) - (w0 * u0 * u0);
                float d1 = dsumX2(histogramValues, i) - (w1 * u1 * u1);

                float l = (float)(1 + w0 * Math.Log(d0 / w0) + w1 * Math.Log(d1 / w1) - 2 * (w0 * Math.Log(w0) + w1 * Math.Log(w1)));

                if (i == 0) minValue = l;
                if (l < minValue)
                {
                    minValue = l;
                    threshold = i;
                }
            }
            bitmap.UnlockBits(data);
            return BinaryThreshold(bitmap, (byte)threshold, true);
        }

        float csum(int[] histogramValues, int n)
        {
            float result = 0;
            for (int i = 0; i < n; i++)
            {
                result += histogramValues[i];
            }
            return result;
        }

        float dsum(int[] histogramValues, int n)
        {
            float result = 0;
            for (int i = 255; i >= n; i--)
            {
                result += histogramValues[i];
            }
            return result;
        }

        float csumX(int[] histogramValues, int n)
        {
            float result = 0;
            for (int i = 0; i < n; i++)
            {
                result += histogramValues[i] * i;
            }
            return result;
        }

        float dsumX(int[] histogramValues, int n)
        {
            float result = 0;
            for (int i = 255; i >= n; i--)
            {
                result += histogramValues[i] * i;
            }
            return result;
        }

        float csumX2(int[] histogramValues, int n)
        {
            float result = 0;
            for (int i = 0; i < n; i++)
            {
                result += histogramValues[i] * i * i;
            }
            return result;
        }

        float dsumX2(int[] histogramValues, int n)
        {
            float result = 0;
            for (int i = 255; i >= n; i--)
            {
                result += histogramValues[i] * i * i;
            }
            return result;
        }
    }
}
