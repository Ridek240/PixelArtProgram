﻿using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using Controll = System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PixelArtProgram.Grafic;
using PixelArtProgram.Tools;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using PixelArtProgram.Algorytms;

namespace PixelArtProgram
{

    public partial class MainWindow : Window
    {
        private bool draw = false;
        private bool cooler3D = false;
        private bool HoWmUcHiHaTe3dInWpF = false;
        private Bitmap bitmapBachground;

        private Bitmap copyPlaceholder;

        private int WidthGlobal;
        private int HeightGlobal;

        private DrawingTools Tool = DrawingTools.Brush;
        public List<Controll.Image> layersImage = new List<Controll.Image>();
        public List<BitmapLayer> layersBitmap = new List<BitmapLayer>();

        private DrawingBoard DB;

        private int spread = 10;

        public MainWindow()
        {
            InitializeComponent();

            //New
            CommandBinding bind = new CommandBinding();
            bind.Command = ApplicationCommands.New;
            bind.Executed += CreateNew;
            this.CommandBindings.Add(bind);

            //Save
            bind = new CommandBinding();
            bind.Command = ApplicationCommands.Save;
            bind.Executed += SaveImage;
            bind.CanExecute += CanExtractExecute;
            this.CommandBindings.Add(bind);
            //Open
            bind = new CommandBinding();
            bind.Command = ApplicationCommands.Open;
            bind.Executed += OpenImage;
            bind.CanExecute += DBOpenned;
            this.CommandBindings.Add(bind);
            //Undo
            bind = new CommandBinding();
            bind.Command = ApplicationCommands.Undo;
            bind.Executed += Undo;
            bind.CanExecute += DBOpenned;
            this.CommandBindings.Add(bind);
            //Redo
            bind = new CommandBinding();
            bind.Command = ApplicationCommands.Redo;
            bind.Executed += Redo;
            bind.CanExecute += DBOpenned;
            this.CommandBindings.Add(bind);
            //Copy
            bind = new CommandBinding();
            bind.Command = ApplicationCommands.Copy;
            bind.Executed += Copy;
            bind.CanExecute += CanExtractExecute;
            this.CommandBindings.Add(bind);
            //Paste
            bind = new CommandBinding();
            bind.Command = ApplicationCommands.Paste;
            bind.Executed += Paste;
            bind.CanExecute += CanPasteExecute;
            this.CommandBindings.Add(bind);


        }

        private void SaveImage(object sender, RoutedEventArgs e)
        {
            DB.SaveFile();
        }

