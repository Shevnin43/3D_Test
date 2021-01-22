using _3D_TestProject.Models;
using System;
using System.Windows;
using System.Windows.Input;

namespace _3D_TestProject.Views
{
    /// <summary>
    /// Логика взаимодействия для Model3DView.xaml
    /// </summary>
    public partial class Model3DView : Window
    {
        private readonly Model3D _model = new Model3D();
        private Model2DView Win2D;

        /// <summary>
        /// Инициализация окна
        /// </summary>
        public Model3DView()
        {
            InitializeComponent();
            BindingCommands();
            DataContext = _model;

        }
        /// <summary>
        ///  Привязка команд
        /// </summary>
        private void BindingCommands()
        {
            CommandBindings.Add(new CommandBinding(Commands.GetLines, _model.GetXMLLines));
            CommandBindings.Add(new CommandBinding(Commands.GetPlane, _model.GetXMLPlane));
            CommandBindings.Add(new CommandBinding(Commands.DefineTransform, _model.DefineTransform));
            CommandBindings.Add(new CommandBinding(Commands.MoveCamera, _model.MoveCamera));
            CommandBindings.Add(new CommandBinding(Commands.MovePlane, _model.MovePlane));
        }

        /// <summary>
        /// Открытие окна двумерного отображения плоскости
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Show2DWindow(object sender, RoutedEventArgs e)
        {
            if (Win2D?.IsVisible == true)
            {
                return;
            }
            Win2D = new Model2DView(_model.Plane, _model.AllLineSegments);
            _model.DataChanged += (obj, args) =>
            {
                Win2D._model.AllLineSegments = args.AllLineSegments;
                Win2D._model.Plane = args.Plane;
            };
            Win2D.Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            Win2D?.Close();
            base.OnClosed(e);
        }
    }
}
