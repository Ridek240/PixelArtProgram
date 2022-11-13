using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System;

namespace PixelArtProgram
{
    public partial class ChangeColor : Window
    {
        bool system_ready = false;
        Color_Pick Picked = Color_Pick.RGB;
        public ChangeColor()
        {
            InitializeComponent();
            system_ready = true;
            RGBBOX.IsChecked = true;
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

                    if(Picked!=Color_Pick.CMYK)
                        ConvertToCMYK(red_value, green_value, blue_value);

                    if (Picked != Color_Pick.HSV)
                        ConvertToHSV(red_value, green_value, blue_value);

                }
                else Error_Label.Content = "Błąd Kolorów";
            }
        }
        public bool Between(int a, int min = 0, int max = 255)
        {
            return a <= max && a >= min;
        }

        private void ConvertToCMYK(int red_v,int green_v,int blue_v)
        {
            float c, m, y, k, rf, gf, bf;
            rf = red_v / 255f;
            gf = green_v / 255f;
            bf = blue_v / 255f;

            k = ClampCmyk(1 - Math.Max(Math.Max(rf, gf), bf));
            c = ClampCmyk((1 - rf - k) / (1 - k));
            m = ClampCmyk((1 - gf - k) / (1 - k));
            y = ClampCmyk((1 - bf - k) / (1 - k));

            Slider_cyan.Value = (int)(c * 100);
            Slider_Magenta.Value = (int)(m * 100);
            Slider_Yellow.Value = (int)(y * 100);
            Slider_Black.Value = (int)(k * 100);

        }
        private void ConvertToHSV(int red_v, int green_v, int blue_v)
        {
            float delta, min, h = 0, s, v;

            min = Math.Min(Math.Min(red_v, green_v), blue_v);
            v = Math.Max(Math.Max(red_v, green_v), blue_v);
            delta = v - min;

            if (v == 0f)
            {
                s = 0;
            }
            else
                s = delta / v;

            if (s == 0f)
                h = 0f;
            else
            {
                if (red_v == v)
                    h = (green_v - blue_v) / delta;
                else if (green_v == v)
                    h = 2 + (blue_v - red_v) / delta;
                else if (blue_v == v)
                    h = 4 + (red_v - green_v) / delta;

                h *= 60;

                if (h <= 0f)
                    h = h + 360;
            }



            Slider_Hue.Value = (int)h;
            Slider_Saturation.Value = (int)(s*100);
            Slider_Value.Value = (int)((v/255F)*100);
        }
 

        private static float ClampCmyk(float value)
        {
            if (value < 0 || float.IsNaN(value))
            {
                value = 0;
            }

            return value;
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



        private void Switch_CMYK(object sender, TextChangedEventArgs e)
        {
            if (Picked != Color_Pick.CMYK)
                return;
            float c = (float)(Slider_cyan.Value) / 100f;
            float m = (float)(Slider_Magenta.Value) / 100f;
            float y = (float)(Slider_Yellow.Value) / 100f;
            float k = (float)(Slider_Black.Value) / 100f;

            int r, g, b;

            r = Convert.ToInt32(255 * (1 - c) * (1 - k));
            g = Convert.ToInt32(255 * (1 - m) * (1 - k));
            b = Convert.ToInt32(255 * (1 - y) * (1 - k));

            Slider_red.Value = r;
            Slider_Green.Value = g;
            Slider_Blue.Value = b;

        }

        private void Switch_HSV(object sender, TextChangedEventArgs e)
        {
            float r = 0, g=0, b = 0;
            if (Picked != Color_Pick.HSV)
                return;
            float s = (float)Slider_Saturation.Value/100f, h = (float)Slider_Hue.Value, v = (float)Slider_Value.Value*255/100;

            if (s==0)
            {
                r = h;
                g = h;
                b = h;
            }
            else
            {
                int i;
                float f, p, q, t;

                if (h == 360)
                    h = 0;
                else h = h / 60;

                i = (int)Math.Truncate(h);
                f = h - i;

                p = v * (1.0f - s);
                q = v * (1.0f - (s*f));
                t = v * (1.0f - (s * (1.0f-f)));


                switch (i)
                {
                    case 0:
                        r = v;
                        g = t;
                        b = p;
                        break;

                    case 1:
                        r = q;
                        g = v;
                        b = p;
                        break;

                    case 2:
                        r = p;
                        g = v;
                        b = t;
                        break;

                    case 3:
                        r = p;
                        g = q;
                        b = v;
                        break;

                    case 4:
                        r = t;
                        g = p;
                        b = v;
                        break;

                    default:
                        r = v;
                        g = p;
                        b = q;
                        break;
                }
            }


            Slider_red.Value = (int)r;
            Slider_Blue.Value = (int)b;
            Slider_Green.Value = (int)g;
        }

        private void OnHSV(object sender, RoutedEventArgs e)
        {
            Picked = Color_Pick.HSV;
            ActiveRGB(false);
            ActiveCMYK(false);
            ActiveHSV(true);
            CMYKBOX.IsChecked = false;
            RGBBOX.IsChecked = false;
        }

        private void OnCMYK(object sender, RoutedEventArgs e)
        {
            Picked = Color_Pick.CMYK;
            ActiveRGB(false);
            ActiveCMYK(true);
            ActiveHSV(false);
            HSVBOX.IsChecked = false;
            RGBBOX.IsChecked = false;
        }

        private void OnRGB(object sender, RoutedEventArgs e)
        {
            Picked = Color_Pick.RGB;
            ActiveRGB(true);
            ActiveCMYK(false);
            ActiveHSV(false);
            HSVBOX.IsChecked = false;
            CMYKBOX.IsChecked = false;
        }

        private void ActiveRGB(bool act)
        {
            Slider_red.IsEnabled = act;
            Slider_Green.IsEnabled = act;
            Slider_Blue.IsEnabled = act;
        } 
        private void ActiveCMYK(bool act)
        {
            Slider_cyan.IsEnabled = act;
            Slider_Magenta.IsEnabled = act;
            Slider_Yellow.IsEnabled = act;
            Slider_Black.IsEnabled = act;
        }       
        private void ActiveHSV(bool act)
        {
            Slider_Hue.IsEnabled = act;
            Slider_Saturation.IsEnabled = act;
            Slider_Value.IsEnabled = act;
        }
    }

    public enum Color_Pick
    {
        RGB,
        CMYK,
        HSV

    }


    }
