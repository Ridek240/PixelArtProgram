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
    
        public DrawingTool(Color color)
        {
            Color = color;
        }

        public abstract void Draw(DrawingBoard drawingBoard, Point mousePosition);
    
    }
}
