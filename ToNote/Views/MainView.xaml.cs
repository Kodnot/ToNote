namespace ToNote
{
    using System;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();

            //Prevents the maximize button from covering taskbar. Doesn't work well if multiple monitors of different resolutions are used.
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            btn_MinimizeWindow.Click += (s, e) =>
            {
                if (this.WindowState != WindowState.Minimized)
                    this.WindowState = WindowState.Minimized;
            };

            btn_MaximizeWindow.Click += (s, e) =>
            {
                if (this.WindowState != WindowState.Maximized)
                {
                    this.WindowState = WindowState.Maximized;
                    return;
                }

                if (this.WindowState == WindowState.Maximized)
                    this.WindowState = WindowState.Normal;
            };

            btn_CloseWindow.Click += (s, e) =>
            {
                if (e.Source != null)
                    this.Close();
            };
        }
    }
}
