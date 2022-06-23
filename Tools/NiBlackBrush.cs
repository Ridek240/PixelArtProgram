using PixelArtProgram.Algorytms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Tools
{
    class NiBlackBrush : BitmapBrush
    {
        public NiBlackBrush(DrawingBoard _drawingBoard, bool[][] colors) : base(_drawingBoard, null, colors)
        {
            NiBlack niBlack = new NiBlack();
            algorytmBitmap = niBlack.Function(new Bitmap(drawingBoard.GetActiveBitmapLayer().bitmap));
        }
    }
}
