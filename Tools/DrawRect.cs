using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PixelArtProgram.Tools
{
    public class DrawRect : DrawObject
    {
        public DrawRect(DrawingBoard _drawingBoard, Color color, Point StartingPoint) : base(_drawingBoard, color, StartingPoint) { }

        public override void Draw(Point mousePosition)
        {
            drawingBoard.GetActiveBitmapLayer().bitmap = new Bitmap(OldBitmap);
            using (var grafics = Graphics.FromImage(drawingBoard.GetActiveBitmapLayer().bitmap))
            {
                Rectangle child = new Rectangle();

                int width = mousePosition.x - startingPoint.x;
                int height = mousePosition.y - startingPoint.y;

                child.Width = width > 0 ? width : width * -1;
                child.Height = height > 0 ? height : height * -1;

                int xPosition = width > 0 ? startingPoint.x : mousePosition.x;
                int yPosition = height > 0 ? startingPoint.y : mousePosition.y;
                child.X = xPosition;
                child.Y = yPosition;
                grafics.DrawRectangle(new Pen(Color), child);
            }
        }
    }
}
