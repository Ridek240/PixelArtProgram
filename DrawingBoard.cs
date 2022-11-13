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
using System.Runtime.InteropServices;

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

            var data = GetActiveBitmapLayer().bitmap.LockBits(
            new Rectangle(System.Drawing.Point.Empty, GetActiveBitmapLayer().bitmap.Size),
            System.Drawing.Imaging.ImageLockMode.ReadWrite,
            System.Drawing.Imaging.PixelFormat.Format32bppRgb
            );

            byte[] arr = new byte[GetActiveBitmapLayer().bitmap.Width * GetActiveBitmapLayer().bitmap.Height * 4];
            Marshal.Copy(data.Scan0, arr,0,arr.Length);
            GetActiveBitmapLayer().bitmap.UnlockBits(data);


            for(int i =0; i<arr.Length;i+=4)
            {
                if (arr[i] == 255 && arr[i] == 255 && arr[i] == 255)
                {
                    outputstring += 0;
                }
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


            var data = GetActiveBitmapLayer().bitmap.LockBits(
            new Rectangle(System.Drawing.Point.Empty, GetActiveBitmapLayer().bitmap.Size),
            System.Drawing.Imaging.ImageLockMode.ReadWrite,
            System.Drawing.Imaging.PixelFormat.Format32bppRgb
            );

            byte[] arr = new byte[GetActiveBitmapLayer().bitmap.Width * GetActiveBitmapLayer().bitmap.Height * 4];
            Marshal.Copy(data.Scan0, arr, 0, arr.Length);
            GetActiveBitmapLayer().bitmap.UnlockBits(data);
            int x = 0;
            for (int i = 0; i < arr.Length; i += 4)
            {
                if (arr[i] == 255 && arr[i] == 255 && arr[i] == 255)
                {
                    bytelist[x] =(byte)0;
                }
                else bytelist[x] = (byte)1;
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

            var data = GetActiveBitmapLayer().bitmap.LockBits(
            new Rectangle(System.Drawing.Point.Empty, GetActiveBitmapLayer().bitmap.Size),
            System.Drawing.Imaging.ImageLockMode.ReadWrite,
            System.Drawing.Imaging.PixelFormat.Format32bppRgb
            );

            byte[] arr = new byte[GetActiveBitmapLayer().bitmap.Width * GetActiveBitmapLayer().bitmap.Height * 4];
            Marshal.Copy(data.Scan0, arr, 0, arr.Length);
            GetActiveBitmapLayer().bitmap.UnlockBits(data);



            for(int ha=0;ha<arr.Length;ha+=4)
            {
                int value = ((int)arr[ha] + (int)arr[ha+1] + (int)arr[ha+2]) / 3;
                outputstring += value / rangesys + " ";
            }


            SaveFile(outputstring);
        }

        public void SaveP5(int range)
        {
            string outputstring = "";
            outputstring += "P5 \n";
            outputstring += GetActiveBitmapLayer().bitmap.Width + " " + GetActiveBitmapLayer().bitmap.Height + "\n";
            int rangesys = 255 / range;
            outputstring += range + "\n";

            var data = GetActiveBitmapLayer().bitmap.LockBits(
            new Rectangle(System.Drawing.Point.Empty, GetActiveBitmapLayer().bitmap.Size),
            System.Drawing.Imaging.ImageLockMode.ReadWrite,
            System.Drawing.Imaging.PixelFormat.Format32bppRgb
            );

            byte[] arr = new byte[GetActiveBitmapLayer().bitmap.Width * GetActiveBitmapLayer().bitmap.Height * 4];
            Marshal.Copy(data.Scan0, arr, 0, arr.Length);

            byte[] bytelist = new byte[GetActiveBitmapLayer().bitmap.Width * GetActiveBitmapLayer().bitmap.Height];
            GetActiveBitmapLayer().bitmap.UnlockBits(data);


            int x = 0;
            for (int ha = 0; ha < arr.Length; ha += 4)
            {
                int value = ((int)arr[ha] + (int)arr[ha + 1] + (int)arr[ha + 2]) / 3;
                bytelist[x] = (byte)(value / rangesys);
                x++;
            }


            byte[] array = Encoding.ASCII.GetBytes(outputstring);


            SaveFileByte(Combine(array, bytelist));
        }

        public void SaveP3(int range)
        {
            string outputstring = "";
            outputstring += "P3 \n";
            outputstring += GetActiveBitmapLayer().bitmap.Width + " " + GetActiveBitmapLayer().bitmap.Height + "\n";
            int rangesys = 255 / range;
            outputstring += range + "\n";


            var data = GetActiveBitmapLayer().bitmap.LockBits(
            new Rectangle(System.Drawing.Point.Empty, GetActiveBitmapLayer().bitmap.Size),
            System.Drawing.Imaging.ImageLockMode.ReadWrite,
            System.Drawing.Imaging.PixelFormat.Format32bppRgb
            );

            byte[] arr = new byte[GetActiveBitmapLayer().bitmap.Width * GetActiveBitmapLayer().bitmap.Height * 4];
            Marshal.Copy(data.Scan0, arr, 0, arr.Length);
            GetActiveBitmapLayer().bitmap.UnlockBits(data);


            for (int ha = 0; ha < arr.Length; ha += 4)
            {
                
                outputstring += (int)arr[ha+2] / rangesys + " " + (int)arr[ha+1] / rangesys + " " + (int)arr[ha] / rangesys + " ";
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
            var data = GetActiveBitmapLayer().bitmap.LockBits(
            new Rectangle(System.Drawing.Point.Empty, GetActiveBitmapLayer().bitmap.Size),
            System.Drawing.Imaging.ImageLockMode.ReadWrite,
            System.Drawing.Imaging.PixelFormat.Format32bppRgb
            );

            byte[] arr = new byte[GetActiveBitmapLayer().bitmap.Width * GetActiveBitmapLayer().bitmap.Height * 4];
            Marshal.Copy(data.Scan0, arr, 0, arr.Length);
            GetActiveBitmapLayer().bitmap.UnlockBits(data);

            for(int ha=0; ha<arr.Length;ha+=4)
                {

                    bytelist[x] = (byte)((int)arr[ha+2] / rangesys);
                    bytelist[x+1] = (byte)((int)arr[ha + 1] / rangesys);
                    bytelist[x+2] = (byte)((int)arr[ha] / rangesys);
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

        #region OldFiles
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
        #endregion

        public void OpenP4(string filename)
        {
            var reader = new BinaryReader(new FileStream(filename, FileMode.Open));

            string width = "", height = "";
            char temp;

            if (reader.ReadChar() != 'P' || reader.ReadChar() != '4')
                return;
            reader.ReadChar();
            RemoveComment(reader, out temp);
            width += temp;
            while ((temp = reader.ReadChar()) != ' ')
                width += temp;

            RemoveComment(reader, out temp);
            height += temp;
            while ((temp = reader.ReadChar()) >= '0' && temp <= '9')
                height += temp;


            int iwidth = int.Parse(width), iheight = int.Parse(height);
            Bitmap tempbitmap = new Bitmap(iwidth, iheight);

            var data = tempbitmap.LockBits(
            new Rectangle(System.Drawing.Point.Empty, tempbitmap.Size),
            System.Drawing.Imaging.ImageLockMode.ReadWrite,
            System.Drawing.Imaging.PixelFormat.Format32bppRgb
            );

            byte[] arr = new byte[tempbitmap.Width * tempbitmap.Height * 4];
            int ind = 0;

            for (int y = 0; y < iheight; y++)
                for (int x = 0; x < iwidth; x++)
                {
                    byte bit = reader.ReadByte();
                    bit = (byte)(bit == 1 ? 0 : 1);
                    arr[ind + 2] = (byte)((int)bit * 255f);
                    arr[ind + 1] = (byte)((int)bit * 255f);
                    arr[ind] = (byte)((int)bit * 255f);
                    arr[ind + 3] = (byte)255;

                    ind += 4;
                }


            Marshal.Copy(arr, 0, data.Scan0, arr.Length);
            tempbitmap.UnlockBits(data);


            AddLayer(new BitmapLayer("intport", tempbitmap), layersBitmap.Count);
        }

        public void OpenP5(string filename)
        {
            var reader = new BinaryReader(new FileStream(filename, FileMode.Open));

            string width = "", height = "", range = "";
            char temp;

            if (reader.ReadChar() != 'P' || reader.ReadChar() != '5')
                return;
            reader.ReadChar();
            RemoveComment(reader, out temp);
            width += temp;
            while ((temp = reader.ReadChar()) != ' ')
                width += temp;

            RemoveComment(reader, out temp);
            height += temp;
            while ((temp = reader.ReadChar()) >= '0' && temp <= '9')
                height += temp;

            RemoveComment(reader, out temp);
            range += temp;
            while ((temp = reader.ReadChar()) >= '0' && temp <= '9')
                range += temp;
            //reader.ReadChar();

            int iwidth = int.Parse(width), iheight = int.Parse(height), irange = int.Parse(range);
            float sysrange = 255f / irange;
            Bitmap tempbitmap = new Bitmap(iwidth, iheight);

            var data = tempbitmap.LockBits(
            new Rectangle(System.Drawing.Point.Empty, tempbitmap.Size),
            System.Drawing.Imaging.ImageLockMode.ReadWrite,
            System.Drawing.Imaging.PixelFormat.Format32bppRgb
            );

            byte[] arr = new byte[tempbitmap.Width * tempbitmap.Height * 4];
            int ind = 0;

            for (int y = 0; y < iheight; y++)
                for (int x = 0; x < iwidth; x++)
                {
                    byte bit = reader.ReadByte();
                    arr[ind + 2] = (byte)((int)bit * sysrange);
                    arr[ind + 1] = (byte)((int)bit * sysrange);
                    arr[ind] = (byte)((int)bit * sysrange);
                    arr[ind + 3] = (byte)255;

                    ind += 4;
                }


            Marshal.Copy(arr, 0, data.Scan0, arr.Length);
            tempbitmap.UnlockBits(data);


            AddLayer(new BitmapLayer("intport", tempbitmap), layersBitmap.Count);

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

            RemoveComment(reader, out temp);
            height += temp;
            while ((temp = reader.ReadChar()) >= '0' && temp <= '9')
                height += temp;

            RemoveComment(reader, out temp);
            range += temp;
            while ((temp = reader.ReadChar()) >= '0' && temp <= '9')
                range += temp;
            //reader.ReadChar();

            int iwidth = int.Parse(width), iheight = int.Parse(height), irange = int.Parse(range);
            int a = 0;
            Bitmap tempbitmap = new Bitmap(iwidth, iheight);

            var data = tempbitmap.LockBits(
            new Rectangle(System.Drawing.Point.Empty, tempbitmap.Size),
            System.Drawing.Imaging.ImageLockMode.ReadWrite,
            System.Drawing.Imaging.PixelFormat.Format32bppRgb
            );

            byte[] arr = new byte[tempbitmap.Width * tempbitmap.Height * 4];
            int ind = 0;

            for (int y = 0; y < iheight; y++)
                for (int x = 0; x < iwidth; x++)
                {
                    arr[ind + 2] = reader.ReadByte();
                    arr[ind + 1] = reader.ReadByte();
                    arr[ind] = reader.ReadByte();
                    arr[ind + 3] = (byte)255;

                    ind += 4;
                }


            Marshal.Copy(arr, 0, data.Scan0, arr.Length);
            tempbitmap.UnlockBits(data);


            AddLayer(new BitmapLayer("intport", tempbitmap), layersBitmap.Count);




        }

        public void NewOpenP1(String filename)
        {

            string[] info = ReadTokens(filename).Take(3).ToArray();
            int width = int.Parse(info[1]), height = int.Parse(info[2]);

            Bitmap tempbitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb); ;

            var data = tempbitmap.LockBits(
            new Rectangle(System.Drawing.Point.Empty, tempbitmap.Size),
            System.Drawing.Imaging.ImageLockMode.ReadWrite,
            System.Drawing.Imaging.PixelFormat.Format32bppRgb
        );
            int x = 0;

            byte[] arr = new byte[tempbitmap.Width * tempbitmap.Height*4];
            
            foreach (var token in ReadTokens(filename).Skip(3))
            {
                foreach (var item in token)
                {


                    if (item.CompareTo('1') == 0)
                    {
                        arr[x] = 0;
                        arr[x + 1] = 0;
                        arr[x + 2] = 0;
                        arr[x + 3] = 255;
                    }
                    else if (item.CompareTo('0') == 0)
                    {
                        arr[x] = 255;
                        arr[x + 1] = 255;
                        arr[x + 2] = 255;
                        arr[x + 3] = 255;
                    }
                    x += 4;
                }
            }


            Marshal.Copy(arr, 0, data.Scan0, arr.Length);
            tempbitmap.UnlockBits(data);
            AddLayer(new BitmapLayer("intport", tempbitmap), layersBitmap.Count);
        }


        public void NewOpenP2(string filename)
        {
            string[] info = ReadTokens(filename).Take(4).ToArray();
            int width = int.Parse(info[1]), height = int.Parse(info[2]);
            int range = int.Parse(info[3]);
            float sysrange = 255f / range;

            Bitmap tempbitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb); ;

            var data = tempbitmap.LockBits(
            new Rectangle(System.Drawing.Point.Empty, tempbitmap.Size),
            System.Drawing.Imaging.ImageLockMode.ReadWrite,
            System.Drawing.Imaging.PixelFormat.Format32bppRgb
        );

            byte[] arr = new byte[tempbitmap.Width * tempbitmap.Height * 4];

            int index = 0;
            foreach(var token in ReadTokens(filename).Skip(4))
            {
                int value = (int)(int.Parse(token) * sysrange);
                arr[index] = (byte)value;
                arr[index + 1] = (byte)value;
                arr[index + 2] = (byte)value;


                index += 4;
            }

            Marshal.Copy(arr, 0, data.Scan0, arr.Length);
            tempbitmap.UnlockBits(data);
            AddLayer(new BitmapLayer("intport", tempbitmap), layersBitmap.Count);
        }



        public void NewOpenP3(string filename)
        {

            string[] info = ReadTokens(filename).Take(4).ToArray();
            int width = int.Parse(info[1]), height = int.Parse(info[2]);
            int range = int.Parse(info[3]);
            float sysrange = 255f / range;


            
            Bitmap tempbitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb); ;

            var data = tempbitmap.LockBits(
            new Rectangle(System.Drawing.Point.Empty, tempbitmap.Size),
            System.Drawing.Imaging.ImageLockMode.ReadWrite,
            System.Drawing.Imaging.PixelFormat.Format32bppRgb
        );

            byte[] arr = new byte[tempbitmap.Width * tempbitmap.Height * 4];

            int index = 0;
           // foreach (var token in ReadTokens(filename).Skip(4).Chunk(3))
           // {
           //     string tempr = token[0];
           //     string tempg = token[1];
           //     string tempb = token[2];
           //     float tempintr = int.Parse(tempr) * sysrange;
           //     float tempintg = int.Parse(tempg) * sysrange;
           //     float tempintb = int.Parse(tempb) * sysrange;
           //
           //
           //     arr[index] = (byte)(int)tempintb;
           //     arr[index + 1] = (byte)(int)tempintg;
           //     arr[index + 2] = (byte)(int)tempintr;
           //     index += 4;
           // }



            /* //.net5
            string[] texting = ReadTokens(filename).Skip(4).ToArray();

            int x = 0;
            int index = 0;
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    string tempr = texting[x];
                    string tempg = texting[x + 1];
                    string tempb = texting[x + 2];
                    float tempintr = int.Parse(tempr) * sysrange;
                    float tempintg = int.Parse(tempg) * sysrange;
                    float tempintb = int.Parse(tempb) * sysrange;


                    arr[index] = (byte)(int)tempintb;
                    arr[index+1] = (byte)(int)tempintg;
                    arr[index+2] = (byte)(int)tempintr;

                    x += 3;
                    index += 4;
                }
            */
            Marshal.Copy(arr, 0, data.Scan0, arr.Length);
            tempbitmap.UnlockBits(data);


            AddLayer(new BitmapLayer("intport", tempbitmap), layersBitmap.Count);
        }
        private IEnumerable<String> ReadTokens(string filename)
        {
            using var reader = new StreamReader(filename);
            
                while(!reader.EndOfStream)
            {
                string line = reader.ReadLine()!;
                int comment = line.IndexOf('#');
                if (comment != -1)
                    line = line[..comment];
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                line = Regex.Replace(line, @"\s+", " ");

                string[] values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < values.Length; i++)
                {
                    yield return values[i];
                }
            }

        }


            public BinaryReader RemoveComment(BinaryReader reader, out char temp)
        {
            temp = reader.ReadChar();
            if (temp  == '#')
            {
                temp = ' ';
                while (reader.ReadByte() != '\n') ;
            }
            else if(temp== ' ')
            {
                    while ((temp = reader.ReadChar()) == ' ') ;
            }
            return reader;
        }
        public void OpenNetpbm()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {

                string[] texting = ReadTokens(openFileDialog.FileName).Take(2).ToArray(); ;


                if(texting[0].CompareTo("P1")==0)
                {
                    NewOpenP1(openFileDialog.FileName);
                }
                if(texting[0].CompareTo("P2")==0)
                {
                    NewOpenP2(openFileDialog.FileName);
                }
                if (texting[0].CompareTo("P3") == 0)
                {
                    NewOpenP3(openFileDialog.FileName);
                }
                if (texting[0].CompareTo("P4") == 0)
                {
                    OpenP4(openFileDialog.FileName);
                }
                if (texting[0].CompareTo("P5")==0)
                {
                    OpenP5(openFileDialog.FileName);
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
