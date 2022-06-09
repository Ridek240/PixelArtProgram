using System.Collections.Generic;
using System.Drawing;

namespace PixelArtProgram
{
    public class Bucket : DrawingTool
    {
        
        public Bucket(System.Drawing.Color color) : base(color) { }

        public override void Draw(DrawingBoard drawingBoard, Point mousePosition)
        {
            drawingBoard.GetActiveBitmapLayers().bitmap = Fill(
                new System.Drawing.Point(mousePosition.x, mousePosition.y),
                drawingBoard.GetActiveBitmapLayers().bitmap, 
                Color, 0);
        }

        protected Bitmap Fill(System.Drawing.Point position, Bitmap bitmap, Color ColorPixel, int range, int maxIteration = 0)
        {
            //System.Drawing.Color blackpixel = System.Drawing.Color.FromArgb(0, 0, 0);
            System.Drawing.Color pixelCompateTo = bitmap.GetPixel(position.X, position.Y);

            Queue<System.Drawing.Point> pixels = new Queue<System.Drawing.Point>();
            pixels.Enqueue(position);

            bool[,] visited = new bool[bitmap.Width, bitmap.Height];
            Bitmap bitmapResult = new Bitmap(bitmap);

            int iteration = 0;
            while (pixels.Count > 0)
            {
                if (maxIteration > 0 && iteration++ >= maxIteration) break;

                System.Drawing.Point pixel = pixels.Dequeue();

                if (pixel.X < 0 || pixel.X >= bitmap.Width ||
                    pixel.Y < 0 || pixel.Y >= bitmap.Height ||
                    visited[pixel.X, pixel.Y]) continue;

                visited[pixel.X, pixel.Y] = true;

                if (GetTolerance(bitmap.GetPixel(pixel.X, pixel.Y), pixelCompateTo, range))
                {
                    bitmapResult.SetPixel(pixel.X, pixel.Y, ColorPixel);
                    foreach (System.Drawing.Point neighbour in neighbours)
                    {
                        System.Drawing.Point nPixel = new System.Drawing.Point(
                            pixel.X + neighbour.X,
                            pixel.Y + neighbour.Y);
                        pixels.Enqueue(nPixel);
                    }
                }
            }

            return bitmapResult;
        }

        protected bool GetTolerance(Color atpixel, Color pixel, int range)
        {
            return atpixel.R >= pixel.R - range
                && atpixel.R <= pixel.R + range & atpixel.G >= pixel.G - range
                && atpixel.G <= pixel.G + range & atpixel.B >= pixel.B - range
                && atpixel.B <= pixel.B + range;
        }

        private static System.Drawing.Point[] neighbours =
        {
            new System.Drawing.Point(-1, 0),
            new System.Drawing.Point(1, 0),
            new System.Drawing.Point(0, 1),
            new System.Drawing.Point(0, -1)
        };
    }
}