        Bitmap EmptyLayer;
        private void CreateNew(object sender, RoutedEventArgs e)
        {
            CreateNew();
        }
        private void CreateNew()
        {
            NewPixel Creating = new NewPixel();
            if (Creating.ShowDialog() == true)
            {
                int Width, Height;
                if (int.TryParse(Creating.Get_Width.Text, out Width) && int.TryParse(Creating.Get_Height.Text, out Height))
                {
                    EmptyLayer = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    HeightGlobal = Height;
                    WidthGlobal = Width;
                    DB = new DrawingBoard(Width, Height);
                    CreateBackGround(Width, Height);
                    UpdateLayersImage();
                    UpdateScreen();
                    UpdateAllLayers();
                }
                else MessageBox.Show("Wartosci są nieprawidłowe");
            }
        }
        private void OpenImage(object sender, RoutedEventArgs e)
        {
            OpenImage();
        }
        private void DBOpenned(object sender, CanExecuteRoutedEventArgs e)
        {
            if (DB != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
		
        }
        private void OpenImage()
        {
            DB.LoadLayer();
            Controll.Image image = new Controll.Image();
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            layersImage.Add(image);
            LayerGrid.Children.Add(image);
            UpdateAllLayers();
        }

        public void UpdateLayersImage()
        {
            for (int i = 0; i < layersImage.Count; i++)
            {
                RemoveImageLayer(0);
            }
            for (int i = 0; i < DB.layersBitmap.Count; i++)
            {
                AddLayerImage();
            }
        }

        private void UpdateScreen()
        {
            if (bitmapBachground == null) return;
            Screen_Background.Source = ConvertToImage(bitmapBachground);
            if (DB.CanDraw())
            {
                layersImage[DB.ActiveLayer].Source = DB.GetActiveBitmapLayer().IsVisible ? ConvertToImage(DB.GetActiveBitmapLayer().bitmap) : ConvertToImage(EmptyLayer);
                UpdateAllLayers();
                Screen.Source = ConvertToImage(EmptyLayer);
            }
            if (HoWmUcHiHaTe3dInWpF)
                CreateHateSpace();
            else
                EndHateSpace();
        }

        private void EndHateSpace()
        {
            HateSpace.Children.Clear();
        }

        private void CreateHateSpace()
        {
            HateSpace.Children.Clear();
            HateSpace.Children.Add(
                new DirectionalLight(
                    System.Windows.Media.Color.FromRgb(255, 255, 255), 
                    new Vector3D(-20, 20, 20)));
            for (int i = 0; i < DB.GetBitmapLayers().Count; i++)
            {
                if(DB.GetBitmapLayer(i).IsVisible)
                    HowIHate3DInWPF.EndThisWorld(HateSpace, DB.GetBitmapLayer(i).bitmap, i);
                else
                    HowIHate3DInWPF.EndThisWorld(HateSpace, EmptyLayer, i);

            }
        }

        private void UpdateAllLayers()
        {
            Layers.Items.Clear();
            foreach (BitmapLayer layer in DB.GetBitmapLayers())
            {
                ListElement listel = new ListElement();
                listel.Label = layer.name;
                listel.Image = ConvertToImage(layer.bitmap);
                listel.IsVisibleFun = layer.IsVisible;
                //Controll.Label label = new Controll.Label();
                //label.Content = layer.name;
                Layers.Items.Add(listel);
            }
            Layers.SelectedIndex = DB.ActiveLayer;
            for (int i = 0; i < DB.GetBitmapLayers().Count(); i++)
            {
                if (DB.GetBitmapLayer(i).IsVisible)
                    layersImage[i].Source = ConvertToImage(DB.GetBitmapLayers()[i].bitmap);
                else
                    layersImage[i].Source = ConvertToImage(EmptyLayer);
            }
            if (HoWmUcHiHaTe3dInWpF)
                CreateHateSpace();
            else
                EndHateSpace();
        }

        public BitmapImage ConvertToImage(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        List<Point> polypoints;
        private void StartDraw(object sender, MouseButtonEventArgs e)
        {
            if (DB == null) return;
            if (draw) return;
            if (!DB.CanDraw()) return;
            Point point = Get_Mouse_Position(e);
            if (point == null) return;

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                // Color Picker
                System.Drawing.Color backupcolor;
                backupcolor = DB.GetPixel(point);
                Show_Color.Fill = new SolidColorBrush(TranstalteColor(backupcolor));
            }
            else if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                draw = true;
                DB.StartDrawing(point, new Eraser(DB));

            }
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (Tool == DrawingTools.Pencil)
                {
                    // This is a Pencil
                    draw = true;
                    DB.StartDrawing(point, new Pencil(DB, TranstalteColor(BrushColor(Show_Color.Fill))));
                }
                else if (Tool == DrawingTools.FillBucket)
                {
                    // This is a Bucket
                    draw = true;
                    DB.StartDrawing(point, new Bucket(DB, TranstalteColor(BrushColor(Show_Color.Fill))));
                }
                else if (Tool == DrawingTools.FloodBucket)
                {
                    // This is also a Bucket but it fill everything 
                    draw = true;
                    DB.StartDrawing(point, new FillBucket(DB, TranstalteColor(BrushColor(Show_Color.Fill))));
                }
                else if (Tool == DrawingTools.Eraser)
                {
                    //This is Erase Tool
                    draw = true;
                    DB.StartDrawing(point, new Eraser(DB));
                }
                else if (Tool == DrawingTools.LineTool)
                {
                    //This is Line Tool
                    draw = true;
                    DB.StartDrawing(point, new DrawLine(DB, TranstalteColor(BrushColor(Show_Color.Fill)), point));

                }
                else if (Tool == DrawingTools.RectangleTool)
                {
                    //This is Rectangle Tool
                    draw = true;
                    DB.StartDrawing(point, new DrawRect(DB, TranstalteColor(BrushColor(Show_Color.Fill)), point));
                }
                else if (Tool == DrawingTools.EllipseTool)
                {
                    //This is Ellipse Tool yes its new
                    draw = true;
                    DB.StartDrawing(point, new DrawElli(DB, TranstalteColor(BrushColor(Show_Color.Fill)), point));
                }
                else if (Tool == DrawingTools.Brush)
                {
                    draw = true;
                    DB.StartDrawing(point, new Tools.Brush(DB, TranstalteColor(BrushColor(Show_Color.Fill)), CreateBrushMask()));
                }
                else if (Tool == DrawingTools.ColorBrake)
                {
                    ColorBrake color = new ColorBrake();
                    if (color.Function(DB.GetActiveBitmapLayer().bitmap, DB.GetPixel(point)))
                    {
                        DB.Replace(color.Background);
                        DB.AddLayer(DB.GetActiveBitmapLayer().name + "Color");
                        DB.Replace(color.Object);
                        AddLayerImage();
                        UpdateAllLayers();
                    }
                }
                else if (Tool == DrawingTools.NiBlackBrush)
                {
                    draw = true;
                    DB.StartDrawing(point, new Tools.NiBlackBrush(DB, CreateBrushMask()));
                }
                else if (Tool == DrawingTools.OtsuBrush)
                {
                    draw = true;
                    
                    DB.StartDrawing(point, new Tools.OtsuBrush(DB, CreateBrushMask()));
                }
                else if(Tool==DrawingTools.PolyDraw)
                {
                    if(Keyboard.IsKeyDown(Key.LeftShift)||polypoints==null)
                    {
                        if (polypoints == null)
                            polypoints = new List<Point>();
                        polypoints.Add(point);

                    }
                    else
                    {
                        DB.StartDrawing(point, new Tools.DrawPoly(DB, TranstalteColor(BrushColor(Show_Color.Fill)), point, polypoints));
                        polypoints = null;
                    }
                }
            }
        }

