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

    }
}
