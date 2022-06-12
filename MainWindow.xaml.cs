using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using Controll = System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using PixelArtProgram.Tools;
using System.Windows.Data;

namespace PixelArtProgram
{

    public partial class MainWindow : Window
    {
        private bool draw = false;
        private Bitmap bitmapBachground;

        private int WidthGlobal;
        private int HeightGlobal;

        private DrawingTools Tool = DrawingTools.Pencil;
        public List<Controll.Image> layersImage = new List<Controll.Image>();
        public List<BitmapLayer> layersBitmap = new List<BitmapLayer>();

        private DrawingBoard DB;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SaveImage(object sender, RoutedEventArgs e)
        {
            DB.SaveFile();
        }

        Bitmap EmptyLayer;
        private void CreateNew(object sender, RoutedEventArgs e)
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
            Screen_Background.Source = ConvertToImage(bitmapBachground);
            if (DB.CanDraw())
            {
                layersImage[DB.ActiveLayer].Source = ConvertToImage(DB.GetActiveBitmapLayer().bitmap);

                Screen.Source = ConvertToImage(EmptyLayer);
            }
        }
        private void UpdateAllLayers()
        {
            Layers.Items.Clear();
            foreach (BitmapLayer layer in DB.GetBitmapLayers())
            {
                Controll.Label label = new Controll.Label();
                label.Content = layer.name;
                Layers.Items.Add(label);
            }
            Layers.SelectedIndex = DB.ActiveLayer;
            for (int i = 0; i < DB.GetBitmapLayers().Count(); i++)
            {
                layersImage[i].Source = ConvertToImage(DB.GetBitmapLayers()[i].bitmap);
            }
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

        
        private void StartDraw(object sender, MouseButtonEventArgs e)
        {
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
            }
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

        private void ShortCuts(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {

                if (Keyboard.IsKeyDown(Key.Z))
                {
                    DB.Undo();
                    UpdateLayersImage();
                    UpdateAllLayers();
                    UpdateScreen();
                }
                if (Keyboard.IsKeyDown(Key.Y))
                {
                    DB.Redo();
                    UpdateLayersImage();
                    UpdateAllLayers();
                    UpdateScreen();
                }
            }

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

        private void AddLayer(object sender, RoutedEventArgs e)
        {
            Naming name = new Naming();
            if (name.ShowDialog() == true)
            {
                Controll.Label label = new Controll.Label();
                label.Content = name.Input.Text;
                Layers.Items.Add(label);

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
                default:
                    break;
            }
        }

        private void ExtractLayer(object sender, RoutedEventArgs e)
        {
            DB.ExtractLayer();
        }

        private void ExtractAll(object sender, RoutedEventArgs e)
        {
            DB.ExtractAll();
        }

        private void Cut(object sender, RoutedEventArgs e)
        {

        }

        private void Copy(object sender, RoutedEventArgs e)
        {

        }

        private void Paste(object sender, RoutedEventArgs e)
        {

        }

        private void ExtractColor(object sender, RoutedEventArgs e)
        {

        }
        private void Undo(object sender, RoutedEventArgs e)
        {

                DB.Undo();
                UpdateLayersImage();
                UpdateAllLayers();
                UpdateScreen();
           

        }

        private void Redo(object sender, RoutedEventArgs e)
        {

                DB.Redo();
                UpdateLayersImage();
                UpdateAllLayers();
                UpdateScreen();
            
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
        RectangleTool
    }

}

