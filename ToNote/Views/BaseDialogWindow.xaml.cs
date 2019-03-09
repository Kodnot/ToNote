namespace ToNote.Views
{
    using System.Windows;
    using ToNote.Interfaces;

    /// <summary>
    /// Interaction logic for BaseDialogWindow.xaml
    /// </summary>
    public partial class BaseDialogWindow : Window, IDialogWindow
    {
        public BaseDialogWindow()
        {
            InitializeComponent();
            this.InvalidateVisual();
        }

        protected void Minimize_Window(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Minimized;
        }

        protected void Maximize_Window(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
        }

        protected void Close_Window(object sender, RoutedEventArgs e)
        {
            if (e.Source != null)
                this.Close();
        }

    }
}
