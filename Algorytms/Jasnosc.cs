using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Algorytms
{
    class Jasnosc
    {
        public Bitmap Function(Bitmap bin, int Val)
        {
            byte[] LUT = new byte[256];
            int b = Val;
            for (int i = 0; i < 256; i++)
            {
                if ((b + i) > 255)
                {
                    LUT[i] = 255;
                }
                else if ((b + i) < 0)
                {
                    LUT[i] = 0;
                }
                else
                {
                    LUT[i] = (byte)(b + i);
                }
            }


            Bitmap bitmap = new Bitmap(bin);

            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            byte[] pixelValues = new byte[Math.Abs(bmpData.Stride) * bitmap.Height];
            Marshal.Copy(bmpData.Scan0, pixelValues, 0, pixelValues.Length);

            for (int i = 0; i < pixelValues.Length; i++)
            {
                pixelValues[i] = LUT[pixelValues[i]];
            }

            Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bitmap.UnlockBits(bmpData);

            return bitmap;
        }
    }
}
