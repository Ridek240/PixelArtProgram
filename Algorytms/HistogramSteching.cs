using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Algorytms
{
    public class  HistogramSteching : AlgorytmBase
    {

        public Bitmap Function(Bitmap bitmap, int minValue, int maxValue)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapData = LockBitmap(bitmap, ref data);

            for (int i = 0; i < bitmapData.Length; i++)
            {
                int diff = (bitmapData[i] - minValue >= 0) ? (bitmapData[i] - minValue) : 0;
                bitmapData[i] = (byte)((float)diff / (maxValue - minValue) * 255);
            }

            Marshal.Copy(bitmapData, 0, data.Scan0, bitmapData.Length);

            bitmap.UnlockBits(data);

            return bitmap;
        }

    }
}
