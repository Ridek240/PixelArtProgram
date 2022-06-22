using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Algorytms
{
    public class RevertColors
    {
        public Bitmap Function(Bitmap bitmap)
        {
            for(int x = 0; x < bitmap.Height; x++)
            {
                for(int y = 0; y < bitmap.Width; y++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    if (pixel.A == 0) continue;
                    Color reverse = Color.FromArgb(255, 255 - pixel.R, 255 - pixel.G, 255 - pixel.B);
                    bitmap.SetPixel(x, y, reverse);
                }
            }
            return bitmap;
        }
    }
}
