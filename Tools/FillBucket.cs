using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram
{
    public class FillBucket : Bucket
    {
        public FillBucket(Color color): base(color){}


        public override void Draw(DrawingBoard drawingBoard, Point mousePosition)
       {
           drawingBoard.GetActiveBitmapLayers().bitmap = Fill(
               drawingBoard.GetPixel(mousePosition),
         drawingBoard.GetActiveBitmapLayers().bitmap,
         Color, 0);
       }

        protected Bitmap Fill(Color Color, Bitmap bitmap, Color ColorPixel, int range)
        {
            Bitmap bitmapResult = new Bitmap(bitmap);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    System.Drawing.Color atpixel = bitmap.GetPixel(x, y);
                    if (GetTolerance(atpixel, Color, range))
                    {
                        bitmapResult.SetPixel(x, y, ColorPixel);
                    }
                }
            }
            return bitmapResult;
        }
    }
}
