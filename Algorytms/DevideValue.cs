using System.Drawing;
using System.Runtime.InteropServices;

namespace PixelArtProgram.Algorytms
{
    class DevideValue : AlgorytmBase
    {
        public Bitmap Function(Bitmap bitmap, float val, bool invidula, float r, float g, float b)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapDataIn = LockBitmap32(bitmap, ref data);
            if((!invidula && val ==0)||(invidula && (r==0 || b ==9 || g ==0)))
            {
                return bitmap;
            }


            int size = bitmapDataIn.Length;
            for (int i = 0; i < size; i += 4)
            {

                if (bitmapDataIn[i + 3] == 255)
                {
                    bitmapDataIn[i] = invidula ? AddIncrement(bitmapDataIn[i], r) : AddIncrement(bitmapDataIn[i], val);
                    bitmapDataIn[i + 1] = invidula ? AddIncrement(bitmapDataIn[i + 1], g) : AddIncrement(bitmapDataIn[i + 1], val);
                    bitmapDataIn[i + 2] = invidula ? AddIncrement(bitmapDataIn[i + 2], b) : AddIncrement(bitmapDataIn[i + 2], val);
                }
            }

            Marshal.Copy(bitmapDataIn, 0, data.Scan0, bitmapDataIn.Length);
            bitmap.UnlockBits(data);
            return bitmap;
        }


        public byte AddIncrement(byte value, float increment)
        {
            int output = (int)(value / increment);
            if (output >= 255)
            {
                output = 255;
            }
            else if (output <= 0)
            {
                output = 0;
            }

            return (byte)output;
        }
    }
}
