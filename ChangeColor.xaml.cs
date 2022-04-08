using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PixelArtProgram
{
    /// <summary>
    /// Logika interakcji dla klasy ChangeColor.xaml
    /// </summary>
    public partial class ChangeColor : Window
    {
        bool system_ready = false;
        public ChangeColor()
        {
            InitializeComponent();
            system_ready = true;
        }

        private void Switch_Color(object sender, TextChangedEventArgs e)
        {
            if (system_ready)
            {
                int red_value, blue_value, green_value;
                if (int.TryParse(Red_color.Text, out red_value) && int.TryParse(Green_color.Text, out green_value) && int.TryParse(Blue_color.Text, out blue_value) &&
                    Between(red_value) && Between(blue_value) && Between(green_value))
                {
                    Error_Label.Content = "Brak Błędów";
                    //actualColor = System.Drawing.Color.FromArgb(255, red_value, green_value, blue_value);
                    Show_Color.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, (byte)red_value, (byte)green_value, (byte)blue_value));
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

        private void Confirm(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
