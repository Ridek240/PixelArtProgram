using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Drawing.Imaging;
using System.Linq;
using System.Drawing.Drawing2D;
using PixelArtProgram.Tools;
using System.Text.RegularExpressions;
using System.Text;
using System;

namespace PixelArtProgram
{
    public class DrawingBoard
    {
        
        public int Width;
        public int Height;
        private int activeLayer = -1;
        public int ActiveLayer { get => activeLayer; set => activeLayer = value; }
        public List<BitmapLayer> layersBitmap = new List<BitmapLayer>();

        public Stack<Action> OldActions = new Stack<Action>();
        public Stack<Action> NewActions = new Stack<Action>();

        public DrawingBoard() { }
        public DrawingBoard(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void Paste(Bitmap bitmap)
        {
            Bitmap bitmapOld = new Bitmap(GetActiveBitmapLayer().bitmap);
            GetActiveBitmapLayer().bitmap = CombineBitmaps(GetActiveBitmapLayer().bitmap, bitmap);
            CreateAction(new DrawAction
            {
                DB = this,
                layerIndex = activeLayer,
                BitmapOld = bitmapOld,
                BitmapNew = new Bitmap(GetActiveBitmapLayer().bitmap)
            });
        }

        public void Replace(Bitmap bitmap)
        {
            Bitmap bitmapOld = new Bitmap(GetActiveBitmapLayer().bitmap);
            GetActiveBitmapLayer().bitmap = bitmap;
            CreateAction(new DrawAction
            {
                DB = this,
                layerIndex = activeLayer,
                BitmapOld = bitmapOld,
                BitmapNew = new Bitmap(GetActiveBitmapLayer().bitmap)
            });
        }

        public Bitmap MergeLayers()
        {
            if (layersBitmap.Count <= 0) return null;
            Bitmap result = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            foreach (BitmapLayer bitmapLayer in layersBitmap)
            {
                result = CombineBitmaps(result, bitmapLayer.bitmap);
            }

            return result;
        }

        public Bitmap CombineBitmaps(Bitmap largeBmp, Bitmap smallBmp)
        {
            Graphics g = Graphics.FromImage(largeBmp);
            g.CompositingMode = CompositingMode.SourceOver;
            //smallBmp.MakeTransparent();
            g.DrawImage(smallBmp, new System.Drawing.Point(0, 0));
            return largeBmp;
        }

        public static void CopyRegionIntoImage(Bitmap srcBitmap, Rectangle srcRegion, ref Bitmap destBitmap, Rectangle destRegion)
        {
            using (Graphics grD = Graphics.FromImage(destBitmap))
            {
                grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
            }
        }

        public Color GetPixel(Point point)
        {
            return layersBitmap[activeLayer].bitmap.GetPixel(point.x, point.y);
        }

        public void RemoveLayer(int id, bool recorded = true)
        {
            if (id >= layersBitmap.Count) return;
            if (recorded)
            {
                CreateAction(new RemoveLayerAction
                {
                    DB = this,
                    layerIndex = id,
                    BitmapLayer = layersBitmap[id]
                });
            }

            layersBitmap.RemoveAt(id);
            activeLayer -= activeLayer >= layersBitmap.Count ? 1 : 0;
        }

        public void AddLayer(string name, bool recorded = true) => AddLayer(name, layersBitmap.Count, recorded);

        public void AddLayer(string name, int layerIndex, bool recorded = true)
        {
            BitmapLayer bitmapLayer = new BitmapLayer(name, new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb));
            AddLayer(bitmapLayer, layerIndex, recorded);
        }

        public void AddLayer(BitmapLayer bitmapLayer, int layerIndex, bool recorded = true)
        {
            if (recorded)
            {
                CreateAction(new AddLayerAction
                {
                    DB = this,
                    layerIndex = layerIndex,
                    BitmapLayer = bitmapLayer
                });
            }

            layersBitmap.Insert(layerIndex, bitmapLayer);
            activeLayer = layersBitmap.Count - 1;
        }

        // Drawing
        private DrawingTool currentDrawingTool;
        private Action currentAction;
        private Bitmap oldBitmap;

