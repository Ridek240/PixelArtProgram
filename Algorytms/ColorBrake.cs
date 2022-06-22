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
    public class ColorBrake : SplitLayer
    {
        public bool Function(Bitmap bitmap,Color color)
        {
            Background = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Object = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int x = 0; x < bitmap.Height; x++)
                {
                    if(bitmap.GetPixel(i,x)==color)
                    {
                        Object.SetPixel(i, x, bitmap.GetPixel(i, x));
                        
                    }
                    else
                    {
                        Background.SetPixel(i, x, bitmap.GetPixel(i, x));
                    }
                }
            }
                    
            
            
            return true;
        }

    }
}
