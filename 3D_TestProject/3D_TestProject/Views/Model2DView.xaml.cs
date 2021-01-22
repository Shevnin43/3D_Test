using _3D_TestProject.Classes;
using _3D_TestProject.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace _3D_TestProject.Views
{
    /// <summary>
    /// Логика взаимодействия для Model2DView.xaml
    /// </summary>
    public partial class Model2DView : Window
    {
        public Model2D _model = new Model2D();

        /// <summary>
        /// Привязка команд
        /// </summary>
        private void BindingCommands()
        {
            CommandBindings.Add(new CommandBinding(Commands.Revert2D, _model.Revert2D));
        }

        /// <summary>
        /// Мсье конструктор
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="allLines"></param>
        public Model2DView(BasePlane plane, ICollection<BaseLineSegment> allLines)
        {
            _model.AllLineSegments = allLines;
            _model.Plane = plane;
            InitializeComponent();
            BindingCommands();
            DataContext = _model;
            Topmost = true;
        }
    }
}
