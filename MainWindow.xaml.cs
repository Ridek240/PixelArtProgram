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
                                //Bitmap saveBitmap = new Bitmap(ActiveBitmap);
                                //ActiveBitmap.Dispose();
                                foreach(BitmapLayer bitmap in layersBitmap)
                                {
                                    bitmap.bitmap.Save(saveFileDialog.FileName + "/" + bitmap.name + "." + i.ToString(), ImageFormat.Png);
                                    i++;
                                }
                                //ActiveBitmap = new Bitmap(saveBitmap);
                            }
                        }
                        catch { _ = MessageBox.Show("Błąd w zapisywaniu pliku"); }
                    }
                }
            }
            else 
                MessageBox.Show("Plik nie istnieje");
            
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
            //Screen.Source = ConvertToImage(ActiveBitmap);
            Screen_Background.Source = ConvertToImage(bitmapBachground);
            //if (activeLayer >= 0)
            if (DB.CanDraw())
            {
                //layersImage[activeLayer].Source = ConvertToImage(layersBitmap[activeLayer].bitmap);
                layersImage[DB.ActiveLayer].Source = ConvertToImage(DB.GetActiveBitmapLayer().bitmap);
                
                Screen.Source = ConvertToImage(EmptyLayer);
            }
            //Screen_Background2.Source = ConvertToImage(bitmapBachground2);
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
            //if (activeLayer < 0) return;
            //if (layersBitmap.Count <= 0) return;
            //if (layersBitmap[activeLayer] == null) return;

            Point point = Get_Mouse_Position(e);
            if (point == null) return;

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                // Color Picker
                System.Drawing.Color backupcolor;
                //backupcolor = layersBitmap[activeLayer].bitmap.GetPixel(point.x, point.y);
                backupcolor = DB.GetPixel(point);
                Show_Color.Fill = new SolidColorBrush(TranstalteColor(backupcolor));
            }
            else if(Tools_ID==0)
            {
                // Pencil
                draw = true;
                DB.StartDrawing(point, new Pencil(TranstalteColor(BrushColor(Show_Color.Fill))));
                //layersBitmap[activeLayer].bitmap.SetPixel(point.x, point.y, System.Drawing.Color.FromArgb(128, 128, 128, 125));
            }
            else if(Tools_ID==1)
            {
                // This is a bucket

                //System.Drawing.Color pixelC = BitmapSrc.GetPixel(pixelx, pixely);
                //System.Drawing.Point pixelP = new System.Drawing.Point(pixelx, pixely);

                //BitmapResult = new Bitmap(BitmapSrc);
                //if (false) return;
                //layersBitmap[activeLayer].bitmap = Blackout(layersBitmap[activeLayer].bitmap.GetPixel(point.x, point.y), layersBitmap[activeLayer].bitmap, 0);
                //else
                //layersBitmap[activeLayer].bitmap = Fill(new System.Drawing.Point(point.x, point.y), layersBitmap[activeLayer].bitmap, TranstalteColor(BrushColor(Show_Color.Fill)) , 0 );
                //layersBitmap[DB.ActiveLayer].bitmap = Fill(new System.Drawing.Point(point.x, point.y), layersBitmap[DB.ActiveLayer].bitmap, TranstalteColor(BrushColor(Show_Color.Fill)) , 0 );
                DB.StartDrawing(point, new Bucket(TranstalteColor(BrushColor(Show_Color.Fill))));
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
            draw = false;
        }

        private void Draw(object sender, MouseEventArgs e)
        {
            //if (activeLayer < 0) return;
            //if (layersBitmap.Count <= 0) return;
            //if (layersBitmap[activeLayer] == null) return;

            Point point = Get_Mouse_Position(e);
            if (point == null) return;
            if (draw)
            {
                //layersBitmap[activeLayer].bitmap.SetPixel(point.x, point.y, TranstalteColor(BrushColor(Show_Color.Fill)));
                DB.Draw(point);
            }
            if(remove)
            {
                //layersBitmap[activeLayer].bitmap.SetPixel(point.x, point.y, System.Drawing.Color.FromArgb(0, 0, 0, 0));
                DB.Draw(point);
            }
        }

        private void StartRemove(object sender, MouseButtonEventArgs e)
        {
            remove = true;
            Point point = Get_Mouse_Position(e);
            //layersBitmap[activeLayer].bitmap.SetPixel(point.x, point.y, System.Drawing.Color.FromArgb(0, 0, 0, 0));
            DB.StartDrawing(point, new Eraser());
        }

        private void StopRemove(object sender, MouseButtonEventArgs e)
        {
            remove = false;
        }

        private void ShortCuts(object sender, KeyEventArgs e)
        {
            if(Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if(Keyboard.IsKeyDown(Key.S))
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
                    bitmapBachground.SetPixel(x,y, (x + y) % 2 == 1 ? System.Drawing.Color.FromArgb(255, 0, 0, 0) : System.Drawing.Color.FromArgb(255, 50, 50, 50));
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
        }

        private Bitmap Fill(System.Drawing.Point position, Bitmap bitmap, System.Drawing.Color ColorPixel, int range,  int maxIteration = 0)
        {
            //System.Drawing.Color blackpixel = System.Drawing.Color.FromArgb(0, 0, 0);
            System.Drawing.Color pixelCompateTo = bitmap.GetPixel(position.X, position.Y);

            Queue<System.Drawing.Point> pixels = new Queue<System.Drawing.Point>();
            pixels.Enqueue(position);

            bool[,] visited = new bool[bitmap.Width, bitmap.Height];
            Bitmap bitmapResult = new Bitmap(bitmap);

            int iteration = 0;
            while (pixels.Count > 0)
            {
                if (maxIteration > 0 && iteration++ >= maxIteration) break;

                System.Drawing.Point pixel = pixels.Dequeue();

                if (pixel.X < 0 || pixel.X >= bitmap.Width ||
                    pixel.Y < 0 || pixel.Y >= bitmap.Height ||
                    visited[pixel.X, pixel.Y]) continue;

                visited[pixel.X, pixel.Y] = true;

                if (GetTolerance(bitmap.GetPixel(pixel.X, pixel.Y), pixelCompateTo, range))
                {
                    bitmapResult.SetPixel(pixel.X, pixel.Y, ColorPixel);
                    foreach (System.Drawing.Point neighbour in neighbours)
                    {
                        System.Drawing.Point nPixel = new System.Drawing.Point(
                            pixel.X + neighbour.X,
                            pixel.Y + neighbour.Y);
                        pixels.Enqueue(nPixel);
                    }
                }
            }

            return bitmapResult;
        }
        private bool GetTolerance(System.Drawing.Color atpixel, System.Drawing.Color pixel, int range)
        {
            return atpixel.R >= pixel.R - range
                && atpixel.R <= pixel.R + range & atpixel.G >= pixel.G - range
                && atpixel.G <= pixel.G + range & atpixel.B >= pixel.B - range
                && atpixel.B <= pixel.B + range;
        }

        private static System.Drawing.Point[] neighbours =
        {
            new System.Drawing.Point(-1, 0),
            new System.Drawing.Point(1, 0),
            new System.Drawing.Point(0, 1),
            new System.Drawing.Point(0, -1)
        };

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

