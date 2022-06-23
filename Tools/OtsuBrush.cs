using PixelArtProgram.Algorytms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Tools
{
    class OtsuBrush : BitmapBrush
    {
        public OtsuBrush(DrawingBoard _drawingBoard, bool[][] colors) : base(_drawingBoard, null, colors)
        {
            Otsu otsu = new Otsu();
            algorytmBitmap = otsu.OtsuMethod(new Bitmap(drawingBoard.GetActiveBitmapLayer().bitmap));
        }
    }
}
