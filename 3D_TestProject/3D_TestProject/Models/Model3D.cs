using _3D_TestProject.Classes;
using _3D_TestProject.Common;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace _3D_TestProject.Models
{
    public class Model3D : AbstractModel
    {
        /// <summary>
        /// Радиус сферы по которой бегает камера
        /// </summary>
        private double CamRadius { get => Plane == null ? 0 : new Vector3D(Plane.MaxCoord, Plane.MaxCoord, Plane.MaxCoord).Length; }

        /// <summary>
        /// Углы для трансформации
        /// </summary>
        private Vector3D Angle { get; set; } = new Vector3D();
        /*
        private float AngleX { get; set; }
        private float AngleY { get; set; }
        private float AngleZ { get; set; }
        */
        private bool JastElementsDefine { get; set; } = false;

        /// <summary>
        /// Графическая модель для системы координат (это чтоб ее пометить отдельно чтоб она не вращалась при трансформации модели)
        /// </summary>
        private Model3DGroup _coordGroup = new Model3DGroup();
        public Model3DGroup CoordGroup
        {
            get => _coordGroup;
            set
            {
                _coordGroup = value;
                Notify();
            }
        }

        /// <summary>
        /// Считать отрезки из XML файла
        /// </summary>
        public void GetXMLLines(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var fileName = StaticCommon.OpenXML();
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return;
                }
                var formatter = new XmlSerializer(typeof(BaseLineSegment[]));
                using (var fs = new FileStream(fileName, FileMode.Open))
                {
                    AllLineSegments = (BaseLineSegment[])formatter.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка при открытии файлов с отрезками. Возможно некорректно указаны данные. Ошибка: {ex.Message}");
                return;
            }
        }

        /// <summary>
        /// Считать плоскость сечения из XML файла
        /// </summary>
        public void GetXMLPlane(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var fileName = StaticCommon.OpenXML();
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return;
                }
                var element = XElement.Load(fileName);
                
                if (element.XPathSelectElement("//X") != null && element.XPathSelectElement("//Y") != null && element.XPathSelectElement("//Z") != null)
                {
                    var formatter = new XmlSerializer(typeof(BasePlane));
                    using (var fs = new FileStream(fileName, FileMode.Open))
                    {
                        Plane = (BasePlane)formatter.Deserialize(fs);
                    }
                }
                else if (element.XPathSelectElement("//LineNumber") != null)
                {
                    var correctLineNumber = int.TryParse(element.XPathSelectElement("//LineNumber")?.Value, out var linenumber);
                    var planeStartLine = AllLineSegments?.FirstOrDefault(x => x.Id == linenumber);
                    var correctVector = Enum.TryParse<Vector3D>(element.XPathSelectElement("//Vector")?.Value, out var vector) && planeStartLine?.Vector != vector;
                    if (AllLineSegments == null || !AllLineSegments.Any())
                    {
                        MessageBox.Show($"В выбранном файле указаны параметры для загрузки плоскости по определяющему отрезку. На данный момент не загружено ни одного отрезка.");
                    }
                    else if (planeStartLine == null)
                    {
                        MessageBox.Show($"В выбранном файле указаны параметры для загрузки плоскости по определяющему отрезку. Среди загруженных ранее отрезков отрезок с Id={linenumber} не обнаружен.");
                    }
                    else if (!correctVector)
                    {
                        MessageBox.Show($"В выбранном файле указаны параметры для загрузки плоскости по определяющему отрезку. Нормаль плоскости задана не корректно, либо совпадает с вектором определяющего отрезка.");
                    }
                    else if (!correctLineNumber)
                    {
                        MessageBox.Show($"В выбранном файле указаны параметры для загрузки плоскости по определяющему отрезку. Номер определяющего отрезка не указан, либо указан не корректно.");
                    }
                    else if (correctLineNumber && correctVector)
                    {
                        Plane = new BasePlane(planeStartLine.StartPoint, vector);
                    }
                    else 
                    {
                        MessageBox.Show($"Хм ... непонятно почему но плоскость не загружена.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка при открытии файла плоскости. Возможно некорректно указаны данные. Ошибка: {ex.Message}");
                return;
            }
        }

        protected override void DefineTransform() => DefineTransform(this, null);

        /// <summary>
        /// Изменение позиции камеры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DefineTransform(object sender, ExecutedRoutedEventArgs e)
        {
            // определяем направление движения камеры
            var options = e?.Parameter.ToString().Split(';');
            // если нажата "Вернуть камеру"
            if (options == null || options[0] =="0")
            {
                Angle = new Vector3D();
            }
            // иначе, если задано корректное направление движения камеры
            else if (e != null && options.Length >= 2 && float.TryParse(options[1], out var step))
            {
                const int stepAngle = 5;
                var angleVactor = Vector3D.Parse(options[0]);
                var deltaAngle = Vector3D.Multiply(angleVactor, stepAngle*step);
                Angle = Vector3D.Add(Angle, deltaAngle);
            }
            var transformGroup = new Transform3DGroup();
            var rotationX = new RotateTransform3D()
            { Rotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), Angle.X) };
            var rotationY = new RotateTransform3D()
            { Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), Angle.Y) };
            var rotationZ = new RotateTransform3D()
            { Rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), Angle.Z) };
            transformGroup.Children.Add(rotationX);
            transformGroup.Children.Add(rotationY);
            transformGroup.Children.Add(rotationZ);
            Group.Transform = transformGroup;
        }

        /// <summary>
        /// Определение графики камеры для 3D
        /// </summary>
        protected override void DefineCamera() => DefineCamera(new Point3D(Plane.MaxCoord, Plane.MaxCoord, Plane.MaxCoord));

        private void DefineCamera(Point3D point)
        {
            if (JastElementsDefine)
            {
                return;
            }
            Camera.Position = point;
            Camera.LookDirection = new Vector3D(-Camera.Position.X, -Camera.Position.Y, -Camera.Position.Z);
            Camera.Width = CamRadius * 2;
            Camera.NearPlaneDistance = 0;
        }

        protected override void DefineGraphixObects()
        {
            Group?.Children?.Clear();
            foreach (var line in LinesOnPlane)
            {
                line.DefineTo(Group, true);
            }
            foreach (var line in LinesFree)
            {
                line.DefineTo(Group, false);
            }
            foreach (var point in CrossPoints)
            {
                point.DefineTo(Group, true);
            }
            Plane.DefineTo(Group);
        }

        protected override void DefineAdditionalData()
        {
            if (!JastElementsDefine)
            {
                DefineCoordSys();
            }
        }

        /// <summary>
        /// Метод определения системы координат
        /// </summary>
        private void DefineCoordSys()
        {
            CoordGroup.Children.Clear();
            foreach (var vector in StaticCommon.AxisColors)
            {
                var axis = new BaseLineSegment(0, 0, 0, vector.Key, Plane.MaxCoord);
                var axisBrush = StaticCommon.AxisColors[vector.Key];
                axis.DefineTo(CoordGroup, false, axisBrush);
            }
            foreach (var arrow in StaticCommon.ArrowsDict)
            {
                var materialGroup = new MaterialGroup();
                materialGroup.Children.Add(StaticCommon.Black);
                materialGroup.Children.Add(arrow.Value);
                CoordGroup.Children.Add(new GeometryModel3D()
                {
                    BackMaterial = StaticCommon.Yellow,
                    Geometry = DefineArrow(arrow.Key),
                    Material = materialGroup
                });
            }
        }

        /// <summary>
        /// Определение стрелочек системы координат
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private MeshGeometry3D DefineArrow(string element)
        {
            var geometry = new MeshGeometry3D();
            foreach (var pointElem in element.Replace("A", Plane.MaxCoord.ToString()).Replace("B", (Plane.MaxCoord + 20).ToString()).Split(' '))
            {
                var coords = pointElem.Split(',');
                if (long.TryParse(coords[0], out var x) && long.TryParse(coords[1], out var y) && long.TryParse(coords[2], out var z))
                {
                    geometry.Positions.Add(new Point3D(x, y, z));
                }
            }
            geometry.TriangleIndices = StaticCommon.ArrowTriangleIndices;
            return geometry;
        }

        /// <summary>
        /// Метод определения движения камеры по сфере координат
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MoveCamera(object sender, ExecutedRoutedEventArgs e)
        {
            const int percentage = 25;
            var sqrt2 = Math.Sqrt(2);
            var options = e?.Parameter.ToString().Split(',');
            if (e != null && options.Length >= 2 && int.TryParse(options[1], out var step))
            {
                JastElementsDefine = true;
                double coordX, coordY, coordZ;
                switch (options[0])
                {
                    case "Vertical":
                        var stepY = (CamRadius / percentage) * step;
                        var projOld = Projection(Camera.Position.Y);
                        coordY = Math.Abs(Camera.Position.Y + stepY) >= CamRadius ? (CamRadius-0.000001) * step : Camera.Position.Y + stepY;
                        var projNew = Projection(coordY);
                        var k = projNew / projOld;
                        coordX = Camera.Position.X * k;
                        coordZ = Camera.Position.Z * k;
                        double Projection(double h) => (Math.Sqrt(CamRadius * CamRadius - h * h));
                        break;
                    case "Horizontal":
                        var radius = Math.Sqrt(CamRadius * CamRadius - Camera.Position.Y * Camera.Position.Y);
                        var moveX = Math.Abs(Camera.Position.X) <= radius / sqrt2;
                        var revert = (moveX && Camera.Position.Z < 0) || (!moveX && Camera.Position.X > 0) ? -1 : 1;
                        coordY = Camera.Position.Y;
                        double kO;
                        if (moveX)
                        {
                            kO = Math.Round(Camera.Position.Z / Math.Abs(Camera.Position.Z));
                            coordX = Camera.Position.X + (radius / percentage) * step * revert;
                            coordZ = Math.Sqrt(radius * radius - coordX * coordX)*kO;
                        }
                        else
                        {
                            kO = Math.Round(Camera.Position.X / Math.Abs(Camera.Position.X));
                            coordZ = Camera.Position.Z + (radius / percentage) * step * revert;
                            coordX = Math.Sqrt(radius * radius - coordZ * coordZ)* kO;
                        }
                        break;
                    default:
                        JastElementsDefine = false;
                        DefineCamera();
                        return;
                }
                Camera.Position = new Point3D(coordX, coordY, coordZ);
                Camera.LookDirection = new Vector3D(-Camera.Position.X, -Camera.Position.Y, -Camera.Position.Z);
                Camera.NearPlaneDistance = 0;
                Camera.Width = CamRadius * 2;
            }
        }

        /// <summary>
        /// Метод передвижения плоскости вдоль оси нормали
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MovePlane(object sender, ExecutedRoutedEventArgs e)
        {
            if (!int.TryParse(e.Parameter.ToString(), out var step))
            {
                return;
            }
            step *= 10;
            JastElementsDefine = true;
            Plane = new BasePlane()
            {
                StartPoint = Vector3D.Add(Vector3D.Multiply(Plane.Vector, step), Plane.StartPoint),
                Vector = Plane.Vector,
                MaxCoord = Plane.MaxCoord
            };
        }
    }
}
