
using _3D_TestProject.Classes;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace _3D_TestProject.Interfaces
{
    public interface IModel
    {
        ICollection<BaseLineSegment> AllLineSegments { get; set; }
        BasePlane Plane { get; set; }
        bool CanDefineCrossObjects { get; }
        ICollection<BaseLineSegment> LinesOnPlane { get; set; }
        ICollection<BaseLineSegment> LinesFree { get; set; }
        ICollection<Point3D> CrossPoints { get; set; }
        Model3DGroup Group { get; set; }
        OrthographicCamera Camera { get; set; }

        void DefineObjects();
        void DefineElements();
        void DefineMaxCoord();

        void DefineGraphixObects();
        void DefineAdditionalData();
        void DefineCamera();
        void DefineTransform();
    }
}
