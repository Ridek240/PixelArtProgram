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
    /// Logika interakcji dla klasy AddMask.xaml
    /// </summary>
    public partial class AddMask : Window
    {
        public AddMask()
        {
            InitializeComponent();
        }
        private void Confirm(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public float[,] GetMask()
        {
            int masksize = 0;
                if (!int.TryParse(MaskSize.Text, out masksize)) return null;
            string StringMask = textblock.Text;

            string[] cut = StringMask.Split(null);

            float[,] output = new float[masksize, masksize];

            int i = 0;
            foreach(string ham in cut)
            {
                int x = i / masksize;
                int y = i % masksize;
                i++;
                float.TryParse(ham, out output[x, y]);


            }

            return output;
        }
    }
}
