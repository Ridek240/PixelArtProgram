using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Algorytms
{
    class PrecentTreshhold : BinaryTreshhold
    {
        public Bitmap Function(Bitmap bitmap, float procent)
        {

            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapData = LockBitmap32(bitmap, ref data);
            int[] histogramValues = GetHistogram(bitmapData);
            int total = data.Height * data.Width;
            int sumBefore = 0;
            int threshold = 0;
            
            for (int i = 0; i < 256; i++)
            {
                sumBefore = sumBefore + histogramValues[i];
                if(sumBefore >= total * procent)
                {
                    threshold = i;
                    break;
                }
            }


            bitmap.UnlockBits(data);
            return BinaryThreshold(bitmap, (byte)threshold, true);
        }
    }
}
