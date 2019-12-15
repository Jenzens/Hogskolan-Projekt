using System.Windows;
using System.Windows.Input;

namespace TheMagnificentPresentation.Views
{
    partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ExpandWindow_Click(object sender, RoutedEventArgs e)
        {
            if (ExpandWindow.Content == FindResource("MaxWindow"))
            {
                ExpandWindow.Content = FindResource("MinWindow");
                this.WindowState = WindowState.Normal;
            }
            else
            {
                ExpandWindow.Content = FindResource("MaxWindow");
                this.WindowState = WindowState.Maximized;
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}

