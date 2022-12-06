using Accord.Collections;
using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace PixelArtProgram.Algorytms
{
    internal class ShowHighestDensity : AlgorytmBase
    {
        public Bitmap Function(Bitmap bitmap, byte[] val) => Function(bitmap, val, val);
        public Bitmap Function(Bitmap bitmap, byte[] minVal, byte[] maxVal)
        {
            System.Drawing.Imaging.BitmapData data = null;
            byte[] bitmapData = LockBitmap32(bitmap, ref data);

            // Thresholding
            for (int i = 0; i < bitmapData.Length; i += 4)
            {
                if (bitmapData[i + 0] >= minVal[0] && bitmapData[i + 0] <= maxVal[0] &&
                    bitmapData[i + 1] >= minVal[1] && bitmapData[i + 1] <= maxVal[1] &&
                    bitmapData[i + 2] >= minVal[2] && bitmapData[i + 2] <= maxVal[2])
                {
                    bitmapData[i + 0] = byte.MaxValue;
                    bitmapData[i + 1] = byte.MaxValue;
                    bitmapData[i + 2] = byte.MaxValue;
                }
                else
                {
                    bitmapData[i + 0] = byte.MinValue;
                    bitmapData[i + 1] = byte.MinValue;
                    bitmapData[i + 2] = byte.MinValue;
                }
            }

             // Group pixels
             List<List<int>> groupsOfPoints = new List<List<int>>();
             bool[] isVisited = new bool[bitmapData.Length / 4];
            
             for (int i = 0; i < bitmapData.Length / 4; i++)
             {
                 if (isVisited[i]) continue;
                 isVisited[i] = true;
                 if (bitmapData[i * 4] != byte.MaxValue) continue;
            
                 List<int> points = new List<int>();
                 groupsOfPoints.Add(points);
                 points.Add(i);

                List<int> pointsToCheck = new List<int>();

                if (i % bitmap.Width == (i + 1) % bitmap.Width)
                    pointsToCheck.Add(i + 1);
                if (i < bitmapData.Length - bitmap.Width)
                    pointsToCheck.Add(i + bitmap.Width);

                while (pointsToCheck.Count > 0)
                 {
                     int index = pointsToCheck[0];
                     pointsToCheck.RemoveAt(0);
                     if (isVisited[index]) continue;
                     isVisited[index] = true;
                     if (bitmapData[index * 4] != byte.MaxValue) continue;
            
                     points.Add(index);
            
                     if (index % bitmap.Width == (index - 1) % bitmap.Width)
                         pointsToCheck.Add(index - 1);
                     if (index % bitmap.Width == (index + 1) % bitmap.Width)
                         pointsToCheck.Add(index + 1);
                     if (index > bitmap.Width)
                         pointsToCheck.Add(index - bitmap.Width);
                     if (index < bitmapData.Length - bitmap.Width)
                         pointsToCheck.Add(index + bitmap.Width);
                 }
             }
            
             // Find biggest group
             int max = 0;
             int indexOfMaxGroup = 0;
             for (int i = 0; i < groupsOfPoints.Count; i++)
             {
                 if (groupsOfPoints[i].Count > max)
                 {
                     max = groupsOfPoints[i].Count;
                     indexOfMaxGroup = i;
                 }
             }
            
             // Remove smaller groups
             for (int i = 0; i < groupsOfPoints.Count; i++)
             {
                 if (i == indexOfMaxGroup) continue;
                 for (int j = 0; j < groupsOfPoints[i].Count; j++)
                 {
                     int index = groupsOfPoints[i][j] * 4;
                     bitmapData[index + 0] = byte.MinValue;
                     bitmapData[index + 1] = byte.MinValue;
                     bitmapData[index + 2] = byte.MinValue;
                 }
             }

            Marshal.Copy(bitmapData, 0, data.Scan0, bitmapData.Length);
            bitmap.UnlockBits(data);

            return bitmap;
        }
    }
}
