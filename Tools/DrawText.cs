using System.Drawing;

namespace PixelArtProgram.Tools
{
    public class DrawText : DrawObject
    {
        string Messege;
        public DrawText(DrawingBoard _drawingBoard, Color color, Point StartingPoint,string Message) : base(_drawingBoard, color, StartingPoint) {
            this.Messege = Message;
        }

        public override void Draw(Point mousePosition)
        {
            drawingBoard.GetActiveBitmapLayer().bitmap = new Bitmap(OldBitmap);
            using (var grafics = Graphics.FromImage(drawingBoard.GetActiveBitmapLayer().bitmap))
            {
               
                grafics.DrawString(Messege,new Font(System.Drawing.FontFamily.GenericSansSerif,6), new SolidBrush(Color),new System.Drawing.Point(startingPoint.x,startingPoint.y));
            }
        }
    }
}
