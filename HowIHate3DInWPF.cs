using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PixelArtProgram
{
    public class HowIHate3DInWPF
    {
        public static Point3D[] points = new Point3D[]
        {
            new Point3D(-1, -1, -1), new Point3D(1, -1, -1),
            new Point3D(-1,  1, -1), new Point3D(1,  1, -1),
            new Point3D(-1, -1,  1), new Point3D(1, -1,  1),
            new Point3D(-1,  1,  1), new Point3D(1,  1,  1)
        };

        public static int[] triangles = new int[]
        {
            0,2,1, 1,2,3, 4,6,0, 2,0,6,
            0,1,4, 1,5,4, 1,7,5, 1,3,7,
            4,5,6, 7,6,5, 2,6,3, 3,6,7
        };
        public static MeshGeometry3D GetMesh(Point3D position)
        {
            MeshGeometry3D m = new MeshGeometry3D();
            Point3D[] p = new Point3D[points.Length];
            for (int i = 0; i < p.Length; i++)
            {
                double x = points[i].X + position.X - 10;
                double y = points[i].Y + position.Y - 10;
                double z = points[i].Z + position.Z - 10;
                p[i] = new Point3D(-y, z, x);
            }
            m.Positions = new Point3DCollection(p);
            m.TriangleIndices = new Int32Collection(triangles);

            return m;
        }

        public static GeometryModel3D CreateCube(System.Drawing.Color color, Point3D position)
        {
            GeometryModel3D model = new GeometryModel3D(
                GetMesh(position), 
                new DiffuseMaterial(
                    new SolidColorBrush(
                        System.Windows.Media.Color.FromArgb(
                            color.A, 
                            color.R, 
                            color.G, 
                            color.B))));

            return model;
        }

        public static void EndThisWorld(Model3DGroup group, Bitmap bitmap, float z)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    System.Drawing.Color color = bitmap.GetPixel(x, y);
                    if (color == System.Drawing.Color.FromArgb(0, 0, 0, 0)) continue;
                    GeometryModel3D model3D = CreateCube(color, new Point3D(x * 2, y * 2, z * 2));
                    group.Children.Add(model3D);
                }
            }
        }
    }
}
