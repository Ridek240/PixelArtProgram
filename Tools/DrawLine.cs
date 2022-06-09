using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PixelArtProgram.Tools
{
    public class DrawLine : DrawObject
    {
        public DrawLine(DrawingBoard _drawingBoard, Color color, Point StartingPoint) : base(_drawingBoard, color, StartingPoint) { }

        public override void Draw(Point mousePosition)
        {
            drawingBoard.GetActiveBitmapLayer().bitmap = new Bitmap(OldBitmap);
            using (var grafics = Graphics.FromImage(drawingBoard.GetActiveBitmapLayer().bitmap))
            {
                grafics.DrawLine(
                    new Pen(Color),
                    new PointF(startingPoint.x, startingPoint.y),
                    new PointF(mousePosition.x, mousePosition.y));
            }
        }
    }
}
