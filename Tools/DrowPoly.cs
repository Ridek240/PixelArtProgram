using System.Collections.Generic;
using System.Drawing;

namespace PixelArtProgram.Tools
{
    public class DrawPoly : DrawObject
    {
        List<Point> Polypoin;
        public DrawPoly(DrawingBoard _drawingBoard, Color color, Point StartingPoint, List<Point> points) : base(_drawingBoard, color, StartingPoint) {
            Polypoin = points;
            Polypoin.Add(StartingPoint);
        }

        public override void Draw(Point mousePosition)
        {
            System.Drawing.Point[] points = new System.Drawing.Point[Polypoin.Count];
            int i = 0;
            foreach (var item in Polypoin)
            {
                points[i] = new System.Drawing.Point(item.x, item.y);
                i++;
            }
            drawingBoard.GetActiveBitmapLayer().bitmap = new Bitmap(OldBitmap);
            using (var grafics = Graphics.FromImage(drawingBoard.GetActiveBitmapLayer().bitmap))
            {
                grafics.DrawPolygon(new Pen(Color), points);

            }
        }
    }
}
