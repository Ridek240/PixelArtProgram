namespace PixelArtProgram
{
    public class Eraser : DrawingTool
    {


        public Eraser(DrawingBoard _drawingBoard) : base(_drawingBoard, System.Drawing.Color.FromArgb(0, 0, 0, 0)) { }
        
        public override void Draw(Point mousePosition)
        {
            drawingBoard.GetActiveBitmapLayer().bitmap.SetPixel(mousePosition.x, mousePosition.y, Color);
        }
    }
}
