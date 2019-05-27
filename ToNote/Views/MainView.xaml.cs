namespace ToNote
{
    using System;
    using System.IO;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Sugma//ToNote//Data//";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            InitializeComponent();

            //Prevents the maximize button from covering taskbar. Doesn't work well if multiple monitors of different resolutions are used.
            var state = WindowState.Normal;
            var currentHeight = this.Height;
            var currentWidth = this.Width;
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.MaxHeight = desktopWorkingArea.Height;
            this.MaxWidth = desktopWorkingArea.Width;

            btn_MinimizeWindow.Click += (s, e) =>
            {
                if (this.WindowState != WindowState.Minimized)
                    this.WindowState = WindowState.Minimized;
            };

            btn_MaximizeWindow.Click += (s, e) =>
            {
                if (state != WindowState.Maximized)
                {
                    state = WindowState.Maximized;
                    this.Width = desktopWorkingArea.Width;
                    this.Height = desktopWorkingArea.Height;
                    this.Left = desktopWorkingArea.Left;
                    this.Top = desktopWorkingArea.Top;
                    return;
                }

                if (state == WindowState.Maximized)
                {
                    state = WindowState.Normal;
                    this.Width = currentWidth;
                    this.Height = currentHeight;
                    this.Left = this.MaxWidth / 2 - currentWidth / 2;
                    this.Top = this.MaxHeight / 2 - currentHeight / 2;
                }
            };

            btn_CloseWindow.Click += (s, e) =>
            {
                if (e.Source != null)
                    this.Close();
            };
        }
    }
}
