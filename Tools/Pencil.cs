namespace PixelArtProgram
{
    public class Pencil : DrawingTool
    {
        public System.Drawing.Color Color;
        public Pencil(System.Drawing.Color color)
        {
            Color = color;
        }
        public void Draw(DrawingBoard drawingBoard, Point mousePosition)
        {
            drawingBoard.GetActiveBitmapLayer().bitmap.SetPixel(mousePosition.x, mousePosition.y, Color);
        }
    }
}
