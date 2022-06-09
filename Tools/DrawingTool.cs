using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PixelArtProgram
{
    public abstract class DrawingTool
    {
        public Color Color;
        protected DrawingBoard drawingBoard;

        public DrawingTool(DrawingBoard _drawingBoard, Color color)
        {
            Color = color;
            drawingBoard = _drawingBoard;
        }

        public abstract void Draw(Point mousePosition);
    
    }
}
