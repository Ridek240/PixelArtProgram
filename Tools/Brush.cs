﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Tools
{
    public class Brush : DrawingTool
    {
        public bool[][] Colors;
        public Brush(DrawingBoard _drawingBoard, Color color, bool[][] colors) : base(_drawingBoard, color)
        {
            Colors = colors;
        }

        public override void Draw(Point mousePosition)
        {
            if (Colors.Length <= 0) return;
            int lengthX = Colors.Length;
            int lengthY = Colors[0].Length;
            int sizeX = drawingBoard.GetActiveBitmapLayer().bitmap.Width;
            int sizeY = drawingBoard.GetActiveBitmapLayer().bitmap.Height;
            for (int x = 0; x < Colors.Length; x++)
            {
                for (int y = 0; y < Colors[x].Length; y++)
                {
                    int locX = mousePosition.x - lengthX / 2 + x;
                    int locY = mousePosition.y - lengthY / 2 + y;
                    if (locX < 0 ||
                        locX >= sizeX ||
                        locY < 0 ||
                        locY >= sizeY) continue;
                    if (!Colors[x][y]) continue;
                    drawingBoard.GetActiveBitmapLayer().bitmap.SetPixel(
                        locX, 
                        locY, 
                        Color);

                }
            }
        }
    }
}