        public bool CanDraw(Point point)
        {
            if (!CanDraw()) return false;
            if (point.x >= Width || point.y >= Height) return false;

            return true;
        }

        public bool CanDraw()
        {
            if (activeLayer < 0) return false;
            if (activeLayer > layersBitmap.Count) return false;
            if (layersBitmap.Count < 0) return false;

            return true;
        }

        public void Draw(Point mousePosition)
        {
            if (!CanDraw(mousePosition)) return;
            currentDrawingTool.Draw(mousePosition);
        }

        public void StartDrawing(Point mousePosition, DrawingTool newDrawingTool)
        {
            currentDrawingTool = newDrawingTool;
            oldBitmap = new Bitmap(GetActiveBitmapLayer().bitmap);
            Draw(mousePosition);
        }

        public List<BitmapLayer> GetBitmapLayers()
        {
            return layersBitmap;
        }

        public void StopDrawing(Point mousePosition)
        {
            currentAction = new DrawAction
            {
                DB = this,
                BitmapNew = new Bitmap(GetActiveBitmapLayer().bitmap),
                BitmapOld = oldBitmap,
                layerIndex = activeLayer
            };
            CreateAction(currentAction);
        }



        public void CreateAction(Action action)
        {
            OldActions.Push(action);
            NewActions = new Stack<Action>();
        }

        public void Undo()
        {
            if (OldActions.Count <= 0) return;
            Action action = OldActions.Pop();
            action.Undo();
            NewActions.Push(action);
        }

        public void Redo()
        {
            if (NewActions.Count <= 0) return;
            Action action = NewActions.Pop();
            action.Redo();
            OldActions.Push(action);
        }

        public BitmapLayer GetActiveBitmapLayer()
        {
            return layersBitmap[activeLayer];
        }
        public BitmapLayer GetBitmapLayer(int layerIndex)
        {
            return layersBitmap[layerIndex];
        }

        public bool LayerUp(int Layers, bool recorded = true)
        {
            if (Layers >= 1)
            {
                if (recorded)
                {
                    CreateAction(new MoveLayerAction
                    {
                        DB = this,
                        LayerIndex = Layers,
                        MovedUp = true
                    });
                }
                BitmapLayer layer = layersBitmap[Layers];
                layersBitmap.Remove(layer);
                layersBitmap.Insert(Layers - 1, layer);
                activeLayer = Layers - 1;
                return true;
            }
            return false;
        }

        public bool LayerDown(int Layers, bool recorded = true)
        {
            if (Layers < layersBitmap.Count - 1)
            {
                if (recorded)
                {
                    CreateAction(new MoveLayerAction
                    {
                        DB = this,
                        LayerIndex = Layers,
                        MovedUp = false
                    });
                }
                BitmapLayer layer = layersBitmap[Layers];
                layersBitmap.Remove(layer);
                layersBitmap.Insert(Layers + 1, layer);
                activeLayer = Layers + 1;
                return true;
            }
            return false;
        }

        public void LoadLayer()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Dead Files (*.png)|*.png|Dead Files (*.bmp)|*.bmp|Dead Files (*.jpg)|*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                Bitmap TempBitmap = new Bitmap(openFileDialog.FileName);
                AddLayer(openFileDialog.SafeFileName.Replace(".png", ""));
                using (var grafics = Graphics.FromImage(GetActiveBitmapLayer().bitmap))
                {
                    grafics.DrawImage(TempBitmap, new Rectangle(0, 0, Width, Height));
                }
            }
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

        public void SaveP1()
        {
            string outputstring = "";
            outputstring += "P1 \n";
            outputstring += GetActiveBitmapLayer().bitmap.Width + " " + GetActiveBitmapLayer().bitmap.Height + "\n";

            for (int j = 0; j < GetActiveBitmapLayer().bitmap.Height; j++)
                for (int i = 0; i < GetActiveBitmapLayer().bitmap.Width; i++)
                {

                    Color pixel = GetActiveBitmapLayer().bitmap.GetPixel(i, j);
                    if (pixel == Color.FromArgb(255, 255, 255))
                    { outputstring += 0; }
                    else outputstring += 1;
                }

            SaveFile(outputstring);
        }