        private bool[][] CreateBrushMask()
        {
            if (!int.TryParse(size.Text, out int brushSize))
                brushSize = 5;
            bool[][] colors = new bool[brushSize][];
            for (int x = 0; x < brushSize; x++)
            {
                colors[x] = new bool[brushSize];
            }
            for (int x = 0; x < brushSize; x++)
            {
                colors[x][brushSize / 2] = true;
            }
            for (int x = 0; x < brushSize; x++)
            {
                colors[brushSize / 2][x] = true;
            }
            return colors;
        }

        private Point Get_Mouse_Position(MouseEventArgs e)
        {
            double pozx = (int)e.GetPosition(Screen).X;
            double pozy = (int)e.GetPosition(Screen).Y;

            int bitmapx = WidthGlobal;
            int bitmapy = HeightGlobal;
            double size = LayerGrid.ActualWidth > LayerGrid.ActualHeight ? LayerGrid.ActualHeight : LayerGrid.ActualWidth;
            double ratioscale = bitmapx > bitmapy ? bitmapy : bitmapx;
            double realratio = size / ratioscale;
            double Realx = realratio * bitmapx;
            double Realy = realratio * bitmapx;

            UpdateScreen(); //To jakimś cudem naprawia error z lagiem przy starcie programu
            if (pozx < 0 || pozy < 0)
                return null;
            double scalex = pozx / Realx;
            double scaley = pozy / Realy;

            int pixelx = (int)(bitmapx * scalex);
            int pixely = (int)(bitmapy * scaley);

            return new Point(pixelx, pixely);
        }

        private void StopDraw(object sender, MouseButtonEventArgs e)
        {
            if (!draw) return;
            draw = false;
            Point point = Get_Mouse_Position(e);
            DB.StopDrawing(point);
        }

        private void Draw(object sender, MouseEventArgs e)
        {
            Point point = Get_Mouse_Position(e);
            if (point == null) return;
            if (draw)
            {
                DB.Draw(point);
            }
        }



