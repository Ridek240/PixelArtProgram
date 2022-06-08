using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Drawing.Imaging;
using System.Linq;

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


        public void SaveFile()
        {
            if (layersBitmap.Count() > 0)
            {
                if (MessageBox.Show("Czy na pewno chcesz Zapisać?", "Usuń Element", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Nie nadpisuj utwórz nowy \n (*.zyd)|*.zyd|All files (*.*)|*.*";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        try
                        {
                            {
                                if (!Directory.Exists(saveFileDialog.FileName))
                                {
                                    Directory.CreateDirectory(saveFileDialog.FileName);
                                }
                                int i = 0;
                                foreach (BitmapLayer bitmap in layersBitmap)
                                {
                                    bitmap.bitmap.Save(saveFileDialog.FileName + "/" + bitmap.name + "." + i.ToString(), ImageFormat.Png);
                                    i++;
                                }
                            }
                        }
                        catch { _ = MessageBox.Show("Błąd w zapisywaniu pliku"); }
                    }
                }
            }
            else
                MessageBox.Show("Plik nie istnieje");

        }

        public void ExtractLayer()
        {
            SaveImage(GetActiveBitmapLayer().bitmap);
        }

        public void ExtractAll()
        {



            Bitmap target = new Bitmap(Width,Height);

            using (var grafics = Graphics.FromImage(target))
            {
                foreach (BitmapLayer layer in layersBitmap)
                {
                    grafics.DrawImage(layer.bitmap, new Rectangle(0, 0, Width, Height));
                }
            }

            SaveImage(target);


        }

        private void SaveImage(Bitmap bitmap)
        {



            Sized sized = new Sized();
            sized.Title = "Eksport";
            if (sized.ShowDialog() == true)
            {

                int size;

                if(int.TryParse(sized.Input.Text,out size)==false)
                {
                    MessageBox.Show("Błędna wielkość");
                    return;
                }




                Bitmap Sized = new Bitmap(Width * size, Height * size);


                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        for (int i1 = 0; i1 < size; i1++)
                        {
                            for (int j1 = 0; j1 < size; j1++)
                            {
                                Sized.SetPixel(i * size + i1, j * size + j1, bitmap.GetPixel(i, j));
                            }
                        }
                    }
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Nie nadpisuj utwórz nowy \n (*.png)|*.png|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        Sized.Save(saveFileDialog.FileName, ImageFormat.Png);
                    }
                    catch { _ = MessageBox.Show("Błąd w zapisywaniu pliku"); }
                }
                else
                    MessageBox.Show("Plik nie istnieje");
            }
        }
    }


    public class BitmapLayer
    {
        public string name;
        public Bitmap bitmap;
        public BitmapLayer(string name, Bitmap bitmap)
        {
            this.name = name;
            this.bitmap = bitmap;
        }
    }
}
