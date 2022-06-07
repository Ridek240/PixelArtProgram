using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PixelArtProgram
{
    public class DrawingBoard
    {
        public int Width;
        public int Height;
        private int activeLayer = -1;
        public int ActiveLayer { get => activeLayer; set => activeLayer = value; }
        public List<BitmapLayer> layersBitmap = new List<BitmapLayer>();
        
        public DrawingBoard() { }
        public DrawingBoard(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public System.Drawing.Color GetPixel(Point point)
        {
            return layersBitmap[activeLayer].bitmap.GetPixel(point.x, point.y);
        }

        public void RemoveLayer(int id)
        {
            layersBitmap.RemoveAt(id);
            activeLayer -= activeLayer >= layersBitmap.Count ? 1 : 0;
        }

        public void AddLayer(string name)
        {
            BitmapLayer bitmapLayer = new BitmapLayer(name, new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb));
            layersBitmap.Add(bitmapLayer);
            activeLayer = layersBitmap.Count - 1;
        }

        // Drawing
        private DrawingTool currentDrawingTool;


        public bool CanDraw()
        {
            if (activeLayer < 0) return false;
            if (activeLayer > layersBitmap.Count) return false;
            if (layersBitmap.Count < 0) return false;

            return true;
        }

        public void Draw(Point mousePosition)
        {
            if (!CanDraw()) return;
            currentDrawingTool.Draw(this, mousePosition);
        }

        public void StartDrawing(Point mousePosition, DrawingTool newDrawingTool)
        {
            currentDrawingTool = newDrawingTool;
            Draw(mousePosition);
        }

        public BitmapLayer GetActiveBitmapLayer()
        {
            return layersBitmap[activeLayer];
        }
    }
}
