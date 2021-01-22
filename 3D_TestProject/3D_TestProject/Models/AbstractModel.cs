using _3D_TestProject.Classes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;

namespace _3D_TestProject.Models
{
    /// <summary>
    /// Абстрактный класс модели (3Д или 2Д). Логика примерно следующая:
    /// "Потомок! Дай мне отрезки и плоскость я выдам тебе готовый графический объект, а камеру и трансформации определяй сам!"
    /// </summary>
    public abstract class AbstractModel : INotifyPropertyChanged
    {
        public delegate void EventHandler(object sender, ChangeDataEvent args);
        public event EventHandler DataChanged = delegate { };

        /// <summary>
        /// Реализация INotifyPropertyChanged уведомление об изменении
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void Notify([CallerMemberName] string property = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        /// <summary>
        /// Флаг возможности опрелделения фигур (только после того как заданы отрезки и плоскость)
        /// </summary>
        private bool CanDefineCrossObjects { get => AllLineSegments?.Any() == true && Plane != null; }

        /// <summary>
        /// Отрезки в плоскости
        /// </summary>
        protected ICollection<BaseLineSegment> LinesOnPlane { get; set; } = new List<BaseLineSegment>();

        /// <summary>
        /// Отрезки  ане плоскости
        /// </summary>
        protected ICollection<BaseLineSegment> LinesFree { get; set; } = new List<BaseLineSegment>();

        /// <summary>
        /// Точки пересечения отрезков и плоскости
        /// </summary>
        protected ICollection<Point3D> CrossPoints { get; set; } = new List<Point3D>();

        /// <summary>
        /// Все заданные отрезки 
        /// </summary>
        private ICollection<BaseLineSegment> _allLineSegments;
        public ICollection<BaseLineSegment> AllLineSegments
        {
            get => _allLineSegments;
            set
            {
                _allLineSegments = value;
                if (CanDefineCrossObjects)
                {
                    DefineObjects();
                }
                DataChanged(this, new ChangeDataEvent(Plane, AllLineSegments));
                Notify();
            }
        }

        /// <summary>
        /// Плоскость
        /// </summary>
        private BasePlane _plane;
        public BasePlane Plane
        {
            get => _plane;
            set
            {
                _plane = value;
                if (CanDefineCrossObjects)
                {
                    DefineObjects();
                }
                DataChanged(this, new ChangeDataEvent(Plane, AllLineSegments));
                Notify();
            }
        }

        /// <summary>
        /// Графическая модель фигур в плоскости
        /// </summary>
        private Model3DGroup _group = new Model3DGroup();
        public Model3DGroup Group
        {
            get => _group;
            set
            {
                _group = value;
                Notify();
            }
        }

        /// <summary>
        /// Параметры  визуальной камеры (которую надо установить на плоскость)
        /// </summary>
        private OrthographicCamera _camera = new OrthographicCamera();
        public OrthographicCamera Camera
        {
            get => _camera;
            set
            {
                _camera = value;
                Notify();
            }
        }

        /// <summary>
        /// Метод определения графических объектов. 
        /// </summary>
        private void DefineObjects()
        {
            DefineMaxCoord();
            DefineElements();
            DefineCamera();
            DefineTransform();
            DefineGraphixObects();
            DefineAdditionalData();
        }

        /// <summary>
        /// Определяем максимальную координату на плоскости (потребуется для установки камеры и отрисовки плоскости)
        /// </summary>
        private void DefineMaxCoord()
        {
            var allPoints = new List<Point3D>();
            //возьмем точки начала и конца отрезков
            allPoints.AddRange(AllLineSegments.Select(l => l.StartPoint));
            allPoints.AddRange(AllLineSegments.Select(l => EndPoint(l)));
            //получаем точку конца отрезка
            Point3D EndPoint(BaseLineSegment line)
            {
                var vector = Vector3D.Multiply(line.Vector, line.Length);
                return Vector3D.Add(vector, line.StartPoint);
            }
            // возьмем вектор самой отдаленной точки (для этого вычислим расстояние от центра осей до точки) 
            Plane.MaxCoord = allPoints.Select(p => new Vector3D(p.X, p.Y, p.Z).Length).Max();
        }

        /// <summary>
        /// Определение элементов математическим методом
        /// </summary>
        private void DefineElements()
        {
            CrossPoints.Clear();
            LinesFree.Clear();
            LinesOnPlane.Clear();

            foreach (var line in AllLineSegments)
            {
                var CV = Vector3D.Multiply(line.Vector, line.Length);
                var CA = Point3D.Subtract(Plane.StartPoint, line.StartPoint);
                var CN = Vector3D.DotProduct(CA, Plane.Vector);
                var CM = Vector3D.DotProduct(CV, Plane.Vector);
                if (CN == 0 && CM == 0)
                {
                    LinesOnPlane.Add(line);
                    continue;
                }
                var k = CN / CM;
                LinesFree.Add(line);
                if (k <= 1 && k >= 0)
                {
                    var x = Vector3D.Multiply(CV, k);
                    CrossPoints.Add(Vector3D.Add(x, line.StartPoint));
                }
            }
        }

        protected abstract void DefineGraphixObects();
        protected abstract void DefineAdditionalData();
        protected abstract void DefineCamera();
        protected abstract void DefineTransform();
    }
}
