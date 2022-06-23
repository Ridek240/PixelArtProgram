using System.Drawing;

namespace PixelArtProgram.Tools
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
                    if (atpixel.A == Color.A && GetTolerance(atpixel, Color, range))
                    {
                        bitmapResult.SetPixel(x, y, ColorPixel);
                    }
                }
            }
            return bitmapResult;
        }
    }
}
