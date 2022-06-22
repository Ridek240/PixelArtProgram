using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Algorytms
{
    public class BinaryTreshhold : AlgorytmBase
    {

        public Bitmap BinaryThreshold(Bitmap bitmap, byte threshold, bool average)
        {
            return BinaryThreshold(bitmap, threshold, false, false, false, average);
        }

        public Bitmap BinaryThreshold(Bitmap bitmap, byte threshold, bool red, bool green, bool blue, bool average)
        {
            var data = bitmap.LockBits(
                new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), bitmap.Size),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var bitmapData = new byte[data.Stride * data.Height];

            Marshal.Copy(data.Scan0, bitmapData, 0, bitmapData.Length);

            for (int i = 0; i < bitmapData.Length; i += 3)
            {
                byte b = blue || average ? bitmapData[i] : byte.MinValue;
                byte g = green || average ? bitmapData[i + 1] : byte.MinValue;
                byte r = red || average ? bitmapData[i + 2] : byte.MinValue;

                byte result = (byte)((r + g + b) / 3);
                if (average)
                {
                    bitmapData[i] = result > threshold ? byte.MaxValue : byte.MinValue;
                    bitmapData[i + 1] = result > threshold ? byte.MaxValue : byte.MinValue;
                    bitmapData[i + 2] = result > threshold ? byte.MaxValue : byte.MinValue;
                }
                else
                {
                    bitmapData[i] = b > threshold ? byte.MaxValue : byte.MinValue;
                    bitmapData[i + 1] = g > threshold ? byte.MaxValue : byte.MinValue;
                    bitmapData[i + 2] = r > threshold ? byte.MaxValue : byte.MinValue;
                }
            }

            Marshal.Copy(bitmapData, 0, data.Scan0, bitmapData.Length);

            bitmap.UnlockBits(data);

            return bitmap;
        }
    }
}
