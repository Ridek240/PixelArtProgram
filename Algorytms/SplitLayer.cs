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
    public class SplitLayer : Otsu
    {
        public Bitmap Background;
        public Bitmap Object;

        public bool Function(Bitmap bitmap)
        {
            Bitmap save = new Bitmap(bitmap);
            Bitmap Binary = OtsuMethod(bitmap);


            Background = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Object = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            for(int i = 0; i < bitmap.Width; i++) 
            {
                for(int x = 0; x < bitmap.Height; x++)
                {
                    if(Binary.GetPixel(i,x)==Color.FromArgb(255,255,255,255))
                    {
                        Background.SetPixel(i, x, save.GetPixel(i, x));
                    }
                    else if (Binary.GetPixel(i, x) == Color.FromArgb(255, 0, 0, 0))
                    {
                        Object.SetPixel(i, x, save.GetPixel(i, x));
                    }

                }
            }

            return true;
        }

    }
}
