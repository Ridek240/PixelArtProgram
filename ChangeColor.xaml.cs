using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;


namespace PixelArtProgram
{
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



        private void Confirm(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void DragAndMoveWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }


    }
