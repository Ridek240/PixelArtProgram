namespace PixelArtProgram
{
    public class Eraser : DrawingTool
    {
        public static System.Drawing.Color Color = System.Drawing.Color.FromArgb(0, 0, 0, 0);
        public void Draw(DrawingBoard drawingBoard, Point mousePosition)
        {
            drawingBoard.GetActiveBitmapLayer().bitmap.SetPixel(mousePosition.x, mousePosition.y, Color);
        }
    }
}
