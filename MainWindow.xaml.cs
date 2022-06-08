using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Controll = System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace PixelArtProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool draw = false;
        bool remove = false;
        Bitmap ActiveBitmap;
        string tempname;
        int NextVersion = 0;
        Bitmap bitmapBachground;

        int WidthGlobal;
        int HeightGlobal;
        
        int Tools_ID = 0;
        public List<Controll.Image> layersImage = new List<Controll.Image>();
        public List<BitmapLayer> layersBitmap = new List<BitmapLayer>();

        public DrawingBoard DB;

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
                    UpdateScreen();
                }
                else MessageBox.Show("Wartosci są nieprawidłowe");

            }
        }

        private void OpenImage(object sender, RoutedEventArgs e)
        {
            if (layersBitmap.Count() >= 0)
            {
                SaveImage(sender, e);
                layersBitmap.Clear();
                layersImage.Clear();
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Dead Files (*.png)|*.png|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog()==true)
            {
                ActiveBitmap = new Bitmap(openFileDialog.FileName);
                CreateBackGround(ActiveBitmap.Width,ActiveBitmap.Height);
                UpdateScreen();
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


        public bool Between(int a, int min = 0, int max = 255)
        {
            return a <= max && a >= min;
        }

        private void StartDraw(object sender, MouseButtonEventArgs e)
        {

            Point point = Get_Mouse_Position(e);
            if (point == null) return;

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                // Color Picker
                System.Drawing.Color backupcolor;
                backupcolor = DB.GetPixel(point);
                Show_Color.Fill = new SolidColorBrush(TranstalteColor(backupcolor));
            }
            else if(Tools_ID==0)
            {
                // Pencil
                draw = true;
                DB.StartDrawing(point, new Pencil(TranstalteColor(BrushColor(Show_Color.Fill))));
            }
            else if(Tools_ID==1)
            {
                // This is a bucket
                DB.StartDrawing(point, new Bucket(TranstalteColor(BrushColor(Show_Color.Fill))));
            }
            else if(Tools_ID==2)
            {
                // This is also a bucket but it fill everything 
                DB.StartDrawing(point, new FillBucket(TranstalteColor(BrushColor(Show_Color.Fill))));
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

            return new Point(pixelx,pixely);

        }

        private void StopDraw(object sender, MouseButtonEventArgs e)
        {
            if (!draw) return;
            draw = false;
            DB.StopDrawing();
        }

        private void Draw(object sender, MouseEventArgs e)
        {
            Point point = Get_Mouse_Position(e);
            if (point == null) return;
            if (draw)
            {
                DB.Draw(point);
            }
            if (remove)
            {
                DB.Draw(point);
            }
        }

        private void StartRemove(object sender, MouseButtonEventArgs e)
        {
            remove = true;
            Point point = Get_Mouse_Position(e);
            DB.StartDrawing(point, new Eraser());
        }

        private void StopRemove(object sender, MouseButtonEventArgs e)
        {
            remove = false;
        }

        private void ShortCuts(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (Keyboard.IsKeyDown(Key.S))
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    //saveFileDialog.Filter = "Nie nadpisuj utwórz nowy \n (*.png)|*.png|All files (*.*)|*.*";
                    if (tempname == null)
                    {
                        
                        if (saveFileDialog.ShowDialog() == true)
                            tempname = saveFileDialog.FileName.ToString();
                    }
                    ActiveBitmap.Save(tempname+NextVersion+".png", ImageFormat.Png);
                    NextVersion++;
                }
                if (Keyboard.IsKeyDown(Key.Z))
                {
                    DB.Undo();
                    UpdateScreen();
                }
                if (Keyboard.IsKeyDown(Key.Y))
                {
                    DB.Redo();
                    UpdateScreen();
                }
            }
            else
            {
               
            }
        }


        private void CreateBackGround(int width, int height)
        {
            bitmapBachground = new Bitmap(width * 2, height * 2);

            for (int x = 0; x < bitmapBachground.Width; x++)
            {
                for (int y = 0; y < bitmapBachground.Height; y++)
                {
                    bitmapBachground.SetPixel(x,y, (x + y) % 2 == 1 ? System.Drawing.Color.FromArgb(255, 100, 100, 100) : System.Drawing.Color.FromArgb(255, 50, 50, 50));
                }
            }
        }

        private void Change_Color(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Shapes.Rectangle rectangle = sender as System.Windows.Shapes.Rectangle;
            if(rectangle!=null)
            {
                ChangeColor change = new ChangeColor();
                if (change.ShowDialog() == true)
                {
                    rectangle.Fill = new SolidColorBrush(
                        System.Windows.Media.Color.FromArgb(
                            255, 
                            (byte)int.Parse(change.Red_color.Text), 
                            (byte)int.Parse(change.Green_color.Text), 
                            (byte)int.Parse(change.Blue_color.Text)));
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
                //layersBitmap.Add(new BitmapLayer(name.Input.Text,new Bitmap(WidthGlobal, HeightGlobal, System.Drawing.Imaging.PixelFormat.Format32bppArgb)));
                
                Controll.Image image = new Controll.Image();
                image.MouseLeftButtonDown += StartDraw;
                image.MouseLeftButtonUp += StopDraw;
                image.MouseMove += Draw;
                image.MouseRightButtonDown += StartRemove;
                image.MouseRightButtonUp += StopRemove;
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
                layersImage.Add(image);
                LayerGrid.Children.Add(image);
                //activeLayer = layersBitmap.Count - 1;
                //activeLayer = DB.ActiveLayer;
            }
        }

        private void RemoveLayer(object sender, RoutedEventArgs e)
        {
            if(Layers.SelectedIndex>=0)
            {
                int temp = Layers.SelectedIndex;
                Layers.SelectedIndex = -1;
                //layersBitmap.RemoveAt(temp);
                DB.RemoveLayer(temp);

                LayerGrid.Children.Remove(layersImage[temp]);
                layersImage.RemoveAt(temp);
                Layers.Items.RemoveAt(temp);
            }
        }
        private void ChangeLayer(object sender, Controll.SelectionChangedEventArgs e)
        {
            if (Layers.SelectedIndex >= 0)
            {
                //activeLayer = Layers.SelectedIndex;
                DB.ActiveLayer = Layers.SelectedIndex;
            }
        }
        private void UpLayer(object sender, RoutedEventArgs e)
        {
            
        }

        private void DownLayer(object sender, RoutedEventArgs e)
        {

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
            if(lastButton!=null)
            {
                lastButton.IsEnabled = true;
            }
            lastButton = button;
            if(button.Name=="Pencil")
            {
                Tools_ID = 0;
            }
            if(button.Name=="Fill_Tool")
            {
                Tools_ID = 1;
            }
            if(button.Name=="FillAll_Tool")
            {
                Tools_ID = 2;
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
    //wyodrębnić



}