        private void Undo()
        {
            DB.Undo();
            UpdateLayersImage();
            UpdateAllLayers();
            UpdateScreen();
        }

        private void Redo()
        {
            DB.Redo();
            UpdateLayersImage();
            UpdateAllLayers();
            UpdateScreen();
        }
        private void CreateBackGround(int width, int height)
        {
            bitmapBachground = new Bitmap(width * 2, height * 2);

            for (int x = 0; x < bitmapBachground.Width; x++)
            {
                for (int y = 0; y < bitmapBachground.Height; y++)
                {
                    bitmapBachground.SetPixel(x, y, (x + y) % 2 == 1 ? System.Drawing.Color.FromArgb(255, 100, 100, 100) : System.Drawing.Color.FromArgb(255, 50, 50, 50));
                }
            }
        }

        private void Change_Color(object sender, MouseButtonEventArgs e)
        {
            
            System.Windows.Shapes.Rectangle rectangle = sender as System.Windows.Shapes.Rectangle;
            
            if (rectangle != null)
            {
                ChangeColor change = new ChangeColor();
                Binding bind = new Binding();
                bind.Source = change.Show_Color;
                bind.Path = new PropertyPath("Fill");
                bind.Mode = BindingMode.OneWay;

                
                SolidColorBrush color = (SolidColorBrush)rectangle.Fill;
                
                change.Red_color.Text = color.Color.R.ToString();
                change.Green_color.Text = color.Color.G.ToString();
                change.Blue_color.Text = color.Color.B.ToString();
                
                rectangle.SetBinding(System.Windows.Shapes.Rectangle.FillProperty, bind);
                if (change.ShowDialog() != true)
                {
                    rectangle.Fill = color;
                }

            }
            
        }

        private void Set_Color(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Shapes.Rectangle rectangle = sender as System.Windows.Shapes.Rectangle;
            if (rectangle != null)
            {
                Show_Color.Fill = rectangle.Fill;
            }
        }

        private void RepositionLayers()
        {
            for (int i = 0; i < layersImage.Count; i++)
            {
                int distance = (i - DB.ActiveLayer) * spread;
                layersImage[i].Margin = new Thickness(distance, distance, -distance, -distance);
            }
        }

        private void RepositionLayersFix()
        {
            for (int i = 0; i < layersImage.Count; i++)
            {
                layersImage[i].Margin = new Thickness(0, 0, -0, -0);
            }
        }

        private void AddLayer(object sender, RoutedEventArgs e)
        {
            if (DB == null) return;
            Naming name = new Naming();
            if (name.ShowDialog() == true)
            {
                ListElement listElement = new ListElement();
                listElement.Label = name.Input.Text;
                //Controll.Label label = new Controll.Label();
                //label.Content = name.Input.Text;
                Layers.Items.Add(listElement);
                
                DB.AddLayer(name.Input.Text);
                AddLayerImage();
            }
        }

        public void AddLayerImage()
        {
            Controll.Image image = new Controll.Image();
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            layersImage.Add(image);
            LayerGrid.Children.Add(image);
            if (cooler3D)
                RepositionLayers();
            else
                RepositionLayersFix();
        }

        private void RemoveLayer(object sender, RoutedEventArgs e)
        {
            if (Layers.SelectedIndex >= 0)
            {
                int temp = Layers.SelectedIndex;
                Layers.SelectedIndex = -1;
                RemoveLayer(temp);
            }
        }

        private void RemoveLayer(int index)
        {
            DB.RemoveLayer(index);
            RemoveImageLayer(index);
        }

        private void RemoveImageLayer(int index)
        {
            if (layersImage.Count <= 0 ||
                Layers.Items.Count <= 0 ||
                index >= layersImage.Count ||
                index >= Layers.Items.Count) return;

            LayerGrid.Children.Remove(layersImage[index]);
            layersImage.RemoveAt(index);
            Layers.Items.RemoveAt(index);
        }

