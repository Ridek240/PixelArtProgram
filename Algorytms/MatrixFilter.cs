using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Algorytms
{
    class MatrixFilter : AlgorytmBase
    {
        public Bitmap Function(Bitmap bitmap, float[,] matrix)
        {
            //System.Drawing.Imaging.BitmapData data = null;
            //byte[] bitmapDataIn = LockBitmap24(bitmap, ref data);
            //byte[] bitmapDataout = new byte[data.Stride * data.Height];

            //Marshal.Copy(data.Scan0, bitmapDataout, 0, bitmapDataout.Length);

            //int dy = data.Height, dx = data.Stride / 3;

            int wX = ((int)matrix.GetLongLength(0) - 1) / 2;
            int wY = ((int)matrix.GetLongLength(1) - 1) / 2;

            float mL = 0;
            for (int i = 0; i < matrix.GetLongLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLongLength(1); j++)
                {
                    mL += matrix[i, j];
                }
            }
            if (mL == 0) mL = 1;
            //for (int j = wY; j < dy - wY - 1; j++)
            //{
            //    for (int i = wX; i < dx - wX - 1; i++)
            //    {
            //        float r = 0;
            //        float g = 0;
            //        float b = 0;
            //        for (int x = -wX; x <= wX; x++)
            //        {
            //            for (int y = -wY; y <= wY; y++)
            //            {
            //                int index = (i + x) * 3 + (j + y) * data.Stride;
            //                int matrixIndexX = x + wX;
            //                int matrixIndexY = y + wY;
            //                r += bitmapDataIn[index] * matrix[matrixIndexX, matrixIndexY];
            //                g += bitmapDataIn[index + 1] * matrix[matrixIndexX, matrixIndexY];
            //                b += bitmapDataIn[index + 2] * matrix[matrixIndexX, matrixIndexY];
            //            }
            //        }

            //        bitmapDataout[(i) * 3 + j * data.Stride] = (byte)(r / mL);
            //        bitmapDataout[(i) * 3 + j * data.Stride + 1] = (byte)(g / mL);
            //        bitmapDataout[(i) * 3 + j * data.Stride + 2] = (byte)(b / mL);

            //        //bitmapDataout[(i + j * data.Stride) * 3] = bitmapDataIn[(i + j * data.Stride) * 3];
            //        //bitmapDataout[(i + j * data.Stride) * 3 + 1] = bitmapDataIn[(i + j * data.Stride) * 3 + 1];
            //        //bitmapDataout[(i + j * data.Stride) * 3 + 2] = bitmapDataIn[(i + j * data.Stride) * 3 + 2];
            //    }
            //}

            //Marshal.Copy(bitmapDataout, 0, data.Scan0, bitmapDataout.Length);
            //bitmap.UnlockBits(data);

            Bitmap output = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            for (int x = wX; x < bitmap.Width - wX - 1; x++)
            {
                for (int y = wY; y < bitmap.Height - wY - 1; y++)
                {
                    float r = 0;
                    float g = 0;
                    float b = 0;
                    for (int xi = -wX; xi <= wX; xi++)
                    {
                        for (int yi = -wY; yi <= wY; yi++)
                        {
                            int matrixIndexX = xi + wX;
                            int matrixIndexY = yi + wY;
                            Color c = bitmap.GetPixel(x + xi, y + yi);
                            r += c.R * matrix[matrixIndexX, matrixIndexY];
                            g += c.G * matrix[matrixIndexX, matrixIndexY];
                            b += c.B * matrix[matrixIndexX, matrixIndexY];
                        }
                    }

                    output.SetPixel(x, y, Color.FromArgb(255, (byte)(r / mL), (byte)(g / mL), (byte)(b / mL)));
                }
            }

            return output;
        }

        private Bitmap RemoveColorPixels(Bitmap bitmap)
        {
            Bitmap output = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int x = 0; x < bitmap.Height; x++)
                {
                    if(bitmap.GetPixel(i, x) == Color.FromArgb(255, 255,255,255))
                    {
                        output.SetPixel(i, x, bitmap.GetPixel(i, x));
                    }
                    else
                    {
                        output.SetPixel(i, x, Color.FromArgb(255, 0,0,0));
                    }
                }
            }

            return output;
        }
    }
}
