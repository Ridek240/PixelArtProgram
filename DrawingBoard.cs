using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Drawing.Imaging;
using System.Linq;
using PixelArtProgram.Tools;

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
            openFileDialog.Filter = "Dead Files (*.png)|*.png|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                Bitmap TempBitmap = new Bitmap(openFileDialog.FileName);
                AddLayer(openFileDialog.SafeFileName.Replace(".png",""));
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
        public BitmapLayer(string name, Bitmap bitmap)
        {
            this.name = name;
            this.bitmap = bitmap;
        }
    }
}
