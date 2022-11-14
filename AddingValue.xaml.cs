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
    /// Logika interakcji dla klasy AddingValue.xaml
    /// </summary>
    public partial class AddingValue : Window
    {
        public AddingValue()
        {
            InitializeComponent();
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
