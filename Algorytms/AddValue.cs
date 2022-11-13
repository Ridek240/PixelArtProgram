﻿using System.Drawing;
using System.Runtime.InteropServices;

namespace PixelArtProgram.Algorytms
{
    class AddValue : AlgorytmBase
    {
        public Bitmap Function(Bitmap bitmap,int val, bool invidula, int r, int g, int b)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapDataIn = LockBitmap32(bitmap, ref data);



            int size = bitmapDataIn.Length;
            for(int i =0;i<size;i+=4)
            {

                if (bitmapDataIn[i + 3] == 255)
                {
                    bitmapDataIn[i] = invidula ? AddIncrement(bitmapDataIn[i], r) : AddIncrement(bitmapDataIn[i], val);
                    bitmapDataIn[i + 1] = invidula ? AddIncrement(bitmapDataIn[i + 1], g) : AddIncrement(bitmapDataIn[i + 1], val);
                    bitmapDataIn[i + 2] = invidula ? AddIncrement(bitmapDataIn[i + 2], b) : AddIncrement(bitmapDataIn[i + 2], val);
                }
            }

            Marshal.Copy(bitmapDataIn, 0, data.Scan0, bitmapDataIn.Length);
            bitmap.UnlockBits(data);
            return bitmap;
        }

        public byte AddIncrement(byte value, int increment)
        {
            int output = (int)value + increment;
            if(output>=255)
            {
                output = 255;
            }
            else if(output<=0)
            {
                output = 0;
            }

            return (byte)output;
        }
    }
}
