﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelArtProgram.Algorytms
{
    public class AlgorytmBase
    {

        protected byte[] LockBitmap(Bitmap bitmap, ref System.Drawing.Imaging.BitmapData data)
        {
            data = bitmap.LockBits(
                new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), bitmap.Size),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var bitmapData = new byte[data.Stride * data.Height];

            Marshal.Copy(data.Scan0, bitmapData, 0, bitmapData.Length);

            return bitmapData;
        }


        protected int[] GetHistogram(byte[] bitmapData)
        {
            int[] histogramValues = new int[byte.MaxValue + 1];

            {
                for (int i = 0; i < bitmapData.Length; i += 3)
                {
                    int value = (bitmapData[i] + bitmapData[i + 1] + bitmapData[i + 2]) / 3;
                    histogramValues[value]++;
                }
            }
            return histogramValues;
        }
    }
}
