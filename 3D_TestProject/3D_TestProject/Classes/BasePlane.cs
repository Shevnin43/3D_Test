using _3D_TestProject.Common;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3D_TestProject.Classes
{
    [Serializable]
    public class BasePlane : BaseObject
    {
        /// <summary>
        /// Направление плоскости (нормаль)
        /// </summary>
        public Vector3D Vector { get; set; }

        /// <summary>
        /// Максимальная координата для отрисовки плоскости
        /// </summary>
        public double MaxCoord { get; set; }

        /// <summary>
        /// Базовый конструктор для сериализации
        /// </summary>
        public BasePlane()
        { }

        /// <summary>
        /// Конструктор для создания плоскости через отрезок
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="vector"></param>
        public BasePlane(Point3D point, Vector3D vector)
        {
            StartPoint = point;
            Vector = vector;
        }

        /// <summary>
        /// Определение графического объекта
        /// </summary>
        /// <param name="modelGroup"></param>
        /// <param name="onPlane"></param>
        public override void DefineTo(Model3DGroup modelGroup, bool onPlane = false, EmissiveMaterial material = null)
        {
            var materialGroup = new MaterialGroup();
            materialGroup.Children.Add(StaticCommon.Black);
            materialGroup.Children.Add(StaticCommon.LightBlue);
            materialGroup.Children.Add(new SpecularMaterial(new SolidColorBrush(Colors.White), 50));
            var geometryModel = new GeometryModel3D()
            { 
                Material = materialGroup,
                BackMaterial = new DiffuseMaterial(Brushes.Black)
            };
            var geometry = new MeshGeometry3D();

            if (Vector.X == 1)
            {
                DefinePX(StartPoint.X - 1);
                DefinePX(StartPoint.X + 1);
                void DefinePX(double x)
                {
                    geometry.Positions.Add(new Point3D(x, MaxCoord, MaxCoord));
                    geometry.Positions.Add(new Point3D(x, MaxCoord, -MaxCoord));
                    geometry.Positions.Add(new Point3D(x, -MaxCoord, -MaxCoord));
                    geometry.Positions.Add(new Point3D(x, -MaxCoord, MaxCoord));
                }
            }
            else if (Vector.Y == 1)
            {
                DefinePY(StartPoint.Y - 1);
                DefinePY(StartPoint.Y + 1);
                void DefinePY(double y)
                {
                    geometry.Positions.Add(new Point3D(MaxCoord, y, MaxCoord));
                    geometry.Positions.Add(new Point3D(-MaxCoord, y, MaxCoord));
                    geometry.Positions.Add(new Point3D(-MaxCoord, y, -MaxCoord));
                    geometry.Positions.Add(new Point3D(MaxCoord, y, -MaxCoord));
                }
            }
            else if(Vector.Z == 1)
            { 
                    DefinePZ(StartPoint.Z - 1);
                    DefinePZ(StartPoint.Z + 1);
                    void DefinePZ(double z)
                    {
                        geometry.Positions.Add(new Point3D(MaxCoord, MaxCoord, z));
                        geometry.Positions.Add(new Point3D(MaxCoord, -MaxCoord, z));
                        geometry.Positions.Add(new Point3D(-MaxCoord, -MaxCoord, z));
                        geometry.Positions.Add(new Point3D(-MaxCoord, MaxCoord, z));
                    }
            }
            geometry.TriangleIndices = StaticCommon.LinesTriangleIndices;
            geometryModel.Geometry = geometry;
            modelGroup.Children.Add(geometryModel);
        }
    }
}
