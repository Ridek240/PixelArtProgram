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
        public DrawRect(System.Drawing.Color color,Point StartingPoint) : base(color,StartingPoint) { }


        public override void Draw(DrawingBoard drawingBoard, Point mousePosition)
        {
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
