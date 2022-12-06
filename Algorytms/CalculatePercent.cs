using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace PixelArtProgram.Algorytms
{
    internal class CalculatePercent : AlgorytmBase
    {
        public float Function(Bitmap bitmap, byte[] val) => Function(bitmap, val, val);
        public float Function(Bitmap bitmap, byte[] minVal, byte[] maxVal)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapData = LockBitmap24(bitmap, ref data);

            int count = 0;

            for (int i = 0; i < bitmapData.Length; i += 3)
            {
                if (bitmapData[i + 0] >= minVal[0] && bitmapData[i + 0] <= maxVal[0] &&
                    bitmapData[i + 1] >= minVal[1] && bitmapData[i + 1] <= maxVal[1] &&
                    bitmapData[i + 2] >= minVal[2] && bitmapData[i + 2] <= maxVal[2])
                    count++;
            }

            bitmap.UnlockBits(data);

            return (float)count / (bitmapData.Length / 3);
        }
    }
}
