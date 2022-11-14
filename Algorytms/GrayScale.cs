using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace PixelArtProgram.Algorytms
{
    class GrayScale : AlgorytmBase
    {
        public Bitmap Function(Bitmap bitmap, bool red, bool green, bool blue)
        {
            if (red == false && blue == false && green == false) return bitmap;

            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapDataIn = LockBitmap32(bitmap, ref data);

            int size = bitmapDataIn.Length;
            for (int i = 0; i < size; i += 4)
            {
                int treshold = (bitmapDataIn[i] * BooltoInt(red) + bitmapDataIn[i + 1] * BooltoInt(green) + bitmapDataIn[i + 2] * BooltoInt(blue)) / (BooltoInt(red) + BooltoInt(green) + BooltoInt(blue));
                bitmapDataIn[i] = (byte)treshold;
                bitmapDataIn[i + 1] = (byte)treshold;
                bitmapDataIn[i + 2] = (byte)treshold;
            }

            Marshal.Copy(bitmapDataIn, 0, data.Scan0, bitmapDataIn.Length);
            bitmap.UnlockBits(data);
            return bitmap;
        }

        public int BooltoInt(bool val)
        {
            if (val) return 1;
            else return 0;
        }
    }
}
