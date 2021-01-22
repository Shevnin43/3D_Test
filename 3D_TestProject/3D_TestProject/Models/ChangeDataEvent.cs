using _3D_TestProject.Classes;
using System;
using System.Collections.Generic;

namespace _3D_TestProject.Models
{
    public class ChangeDataEvent : EventArgs
    {
        /// <summary>
        /// Все заданные отрезки 
        /// </summary>
        public ICollection<BaseLineSegment> AllLineSegments { get; set; }

        /// <summary>
        /// Плоскость
        /// </summary>
        public BasePlane Plane { get; set; }

        public ChangeDataEvent(BasePlane plane, ICollection<BaseLineSegment> allLineSegments )
        {
            AllLineSegments = allLineSegments;
            Plane = plane;
        }
    }
}
