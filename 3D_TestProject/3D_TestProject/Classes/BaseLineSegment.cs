using _3D_TestProject.Common;
using System;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;

namespace _3D_TestProject.Classes
{
    /// <summary>
    /// Класс описания отрезка
    /// </summary>
    [Serializable]
    public class BaseLineSegment : BaseObject
    {   
        /// <summary>
        /// Номер строки
        /// </summary>
        [XmlAttribute]
        public int Id { get; set; }
        /// <summary>
        /// Длина отрезка
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        /// Направление отрезка (ось)
        /// </summary>
        public Vector3D Vector { get; set; }

        /// <summary>
        /// Базовый конструктор для сериализации
        /// </summary>
        public BaseLineSegment() 
        { }

        /// <summary>
        /// Определение графического объекта
        /// </summary>
        /// <param name="modelGroup"></param>
        /// <param name="onPlane"></param>
        public override void DefineTo(Model3DGroup modelGroup, bool onPlane = false, EmissiveMaterial material = null)
        {
            var i = onPlane ? 2 : 1;
            var materialGroup = new MaterialGroup();
            materialGroup.Children.Add(StaticCommon.Black);
            materialGroup.Children.Add(onPlane ? StaticCommon.Red : material ?? StaticCommon.Blue);

            var geometryModel = new GeometryModel3D()
            {
                Material = materialGroup 
            };
            var geometry = new MeshGeometry3D();
            if (Vector.X == 1)
            {
                DefineX(StartPoint.X);
                DefineX(StartPoint.X + Length);
                void DefineX(double x)
                {
                    geometry.Positions.Add(new Point3D(x, StartPoint.Y + i, StartPoint.Z + i));
                    geometry.Positions.Add(new Point3D(x, StartPoint.Y + i, StartPoint.Z - i));
                    geometry.Positions.Add(new Point3D(x, StartPoint.Y - i, StartPoint.Z - i));
                    geometry.Positions.Add(new Point3D(x, StartPoint.Y - i, StartPoint.Z + i));
                }
            }
            else if (Vector.Y == 1)
            {
                DefineY(StartPoint.Y);
                DefineY(StartPoint.Y + Length);
                void DefineY(double y)
                {
                    geometry.Positions.Add(new Point3D(StartPoint.X + i, y, StartPoint.Z + i));
                    geometry.Positions.Add(new Point3D(StartPoint.X - i, y, StartPoint.Z + i));
                    geometry.Positions.Add(new Point3D(StartPoint.X - i, y, StartPoint.Z - i));
                    geometry.Positions.Add(new Point3D(StartPoint.X + i, y, StartPoint.Z - i));
                }
            }
           else if (Vector.Z == 1)
            { 
                    DefineZ(StartPoint.Z);
                    DefineZ(StartPoint.Z + Length);
                    void DefineZ(double z)
                    {
                        geometry.Positions.Add(new Point3D(StartPoint.X + i, StartPoint.Y + i, z));
                        geometry.Positions.Add(new Point3D(StartPoint.X + i, StartPoint.Y - i, z));
                        geometry.Positions.Add(new Point3D(StartPoint.X - i, StartPoint.Y - i, z));
                        geometry.Positions.Add(new Point3D(StartPoint.X - i, StartPoint.Y + i, z));
                    }
            }
            geometry.TriangleIndices = StaticCommon.LinesTriangleIndices;
            geometryModel.Geometry = geometry;
            modelGroup.Children.Add(geometryModel);
        }

        public BaseLineSegment(long x, long y, long z, Vector3D vector, double length)
        {
            StartPoint = new Point3D(x, y, z);
            Vector = vector;
            Length = length;
        }
    }
}