        private void ChangeLayer(object sender, Controll.SelectionChangedEventArgs e)
        {
            if (Layers.SelectedIndex >= 0)
            {
                DB.ActiveLayer = Layers.SelectedIndex;
                if (cooler3D)
                    RepositionLayers();
                else
                    RepositionLayersFix();
            }
        }

        private void UpLayer(object sender, RoutedEventArgs e)
        {
            if (Layers.SelectedIndex >= 0)
            {
                DB.LayerUp(Layers.SelectedIndex);
                UpdateAllLayers();
                UpdateScreen();
            }
        }

        private void DownLayer(object sender, RoutedEventArgs e)
        {
            if (Layers.SelectedIndex >= 0)
            {
                if (DB.LayerDown(Layers.SelectedIndex))
                {
                    UpdateAllLayers();
                    UpdateScreen();
                }
            }
        }

        private System.Drawing.Color TranstalteColor(System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(255, color.R, color.G, color.B);
        }

        private System.Windows.Media.Color TranstalteColor(System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(255, color.R, color.G, color.B);
        }
        private System.Windows.Media.Color BrushColor(System.Windows.Media.Brush brush)
        {
            SolidColorBrush bruh = (SolidColorBrush)brush;
            return bruh.Color;
        }

        Controll.Button lastButton;
        private void Change_Tool(object sender, RoutedEventArgs e)
        {
            Controll.Button button = sender as Controll.Button;
            button.IsEnabled = false;
            if (lastButton != null)
            {
                lastButton.IsEnabled = true;
            }
            lastButton = button;
            switch (button.Name)
            {
                case "Pencil": Tool = DrawingTools.Pencil; break;
                case "Fill_Tool": Tool = DrawingTools.FillBucket; break;
                case "FillAll_Tool": Tool = DrawingTools.FloodBucket; break;
                case "Eraser": Tool = DrawingTools.Eraser; break;
                case "DrawLine": Tool = DrawingTools.LineTool; break;
                case "DrawRect": Tool = DrawingTools.RectangleTool; break;
                case "DrawEllipse": Tool = DrawingTools.EllipseTool; break;
                case "Brush": Tool = DrawingTools.Brush; break;
                case "ColorBrake": Tool = DrawingTools.ColorBrake; break;
                case "NiBlackBrush": Tool = DrawingTools.NiBlackBrush; break;
                case "OtsuBrush": Tool = DrawingTools.OtsuBrush; break;
                case "DrawPoly": Tool = DrawingTools.PolyDraw; break;




                default:
                    break;
            }
        }
        private void Copy()
        {
            copyPlaceholder = new Bitmap(DB.GetActiveBitmapLayer().bitmap);
        }
        private void CanExtractExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (DB != null&&DB.ActiveLayer>=0)
                e.CanExecute = true;
            else
                e.CanExecute = false;

        }
        private void ExtractLayer(object sender, RoutedEventArgs e)
        {
            DB.ExtractLayer();
        }

        private void ExtractAll(object sender, RoutedEventArgs e)
        {
            DB.ExtractAll();
            
        }
        private void ExtractToP1(object sender, RoutedEventArgs e)
        {
            DB.SaveP1();
        }
        private void ExtractToP2(object sender, RoutedEventArgs e)
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

                DB.SaveP2(size);
            }
        }
        private void ExtractToP3(object sender, RoutedEventArgs e)
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

                DB.SaveP3(size);
            }
        }
        private void ExtractToP4(object sender, RoutedEventArgs e)
        {
          DB.SaveP4();  
          
        }
        private void ExtractToP5(object sender, RoutedEventArgs e)
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

                DB.SaveP5(size);
            }
        }
        private void ExtractToP6(object sender, RoutedEventArgs e)
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

                DB.SaveP6(size);
            }
        }
        private void OpenNetpbm(object sender, RoutedEventArgs e)
        {
            DB.OpenNetpbm();
            Controll.Image image = new Controll.Image();
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            layersImage.Add(image);
            LayerGrid.Children.Add(image);
            UpdateAllLayers();
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            Copy();
        }

        private void Paste(object sender, RoutedEventArgs e)
        {
            DB.Paste(copyPlaceholder);
        }

        private void ExtractColor(object sender, RoutedEventArgs e)
        {

        }
        private void Undo(object sender, RoutedEventArgs e)
        {
            Undo();
        }

        private void Redo(object sender, RoutedEventArgs e)
        {
            Redo();
        }

        private void flat3D(object sender, RoutedEventArgs e)
        {
            cooler3D = !cooler3D;
            if (cooler3D)
                RepositionLayers();
            else
                RepositionLayersFix();
        }
        private void CanPasteExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (DB != null && DB.ActiveLayer >= 0 && copyPlaceholder != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void fat3D(object sender, RoutedEventArgs e)
        {
            HoWmUcHiHaTe3dInWpF = !HoWmUcHiHaTe3dInWpF;

            if (HoWmUcHiHaTe3dInWpF)
                CreateHateSpace();
            else
                EndHateSpace();
        }

        private void MergeLayer(object sender, RoutedEventArgs e)
        {
            if (Layers.SelectedIndex >= 1)
            {
                DB.CombineBitmaps(DB.GetBitmapLayer(Layers.SelectedIndex-1).bitmap, DB.GetActiveBitmapLayer().bitmap);
                RemoveLayer(sender, e);
                UpdateAllLayers();
            }



        }

        private void VisibleLayer(object sender, RoutedEventArgs e)
        {
            if (Layers.SelectedIndex >= 0)
            {

                bool test = !DB.GetActiveBitmapLayer().IsVisible;
                DB.GetActiveBitmapLayer().IsVisible = !DB.GetActiveBitmapLayer().IsVisible;
                UpdateAllLayers();
            }
        }

        private void Streching(object sender, RoutedEventArgs e)
        {
            NewPixel pixel = new NewPixel();

            if (pixel.ShowDialog() == true)
            {
                int min, max;
                if (!int.TryParse(pixel.Get_Width.Text, out min)) return;
                if (!int.TryParse(pixel.Get_Height.Text, out max)) return;

                HistogramSteching steching = new HistogramSteching();
                DB.Paste(steching.Function(DB.GetActiveBitmapLayer().bitmap, min, max));
            }
            
        }

        private void Equailization(object sender, RoutedEventArgs e)
        {
            HistogramEqual equal = new HistogramEqual();
            DB.Replace(equal.Function(DB.GetActiveBitmapLayer().bitmap));
        }

        private void Threshold(object sender, RoutedEventArgs e)
        {
            Sized sized = new Sized();
            if(sized.ShowDialog()==true)
            {
                int m;
                if (!int.TryParse(sized.Input.Text, out m)) return;
                BinaryTreshhold threshold = new BinaryTreshhold();
                DB.Replace(threshold.BinaryThreshold(DB.GetActiveBitmapLayer().bitmap, (byte) m , true));
            }
        }

        private void Otsu(object sender, RoutedEventArgs e)
        {
            Otsu otsu = new Otsu();
            DB.Replace(otsu.OtsuMethod(DB.GetActiveBitmapLayer().bitmap));
        }

        private void NiBlack(object sender, RoutedEventArgs e)
        {
            NiBlack niBlack = new NiBlack();
            DB.Replace(niBlack.Function(DB.GetActiveBitmapLayer().bitmap));
        }

        private void ReverseColors(object sender, RoutedEventArgs e)
        {
            RevertColors rev = new RevertColors();
            DB.Replace(rev.Function(DB.GetActiveBitmapLayer().bitmap));
        }

        private void Kontrast(object sender, RoutedEventArgs e)
        {

            Sized sized = new Sized();
            if (sized.ShowDialog() == true)
            {
                float m;
                if (!float.TryParse(sized.Input.Text, out m)) return;

                Kontrast kon = new Kontrast();
                DB.Replace(kon.Function(DB.GetActiveBitmapLayer().bitmap,m));
            }
        }

        private void Jasnosc(object sender, RoutedEventArgs e)
        {
            Sized sized = new Sized();
            if (sized.ShowDialog() == true)
            {
                float m;
                if (!float.TryParse(sized.Input.Text, out m)) return;

                Jasnosc jan = new Jasnosc();
                DB.Replace(jan.Function(DB.GetActiveBitmapLayer().bitmap, (int)m));
            }
        }

        private void Szkieletyzacja(object sender, RoutedEventArgs e)
        {
            Szkieletyzacja szkieletyzacja = new Szkieletyzacja();
            DB.Replace(szkieletyzacja.KMM(DB.GetActiveBitmapLayer().bitmap));
        }

        private void ChangebrushSys(object sender, RoutedEventArgs e)
        {

        }

        private void SplitLayer(object sender, RoutedEventArgs e)
        {
            SplitLayer split = new SplitLayer();

            if (split.Function(DB.GetActiveBitmapLayer().bitmap))
            {
                DB.Replace(split.Background);
                DB.AddLayer(DB.GetActiveBitmapLayer().name + "Object");
                DB.Replace(split.Object);
                AddLayerImage();
                UpdateAllLayers();
            }

        }
    }

    public class Point
    {
        public int x;
        public int y;
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public enum DrawingTools
    {
        Pencil,
        FillBucket,
        FloodBucket,
        Eraser,
        LineTool,
        RectangleTool,
        EllipseTool,
        Brush,
        ColorBrake,
        NiBlackBrush,
        OtsuBrush,
        PolyDraw
    }

    public static class CustomCommands
    {
        public static readonly RoutedUICommand ExtractLayer =
            new RoutedUICommand
            (
                "ExtractLayer",
                "ExtractLayer",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.E,ModifierKeys.Control)
                }
            );
        public static readonly RoutedUICommand ExtractAll =
            new RoutedUICommand
                (
                    "ExtractAll",
                    "ExtractAll",
                    typeof(CustomCommands),
                    new InputGestureCollection()
                    {
                        new KeyGesture(Key.R,ModifierKeys.Control)
                    }
    );        
        
        public static readonly RoutedUICommand ExtractToP1 =
            new RoutedUICommand
                (
                    "ExtractToP1",
                    "ExtractToP1",
                    typeof(CustomCommands),
                    new InputGestureCollection()
                    {
                        new KeyGesture(Key.L,ModifierKeys.Control)
                    }
    );         
        public static readonly RoutedUICommand ExtractToP2 =
            new RoutedUICommand
                (
                    "ExtractToP2",
                    "ExtractToP2",
                    typeof(CustomCommands),
                    new InputGestureCollection()
                    {
                        new KeyGesture(Key.L,ModifierKeys.Control)
                    }
    );         
        public static readonly RoutedUICommand ExtractToP3 =
            new RoutedUICommand
                (
                    "ExtractToP3",
                    "ExtractToP3",
                    typeof(CustomCommands),
                    new InputGestureCollection()
                    {
                        new KeyGesture(Key.L,ModifierKeys.Control)
                    }
    );         
        public static readonly RoutedUICommand ExtractToP4 =
            new RoutedUICommand
                (
                    "ExtractToP4",
                    "ExtractToP4",
                    typeof(CustomCommands),
                    new InputGestureCollection()
                    {
                        new KeyGesture(Key.L,ModifierKeys.Control)
                    }
    );        
        public static readonly RoutedUICommand ExtractToP5 =
            new RoutedUICommand
                (
                    "ExtractToP5",
                    "ExtractToP5",
                    typeof(CustomCommands),
                    new InputGestureCollection()
                    {
                        new KeyGesture(Key.L,ModifierKeys.Control)
                    }
    );         
        public static readonly RoutedUICommand ExtractToP6 =
            new RoutedUICommand
                (
                    "ExtractToP6",
                    "ExtractToP6",
                    typeof(CustomCommands),
                    new InputGestureCollection()
                    {
                        new KeyGesture(Key.L,ModifierKeys.Control)
                    }
    ); 
        public static readonly RoutedUICommand OpenNetpbm =
            new RoutedUICommand
                (
                    "OpenNetpbm",
                    "OpenNetpbm",
                    typeof(CustomCommands),
                    new InputGestureCollection()
                    {
                        new KeyGesture(Key.I,ModifierKeys.Control)
                    }
    );
    }
}

