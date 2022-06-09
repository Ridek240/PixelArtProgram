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
        public FillBucket(DrawingBoard _drawingBoard, Color color) : base(_drawingBoard, color) { }

        public override void Draw(Point mousePosition)
        {
            drawingBoard.GetActiveBitmapLayer().bitmap = Fill(
                drawingBoard.GetPixel(mousePosition),
                drawingBoard.GetActiveBitmapLayer().bitmap,
                Color, 0);
        }

        protected Bitmap Fill(Color Color, Bitmap bitmap, Color ColorPixel, int range)
        {
            Bitmap bitmapResult = new Bitmap(bitmap);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color atpixel = bitmap.GetPixel(x, y);
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
