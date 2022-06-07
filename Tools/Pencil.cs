namespace PixelArtProgram
{
    public class Pencil : DrawingTool
    {

        public Pencil(System.Drawing.Color color) : base(color) { }

        public override void Draw(DrawingBoard drawingBoard, Point mousePosition)
        {
            drawingBoard.GetActiveBitmapLayer().bitmap.SetPixel(mousePosition.x, mousePosition.y, Color);
        }
    }
}
