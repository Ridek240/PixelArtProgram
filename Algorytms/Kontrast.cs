using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Algorytms
{
    class Kontrast
    {
         public Bitmap Function(Bitmap bitmap, float Value)
        {
            byte[] LUT = new byte[256];
            double a;
            if (Value <= 0)
            {
                a = 1.0 + (Value / 256.0);
            }
            else
            {
                a = 256.0 / Math.Pow(2, Math.Log(257 - Value, 2));
            }
            for (int i = 0; i < 256; i++)
            {
                if ((a * (i - 127) + 127) > 255)
                {
                    LUT[i] = 255;
                }
                else if ((a * (i - 127) + 127) < 0)
                {
                    LUT[i] = 0;
                }
                else
                {
                    LUT[i] = (byte)(a * (i - 127) + 127);
                }
            }



            Bitmap bitmapcopy = new Bitmap(bitmap);

            BitmapData bmpData = bitmapcopy.LockBits(new Rectangle(0, 0, bitmapcopy.Width, bitmapcopy.Height), ImageLockMode.ReadWrite, bitmapcopy.PixelFormat);
            byte[] pixelValues = new byte[Math.Abs(bmpData.Stride) * bitmap.Height];
            Marshal.Copy(bmpData.Scan0, pixelValues, 0, pixelValues.Length);

            for (int i = 0; i < pixelValues.Length; i++)
            {
                pixelValues[i] = LUT[pixelValues[i]];
            }

            Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bitmapcopy.UnlockBits(bmpData);

            return bitmapcopy; 
        }
    }
}

