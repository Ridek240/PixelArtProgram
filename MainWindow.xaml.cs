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
using System.Windows.Controls;
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
        bool system_ready = false;
        bool draw = false;
        bool remove = false;
        Bitmap bitmapMain;
        string tempname;
        int NextVersion = 0;
        Bitmap bitmapBachground;
        Bitmap bitmapBachground2;
        List<System.Drawing.Color> Using_colors = new List<System.Drawing.Color>();
        System.Drawing.Color actualColor = System.Drawing.Color.FromArgb(255, 255, 255, 255);
        public MainWindow()
        {
            InitializeComponent();
            system_ready = true;

        }

        private void SaveImage(object sender, RoutedEventArgs e)
        {
            if (bitmapMain != null)
            {
                if (MessageBox.Show("Czy na pewno chcesz Zapisać?", "Usuń Element", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Nie nadpisuj utwórz nowy \n (*.png)|*.png|All files (*.*)|*.*";
                    if (saveFileDialog.ShowDialog() == true)
                     try
                     {
                    {
                        Bitmap saveBitmap = new Bitmap(bitmapMain);
                        bitmapMain.Dispose();
                        saveBitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
                        bitmapMain = new Bitmap(saveBitmap);
                    }
                        }
                        catch { MessageBox.Show("Błąd w zapisywaniu pliku"); }




                }
            }
            else MessageBox.Show("Plik nie istnieje");
            
        }


        private void CreateNew(object sender, RoutedEventArgs e)
        {
            NewPixel Creating = new NewPixel();
            if (Creating.ShowDialog() == true)
            {
                int Width, Height;
                if (int.TryParse(Creating.Get_Width.Text, out Width) && int.TryParse(Creating.Get_Height.Text, out Height))
                {
                    bitmapMain = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    bitmapMain.MakeTransparent();
                    bitmapBachground2 = new Bitmap(Width, Height);
                    using (Graphics gfx = Graphics.FromImage(bitmapBachground2))
                    using (SolidBrush brush = new SolidBrush(System.Drawing.Color.FromArgb(0, 0, 0)))
                    {
                        gfx.FillRectangle(brush, 0, 0, Width, Height);
                    }
                    CreateBackGround(Width, Height);
                    UpdateScreen();
                }
                else MessageBox.Show("Wartosci są nieprawidłowe");

            }
        }

        private void OpenImage(object sender, RoutedEventArgs e)
        {
            if (bitmapMain != null) SaveImage(sender, e);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Dead Files (*.png)|*.png|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog()==true)
            {
                bitmapMain = new Bitmap(openFileDialog.FileName);
                CreateBackGround(bitmapMain.Width,bitmapMain.Height);
                UpdateScreen();
            }
            
           
        }
        private void UpdateScreen()
        {
            Screen.Source = ConvertToImage(bitmapMain);
            Screen_Background.Source = ConvertToImage(bitmapBachground);
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
            
            if(Keyboard.IsKeyDown(Key.LeftShift))
            {
                double pozx = (int)e.GetPosition(Screen).X;
                double Realx = Screen.ActualWidth;
                int pozy = (int)e.GetPosition(Screen).Y;
                double Realy = Screen.ActualHeight;



                double scalex = pozx / Realx;
                double scaley = pozy / Realy;

                int bitmapx = bitmapMain.Width;
                int bitmapy = bitmapMain.Height;
                int pixelx = (int)(bitmapx * scalex);
                int pixely = (int)(bitmapy * scaley);
                System.Drawing.Color backupcolor;
                backupcolor = bitmapMain.GetPixel(pixelx, pixely);
                Show_Color.Fill = new SolidColorBrush(TranstalteColor(backupcolor));
            }
            else
            {
                draw = true;
                double pozx = (int)e.GetPosition(Screen).X;
                double Realx = Screen.ActualWidth;
                int pozy = (int)e.GetPosition(Screen).Y;
                double Realy = Screen.ActualHeight;

                double scalex = pozx / Realx;
                double scaley = pozy / Realy;

                int bitmapx = bitmapMain.Width;
                int bitmapy = bitmapMain.Height;
                int pixelx = (int)(bitmapx * scalex);
                int pixely = (int)(bitmapy * scaley);

                bitmapMain.SetPixel(pixelx, pixely, TranstalteColor(BrushColor(Show_Color.Fill)));
                UpdateScreen();
            }

            

        }


        private void StopDraw(object sender, MouseButtonEventArgs e)
        {
            draw = false;
        }

        private void Draw(object sender, MouseEventArgs e)
        {
            if (bitmapMain != null)
            {
                if (draw)
                {
                    double pozx = (int)e.GetPosition(Screen).X;
                    double Realx = Screen.ActualWidth;
                    int pozy = (int)e.GetPosition(Screen).Y;
                    double Realy = Screen.ActualHeight;



                    double scalex = pozx / Realx;
                    double scaley = pozy / Realy;

                    int bitmapx = bitmapMain.Width;
                    int bitmapy = bitmapMain.Height;
                    int pixelx = (int)(bitmapx * scalex);
                    int pixely = (int)(bitmapy * scaley);

                    bitmapMain.SetPixel(pixelx, pixely, TranstalteColor(BrushColor(Show_Color.Fill)));
                    UpdateScreen();
                }
                if(remove)
                {
                    double pozx = (int)e.GetPosition(Screen).X;
                    double Realx = Screen.ActualWidth;
                    int pozy = (int)e.GetPosition(Screen).Y;
                    double Realy = Screen.ActualHeight;



                    double scalex = pozx / Realx;
                    double scaley = pozy / Realy;

                    int bitmapx = bitmapMain.Width;
                    int bitmapy = bitmapMain.Height;
                    int pixelx = (int)(bitmapx * scalex);
                    int pixely = (int)(bitmapy * scaley);

                    bitmapMain.SetPixel(pixelx, pixely, System.Drawing.Color.FromArgb(0, 0, 0, 0));
                    UpdateScreen();
                }
            }

        }

        private void StartRemove(object sender, MouseButtonEventArgs e)
        {
            remove = true;
            double pozx = (int)e.GetPosition(Screen).X;
            double Realx = Screen.ActualWidth;
            int pozy = (int)e.GetPosition(Screen).Y;
            double Realy = Screen.ActualHeight;



            double scalex = pozx / Realx;
            double scaley = pozy / Realy;

            int bitmapx = bitmapMain.Width;
            int bitmapy = bitmapMain.Height;
            int pixelx = (int)(bitmapx * scalex);
            int pixely = (int)(bitmapy * scaley);

            bitmapMain.SetPixel(pixelx, pixely, System.Drawing.Color.FromArgb(0, 0, 0, 0));
            UpdateScreen();
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
                    bitmapMain.Save(tempname+NextVersion+".png", ImageFormat.Png);
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
                        rectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, (byte)int.Parse(change.Red_color.Text), (byte)int.Parse(change.Green_color.Text), (byte)int.Parse(change.Blue_color.Text)));
                        
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
            Label label = new Label();
            label.Content = "layertest";
            Layers.Items.Add(label);
        }

        private void RemoveLayer(object sender, RoutedEventArgs e)
        {
            if(Layers.SelectedIndex>=0)
            {
                Layers.Items.RemoveAt(Layers.SelectedIndex);
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


        Button lastButton;
        private void Change_Tool(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;
            if(lastButton!=null)
            {
                lastButton.IsEnabled = true;
            }
            lastButton = button;
        }
    }



}
