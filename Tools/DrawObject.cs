using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PixelArtProgram.Tools
{
    public abstract class DrawObject : DrawingTool
    {
        protected Point startingPoint;
        protected Bitmap OldBitmap;

        public DrawObject(DrawingBoard _drawingBoard, Color color, Point StartingPoint) : base(_drawingBoard, color)
        {
            startingPoint = StartingPoint;
            OldBitmap = new Bitmap(drawingBoard.GetActiveBitmapLayer().bitmap);
        }
        //public void GetStartingPoint(Point point)
        //{
        //    startingPoint = point;
        //}
    }
}
