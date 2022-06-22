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
    class HistogramEqual : AlgorytmBase
    {
        public Bitmap Function(Bitmap bitmap)
        {
            int[] red = new int[256];
            int[] green = new int[256];
            int[] blue = new int[256];

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    System.Drawing.Color color = bitmap.GetPixel(x, y);
                    red[color.R]++;
                    green[color.G]++;
                    blue[color.B]++;
                }
            }

            int[] LUTred = calculateLUTequal(red, bitmap.Width * bitmap.Height);
            int[] LUTgreen = calculateLUTequal(green, bitmap.Width * bitmap.Height);
            int[] LUTblue = calculateLUTequal(blue, bitmap.Width * bitmap.Height);
            Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height, bitmap.PixelFormat);


            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    Color newpixel = Color.FromArgb(LUTred[pixel.R], LUTgreen[pixel.G], LUTblue[pixel.B]);
                    newBitmap.SetPixel(x, y, newpixel);
                }
            }
            return newBitmap;
        }

        private int[] calculateLUTequal(int[] values, int size)
        {
            double minValue = values.Min();
            int[] output = new int[256];
            double Dn = 0;
            for (int i = 0; i < 256; i++)
            {
                Dn += values[i];
                output[i] = (int)(((Dn - minValue) / (size - minValue)) * 255.0);
            }

            return output;
        }
    }
}
