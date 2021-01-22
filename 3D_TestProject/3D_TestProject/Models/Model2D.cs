using _3D_TestProject.Common;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace _3D_TestProject.Models
{
    public class Model2D : AbstractModel
    {
        public ChangeDataEvent DataChangedEvent { get; }

        /// <summary>
        /// Отражение плоскости в окне
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Revert2D(object sender, ExecutedRoutedEventArgs e)
        {
            double angle = 0;
            if (Group.Transform is RotateTransform3D rotateTransform)
            {
                angle = (rotateTransform.Rotation as AxisAngleRotation3D).Angle;
            }
            AxisAngleRotation3D rotation;
            if (Plane.Vector.X == 0 && Plane.Vector.Z == 0)
            {
                rotation = new AxisAngleRotation3D(new Vector3D(1, 0.000001, 0), angle == 0 ? 180 : 0);
            }
            else 
            {
                rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), angle == 0 ? 180 : 0);
            }

            Group.Transform = new RotateTransform3D
            {
                Rotation = rotation
            };
        }

        protected override void DefineAdditionalData()
        { }

        /// <summary>
        /// Определение параметров камеры для отображения плоскости
        /// </summary>
        protected override void DefineCamera()
        {
            Camera.Position = Vector3D.Add( Vector3D.Multiply(Plane.Vector , Plane.MaxCoord), new Point3D(0,0,0));
            Camera.LookDirection = new Vector3D(-Camera.Position.X, -Camera.Position.Y, -Camera.Position.Z);
            Camera.Width = Plane.MaxCoord * 3;
        }

        protected override void DefineGraphixObects()
        {
            Group?.Children?.Clear();
            foreach (var line in LinesOnPlane)
            {
                line.DefineTo(Group, true);
            }
            foreach (var point in CrossPoints)
            {
                point.DefineTo(Group, true);
            }
            Plane.DefineTo(Group);
        }

        /// <summary>
        /// Нам не нужна трансформация двумерной графики. Обойдемся камерой
        /// </summary>
        protected override void DefineTransform()
        {}

    }
}
