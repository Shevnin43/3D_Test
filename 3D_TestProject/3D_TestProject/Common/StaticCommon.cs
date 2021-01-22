using _3D_TestProject.Classes;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3D_TestProject.Common
{
    public static class StaticCommon
    {
        /// <summary>
        /// Обход точек осей/отрезков
        /// </summary>
        public static readonly Int32Collection LinesTriangleIndices = new Int32Collection() { 0, 1, 2, 0, 2, 3, 0, 4, 5, 0, 5, 1, 1, 5, 6, 1, 6, 2, 2, 6, 7, 2, 7, 3, 3, 7, 4, 3, 4, 0, 4, 7, 6, 4, 6, 5 };

        /// <summary>
        /// Обход точек стрелочек
        /// </summary>
        public static readonly Int32Collection ArrowTriangleIndices = new Int32Collection() { 1, 0, 4, 4, 0, 3, 3, 0, 2, 2, 0, 1, 1, 4, 2, 4, 3, 2 };

        /// <summary>
        /// Цвета для материалов
        /// </summary>
        public static EmissiveMaterial Red = new EmissiveMaterial(Brushes.Red);
        public static EmissiveMaterial Blue = new EmissiveMaterial(Brushes.Blue);
        public static EmissiveMaterial Yellow = new EmissiveMaterial(Brushes.Yellow);
        public static EmissiveMaterial LightPink = new EmissiveMaterial(Brushes.LightPink);
        public static EmissiveMaterial LightBlue = new EmissiveMaterial(Brushes.LightBlue);
        public static EmissiveMaterial LightGreen = new EmissiveMaterial(Brushes.LightGreen);

        public static DiffuseMaterial Black = new DiffuseMaterial(Brushes.Black);

        /// <summary>
        /// Цвета для осей
        /// </summary>
        public static Dictionary<Vector3D, EmissiveMaterial> AxisColors = new Dictionary<Vector3D, EmissiveMaterial>
        {
            {new Vector3D(1,0,0), LightPink },
            {new Vector3D(0,1,0), LightGreen },
            {new Vector3D(0,0,1), Yellow }
        };

        /// <summary>
        /// Словарь стрелочек системы координат
        /// </summary>
        public static readonly Dictionary<string, EmissiveMaterial> ArrowsDict = new Dictionary<string, EmissiveMaterial>
        {
            {"B,0,0 A,3,3 A,-3,3 A,-3,-3 A,3,-3", LightPink },
            {"0,B,0 3,A,3 3,A,-3 -3,A,-3 -3,A,3", LightGreen },
            {"0,0,B 3,3,A -3,3,A -3,-3,A 3,-3,A", Yellow }
        };

        /// <summary>
        /// Диалог открытия XML файла
        /// </summary>
        /// <returns></returns>
        public static string OpenXML()
        {
            var dlg = new OpenFileDialog()
            {
                DefaultExt = "xml",
                Filter = "Файлы XML|*.xml",
                Multiselect = false
            };
            if (dlg.ShowDialog() == true)
            {
                return dlg.FileName;
            }
            return null;
        }

        public static void DefineTo(this Point3D point, Model3DGroup modelGroup, bool onPlane = false)
        {
            var i = onPlane ? 4 : 1;
            var materialGroup = new MaterialGroup();
            materialGroup.Children.Add(StaticCommon.Black);
            materialGroup.Children.Add(onPlane ? StaticCommon.Red : StaticCommon.Blue);
            var geometryModel = new GeometryModel3D()
            { Material = materialGroup };
            var geometry = new MeshGeometry3D();
            DefinePoint(point.X - i);
            DefinePoint(point.X + i);
            void DefinePoint(double x)
            {
                geometry.Positions.Add(new Point3D(x, point.Y + i, point.Z + i));
                geometry.Positions.Add(new Point3D(x, point.Y + i, point.Z - i));
                geometry.Positions.Add(new Point3D(x, point.Y - i, point.Z - i));
                geometry.Positions.Add(new Point3D(x, point.Y - i, point.Z + i));
            }
            geometry.TriangleIndices = StaticCommon.LinesTriangleIndices;
            geometryModel.Geometry = geometry;
            modelGroup.Children.Add(geometryModel);
        }
    }
}
