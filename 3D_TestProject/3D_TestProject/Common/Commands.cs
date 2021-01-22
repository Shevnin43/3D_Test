using System.Windows.Input;

namespace _3D_TestProject
{
    public class Commands
    {
        private static readonly RoutedUICommand getLines;
        public static RoutedUICommand GetLines { get { return getLines; } }
        private static readonly RoutedUICommand getPlane;
        public static RoutedUICommand GetPlane { get { return getPlane; } }
        private static readonly RoutedUICommand defineTransform;
        public static RoutedUICommand DefineTransform { get { return defineTransform; } }
        private static readonly RoutedUICommand moveCamera;
        public static RoutedUICommand MoveCamera { get { return moveCamera; } }
        private static readonly RoutedUICommand revert2D;
        public static RoutedUICommand Revert2D { get { return revert2D; } }
        private static readonly RoutedUICommand movePlane;
        public static RoutedUICommand MovePlane { get { return movePlane; } }

        static Commands()
        {
            getLines = new RoutedUICommand("Получить отрезки из XML", "GetLines", typeof(Commands));
            getPlane = new RoutedUICommand("Получить плоскость из XML", "GetPlane", typeof(Commands));
            defineTransform = new RoutedUICommand("Повороты модели", "DefineTransform", typeof(Commands));
            moveCamera = new RoutedUICommand("Управление камерой", "MoveCamera", typeof(Commands));
            revert2D = new RoutedUICommand("Отражение окна 2D", "Revert2D", typeof(Commands));
            moveCamera = new RoutedUICommand("Управление камерой", "MoveCamera", typeof(Commands));
            movePlane = new RoutedUICommand("Управление движением плоскости", "MovePlane", typeof(Commands));
        }
    }
}