        public void SaveP4()
        {

            string outputstring = "";
            outputstring += "P4 \n";
            outputstring += GetActiveBitmapLayer().bitmap.Width + " " + GetActiveBitmapLayer().bitmap.Height + "\n";
            byte[] bytelist = new byte[GetActiveBitmapLayer().bitmap.Width * GetActiveBitmapLayer().bitmap.Height];

            int x = 0;
            for (int j = 0; j < GetActiveBitmapLayer().bitmap.Height; j++)
                for (int i = 0; i < GetActiveBitmapLayer().bitmap.Width; i++)
                {

                    Color pixel = GetActiveBitmapLayer().bitmap.GetPixel(i, j);
                    if (pixel == Color.FromArgb(255, 255, 255))
                    { bytelist[x] = 0; }
                    else bytelist[x] = 1;
                    x++;
                }
            byte[] array = Encoding.ASCII.GetBytes(outputstring);


            SaveFileByte(Combine(array,bytelist));
        }
        public static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] bytes = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
            return bytes;
        }
        public void SaveP2(int range)
        {
            string outputstring = "";
            outputstring += "P2 \n";
            outputstring += GetActiveBitmapLayer().bitmap.Width + " " + GetActiveBitmapLayer().bitmap.Height + "\n";
            int rangesys = 255 / range;
            outputstring += range + "\n";

            for (int j = 0; j < GetActiveBitmapLayer().bitmap.Height; j++)
                for (int i = 0; i < GetActiveBitmapLayer().bitmap.Width; i++)
                {

                    Color pixel = GetActiveBitmapLayer().bitmap.GetPixel(i, j);
                    int value = (pixel.R + pixel.G + pixel.B) / 3;

                    outputstring += value/rangesys + " ";
                }

            SaveFile(outputstring);
        }

        public void SaveP3(int range)
        {
            string outputstring = "";
            outputstring += "P3 \n";
            outputstring += GetActiveBitmapLayer().bitmap.Width + " " + GetActiveBitmapLayer().bitmap.Height + "\n";
            int rangesys = 255 / range;
            outputstring += range + "\n";

            for (int j = 0; j < GetActiveBitmapLayer().bitmap.Height; j++)
                for (int i = 0; i < GetActiveBitmapLayer().bitmap.Width; i++)
                {

                    Color pixel = GetActiveBitmapLayer().bitmap.GetPixel(i, j);
                    outputstring = pixel.R/rangesys + " " + pixel.G/rangesys + " " + pixel.B/rangesys + " ";

                    
                }

            SaveFile(outputstring);
        }

        public void SaveP6(int range)
        {
            string outputstring = "";
            outputstring += "P6 \n";
            outputstring += GetActiveBitmapLayer().bitmap.Width + " " + GetActiveBitmapLayer().bitmap.Height + "\n";
            int rangesys = 255 / range;


            byte[] bytelist = new byte[GetActiveBitmapLayer().bitmap.Width * GetActiveBitmapLayer().bitmap.Height*3];
            outputstring += range + "\n";
            int x = 0;
            for (int j = 0; j < GetActiveBitmapLayer().bitmap.Height; j++)
                for (int i = 0; i < GetActiveBitmapLayer().bitmap.Width; i++)
                {

                    Color pixel = GetActiveBitmapLayer().bitmap.GetPixel(i, j);
                    //outputstring = pixel.R / rangesys + " " + pixel.G / rangesys + " " + pixel.B / rangesys + " ";
                    bytelist[x] = (byte)(pixel.R / rangesys);
                    bytelist[x+1] = (byte)(pixel.G / rangesys);
                    bytelist[x+2] = (byte)(pixel.B / rangesys);
                    x += 3;
                }

            byte[] array = Encoding.ASCII.GetBytes(outputstring);
            SaveFileByte(Combine(array, bytelist));
        }

        public void SaveFile(string message)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Nie nadpisuj utwórz nowy \n (*.ppm)|*.pbm|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    
                    File.WriteAllText(saveFileDialog.FileName, message);
                }
                catch { _ = MessageBox.Show("Błąd w zapisywaniu pliku"); }
            }
            else
                MessageBox.Show("Plik nie istnieje");
        }
        public void SaveFileByte(byte[] message)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Nie nadpisuj utwórz nowy \n (*.ppm)|*.pbm|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {

                    File.WriteAllBytes(saveFileDialog.FileName, message);
                }
                catch { _ = MessageBox.Show("Błąd w zapisywaniu pliku"); }
            }
            else
                MessageBox.Show("Plik nie istnieje");
        }


        public void OpenP1(string[] texting)
        {

            int width = int.Parse(texting[1]), height = int.Parse(texting[2]);

            Bitmap tempbitmap = new Bitmap(width, height);

            int x = 0;

            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    char temp = texting[3][x];
                    if(temp.CompareTo('1')==0)
                    {
                        tempbitmap.SetPixel(i, j, Color.FromArgb(255, 0, 0, 0));
                    }
                    else if(temp.CompareTo('0') == 0)
                    {
                        tempbitmap.SetPixel(i, j, Color.FromArgb(255, 255, 255, 255));
                    }
                    x++;
                }

            AddLayer(new BitmapLayer("intport",tempbitmap), layersBitmap.Count);
        }

        public void OpenP2(string[] texting)
        {

            int width = int.Parse(texting[1]), height = int.Parse(texting[2]);
            int range = int.Parse(texting[3]);
            int sysrange = 255 / range;
            


            int x = 4;
            Bitmap tempbitmap = new Bitmap(width, height);

            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    string temp = texting[x];
                    int tempint = int.Parse(temp) * sysrange;
                    tempbitmap.SetPixel(i, j, Color.FromArgb(255, tempint, tempint, tempint));

                    x++;
                }
            AddLayer(new BitmapLayer("intport", tempbitmap), layersBitmap.Count);

        }
        public void OpenP3(string[] texting)
        {

            int width = int.Parse(texting[1]), height = int.Parse(texting[2]);
            int range = int.Parse(texting[3]);
            float sysrange = 255f / range;


            int x = 4;
            Bitmap tempbitmap = new Bitmap(width, height);

            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    string tempr = texting[x];
                    string tempg = texting[x+1];
                    string tempb = texting[x+2];
                    float tempintr = int.Parse(tempr) * sysrange;
                    float tempintg = int.Parse(tempg) * sysrange;
                    float tempintb = int.Parse(tempb) * sysrange;
                    tempbitmap.SetPixel(i, j, Color.FromArgb(255, (int)tempintr, (int)tempintg, (int)tempintb));

                    x+=3;
                }
            AddLayer(new BitmapLayer("intport", tempbitmap), layersBitmap.Count);
        }



        public void openp6test(string filename)
        {
            using var reader = new StreamReader(filename);
            string? header = null;
            int? width = null, height = null;
            //while(!reader.EndOfStream)
            string[] line = reader.ReadLine()?.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (header == null)
                header = line[0];
            else if (width == null)
            {
                width = int.Parse(line[0]);
            }
            //else if ()


              //  byte[] arr = new byte[width * height * 3];
        }

        public void OpenP6(string filename)
        {
            //byte[] byte_value = File.ReadAllBytes(filename);
            var reader = new BinaryReader(new FileStream(filename, FileMode.Open));
            
            string width = "", height = "",range = "";
            char temp;

            if (reader.ReadChar() != 'P' || reader.ReadChar() != '6')
                return;
            reader.ReadChar();
            RemoveComment(reader, out temp);
            width += temp;
            while ((temp = reader.ReadChar()) != ' ')
                width += temp;
            while ((temp = reader.ReadChar()) >= '0' && temp <= '9')
                height += temp;
           // if (reader.ReadChar() != '2' || reader.ReadChar() != '5' || reader.ReadChar() != '5')
           //     return;
            while ((temp = reader.ReadChar()) >= '0' && temp <= '9')
                range += temp;
            //reader.ReadChar();

            int iwidth = int.Parse(width), iheight = int.Parse(height), irange = int.Parse(range);
            int a = 0;
            Bitmap tempbitmap = new Bitmap(iwidth, iheight);

            for (int y = 0; y < iheight; y++)
                for (int x = 0; x < iwidth; x++)
                {

                    tempbitmap.SetPixel(x, y, Color.FromArgb(reader.ReadByte(), reader.ReadByte(), reader.ReadByte()));
                }


            AddLayer(new BitmapLayer("intport", tempbitmap), layersBitmap.Count);




        }
        public BinaryReader RemoveComment(BinaryReader reader, out char temp)
        {
            if ((temp = reader.ReadChar()) == '#')
            {
                temp = ' ';
                while (reader.ReadChar() != '\n') ;
            }

            return reader;
        }
        public void OpenNetpbm()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string input = File.ReadAllText(openFileDialog.FileName);

                string pattern = "#" + "(.*?)" + "\n";
               Regex regex = new Regex(pattern, RegexOptions.RightToLeft);
               
               foreach (Match match in regex.Matches(input))
               {
                   input = input.Replace(match.Groups[1].Value, string.Empty);
               }

                input = input.Replace("#", "");
                input = Regex.Replace(input, @"\s+", " ");
                string[] texting = input.Split(" ");

                


                if(texting[0].CompareTo("P1")==0)
                {
                    OpenP1(texting);
                }
                if(texting[0].CompareTo("P2")==0)
                {
                    OpenP2(texting);
                }
                if (texting[0].CompareTo("P3") == 0)
                {
                    OpenP3(texting);
                }
                if(texting[0].CompareTo("P6")==0)
                {
                    OpenP6(openFileDialog.FileName);
                }


            }
        }

    

        public void ExtractAll()
        {
            Bitmap target = new Bitmap(Width, Height);

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

                if (int.TryParse(sized.Input.Text, out size) == false)
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
                saveFileDialog.Filter = "Nie nadpisuj utwórz nowy \n (*.png)|*.png|Nie nadpisuj utwórz nowy \n (*.bmp)|*.bmp|Nie nadpisuj utwórz nowy \n (*.jpg)|*.jpg|All files (*.*)|*.*";
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

    public abstract class Action
    {
        public DrawingBoard DB;

        public abstract void Undo();

        public abstract void Redo();
    }

    public class RemoveLayerAction : Action
    {
        public BitmapLayer BitmapLayer;
        public int layerIndex;
        public override void Redo()
        {
            DB.RemoveLayer(layerIndex, false);
        }

        public override void Undo()
        {
            DB.AddLayer(BitmapLayer, layerIndex, false);
        }
    }

    public class AddLayerAction : Action
    {
        public BitmapLayer BitmapLayer;
        public int layerIndex;
        public override void Redo()
        {
            DB.AddLayer(BitmapLayer, layerIndex, false);
        }

        public override void Undo()
        {
            DB.RemoveLayer(layerIndex, false);
        }
    }

    public class DrawAction : Action
    {
        public Bitmap BitmapOld;
        public Bitmap BitmapNew;
        public int layerIndex;

        public override void Undo()
        {
            DB.GetBitmapLayer(layerIndex).bitmap = BitmapOld;
        }

        public override void Redo()
        {
            DB.GetBitmapLayer(layerIndex).bitmap = BitmapNew;
        }
    }

    public class MoveLayerAction : Action
    {
        public int LayerIndex;
        public bool MovedUp;
        public override void Redo()
        {
            if (MovedUp)
                DB.LayerUp(LayerIndex, false);
            else
                DB.LayerDown(LayerIndex, false);
        }

        public override void Undo()
        {
            if (MovedUp)
                DB.LayerDown(LayerIndex - 1, false);
            else
                DB.LayerUp(LayerIndex + 1, false);
        }
    }

    public class BitmapLayer
    {
        public string name;
        public Bitmap bitmap;
        public bool IsVisible;
        public BitmapLayer(string name, Bitmap bitmap)
        {
            this.name = name;
            this.bitmap = bitmap;
            IsVisible = true;
        }
    }
}
