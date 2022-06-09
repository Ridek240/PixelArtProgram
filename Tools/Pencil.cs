namespace PixelArtProgram
{
    public class Pencil : DrawingTool
    {

        public Pencil(System.Drawing.Color color) : base(color) { }

        public override void Draw(DrawingBoard drawingBoard, Point mousePosition)
        {
            drawingBoard.GetActiveBitmapLayers().bitmap.SetPixel(mousePosition.x, mousePosition.y, Color);
            
        }
    }
}
