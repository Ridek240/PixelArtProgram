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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PixelArtProgram.Grafic
{
    /// <summary>
    /// Logika interakcji dla klasy ListElement_.xaml
    /// </summary>
    public partial class ListElement : UserControl
    {
        public ListElement()
        {
            InitializeComponent();
        }

        public string Label
        {
            get { return LabelSus.Content.ToString();  }

            set { LabelSus.Content = value; }
        }

        public BitmapImage Image
        {
            set { Bitmapa.Source = value;  }
        }

        public bool IsVisibleFun
        {
            set { 
                if(value)
                {
                    IsVisible.Visibility = Visibility.Visible;
                }
                else
                {
                    IsVisible.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
