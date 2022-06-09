using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PixelArtProgram.Tools
{
    public class DrawObject : DrawingTool
    {
        protected Point startingPoint;

        public DrawObject(System.Drawing.Color color,Point StartingPoint) : base(color)
        {
            startingPoint = StartingPoint;
        }
        

        public override void Draw(DrawingBoard drawingBoard, Point mousePosition)
        {

        }
        public void GetStartingPoint(Point point)
        {
            startingPoint = point;
        }

    }
}
