namespace PixelArtProgram
{
    public class Eraser : DrawingTool
    {


        public Eraser() : base(System.Drawing.Color.FromArgb(0, 0, 0, 0)) { }
        
        public override void Draw(DrawingBoard drawingBoard, Point mousePosition)
        {
            drawingBoard.GetActiveBitmapLayers().bitmap.SetPixel(mousePosition.x, mousePosition.y, Color);
        }
    }
}
