using System.Windows.Media.Media3D;

namespace _3D_TestProject.Classes
{
    public abstract class BaseObject
    {
        /// <summary>
        /// Координаты точки
        /// </summary>
        public Point3D StartPoint { get; set; }

        /// <summary>
        /// Определение графического объекта
        /// </summary>
        /// <param name="modelGroup"></param>
        /// <param name="onPlane"></param>
        /// <param name="material"></param>
        public abstract void DefineTo(Model3DGroup modelGroup, bool onPlane = false, EmissiveMaterial material = null);
    }
}
