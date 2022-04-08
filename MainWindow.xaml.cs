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
        List<System.Drawing.Color> Using_colors = new List<System.Drawing.Color>();
        System.Drawing.Color actualColor = System.Drawing.Color.FromArgb(255, 255, 255, 255);
        public MainWindow()
        {
            InitializeComponent();
            system_ready = true;
            for (int i = 0; i < 9; i++)
            {
                Using_colors.Add( System.Drawing.Color.FromArgb(255,255,255,255));
            }
            TranslateToList();
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
                            bitmapMain.Save(saveFileDialog.FileName, ImageFormat.Png);
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
                    //bitmapMain.MakeTransparent();
                    /*bitmapBachground = new Bitmap(Width, Height);
                    using (Graphics gfx = Graphics.FromImage(bitmapBachground))
                    using (SolidBrush brush = new SolidBrush(System.Drawing.Color.FromArgb(0, 0, 0)))
                    {
                        gfx.FillRectangle(brush, 0, 0, Width, Height);
                    }*/
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
                UpdateScreen();
            }
            
           
        }
        private void UpdateScreen()
        {
            Screen.Source = ConvertToImage(bitmapMain);
            Screen_Background.Source = ConvertToImage(bitmapBachground);
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

        private void Switch_Color(object sender, TextChangedEventArgs e)
        {
            if (system_ready)
            {
                int red_value, blue_value, green_value;
                if (int.TryParse(Red_color.Text, out red_value) && int.TryParse(Green_color.Text, out green_value) && int.TryParse(Blue_color.Text, out blue_value)&& 
                    Between(red_value) && Between(blue_value) && Between(green_value))
                {
                    Error_Label.Content = "Brak Błędów";
                    actualColor = System.Drawing.Color.FromArgb(255, red_value, green_value, blue_value);
                    Show_Color.Fill = new SolidColorBrush( System.Windows.Media.Color.FromArgb(255, (byte)red_value, (byte)green_value, (byte)blue_value));
                    Slider_red.Value = red_value;
                    Slider_Green.Value = green_value;
                    Slider_Blue.Value = blue_value;
                    
                }
                else Error_Label.Content = "Błąd Kolorów";
            }
        }
        public bool Between(int a, int min = 0, int max = 255)
        {
            return a <= max && a >= min;
        }

        private void Slide_Color(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (system_ready)
            {
                Red_color.Text = Slider_red.Value.ToString();
                Green_color.Text = Slider_Green.Value.ToString();
                Blue_color.Text = Slider_Blue.Value.ToString();
            }
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
                //system_ready = false;

                Red_color.Text = backupcolor.R.ToString();
                Green_color.Text = backupcolor.G.ToString();
                Blue_color.Text = backupcolor.B.ToString();


                //system_ready = true;
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

                bitmapMain.SetPixel(pixelx, pixely, actualColor);
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

                    bitmapMain.SetPixel(pixelx, pixely, actualColor);
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
                if(Keyboard.IsKeyDown(Key.D1))
                {
                    ChangeColors(0);
                }
                if (Keyboard.IsKeyDown(Key.D2))
                {
                    ChangeColors(1);
                }
                if (Keyboard.IsKeyDown(Key.D3))
                {
                    ChangeColors(2);
                }
                if (Keyboard.IsKeyDown(Key.D4))
                {
                    ChangeColors(3);
                }
                if (Keyboard.IsKeyDown(Key.D5))
                {
                    ChangeColors(4);
                }
                if (Keyboard.IsKeyDown(Key.D6))
                {
                    ChangeColors(5);
                }
                if (Keyboard.IsKeyDown(Key.D7))
                {
                    ChangeColors(6);
                }
                if (Keyboard.IsKeyDown(Key.D8))
                {
                    ChangeColors(7);
                }
                if (Keyboard.IsKeyDown(Key.D9))
                {
                    ChangeColors(8);
                }
            }
        }

        private void ChangeColors(int i)
        {
            Red_color.Text = Using_colors[i].R.ToString();
            Green_color.Text = Using_colors[i].G.ToString();
            Blue_color.Text = Using_colors[i].B.ToString();
        }

        private void TranslateToList()
        {
            Colors.Items.Clear();
            foreach(System.Drawing.Color color in Using_colors)
            {
                System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
                rectangle.Height = 16;
                rectangle.Width = 16;
                rectangle.Stroke = System.Windows.Media.Brushes.Black;
                rectangle.StrokeThickness = 1;
                rectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, color.R, color.G, color.B));
                Colors.Items.Add(rectangle);
            }
        }





        private void Set_Color(object sender, SelectionChangedEventArgs e)
        {
            if (Colors.SelectedIndex >= 0)
            {
                Red_color.Text = Using_colors[Colors.SelectedIndex].R.ToString();
                Green_color.Text = Using_colors[Colors.SelectedIndex].G.ToString();
                Blue_color.Text = Using_colors[Colors.SelectedIndex].B.ToString();
            }
        }

        private void Change_color(object sender, RoutedEventArgs e)
        {
            ChangeColor change = new ChangeColor();
            if(change.ShowDialog()==true && Colors.SelectedIndex >= 0)
            {
                Using_colors[Colors.SelectedIndex] = System.Drawing.Color.FromArgb(255, int.Parse(change.Red_color.Text), int.Parse(change.Green_color.Text), int.Parse(change.Blue_color.Text));
                TranslateToList();
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
    }



}
