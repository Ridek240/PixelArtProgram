using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram
{
    public interface DrawingTool
    {
        public void Draw(DrawingBoard drawingBoard, Point mousePosition);
    }
}
