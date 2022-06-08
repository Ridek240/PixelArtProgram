using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Tools
{
    class DrawObject : DrawingTool
    {
        Point startingPoint;

        public DrawObject(System.Drawing.Color color) : base(color) { }

        public override void Draw(DrawingBoard drawingBoard, Point mousePosition)
        {

        }
        public void GetStartingPoint(Point point)
        {
            startingPoint = point;
        }

    }
}
