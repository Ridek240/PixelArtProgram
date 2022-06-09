namespace PixelArtProgram
{
    public class Pencil : DrawingTool
    {
        public Pencil(DrawingBoard _drawingBoard, System.Drawing.Color color) : base(_drawingBoard, color) { }

        public override void Draw(Point mousePosition)
        {
            drawingBoard.GetActiveBitmapLayer().bitmap.SetPixel(mousePosition.x, mousePosition.y, Color);
            
        }
    }
}
