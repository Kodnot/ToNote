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
        WindowState state;
        public MainView()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Sugma//ToNote//Data//";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            InitializeComponent();

            //Prevents the maximize button from covering taskbar. Doesn't work well if multiple monitors of different resolutions are used.
            state = WindowState.Normal;
            var currentHeight = this.Height;
            var currentWidth = this.Width;
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.MaxHeight = desktopWorkingArea.Height;
            this.MaxWidth = desktopWorkingArea.Width;
            
            StateChanged += WindowStateChanged;

            btn_MinimizeWindow.Click += (s, e) =>
            {
                if (this.WindowState != WindowState.Minimized)
                    this.WindowState = WindowState.Minimized;
            };

            btn_MaximizeWindow.Click += (s, e) =>
            {
                if (state == WindowState.Normal)
                {
                    state = WindowState.Maximized;
                    this.Width = desktopWorkingArea.Width;
                    this.Height = desktopWorkingArea.Height;
                    this.Left = desktopWorkingArea.Left;
                    this.Top = desktopWorkingArea.Top;
                }
                else
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

        void WindowStateChanged(object sender, EventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                state = WindowState.Maximized;
                this.Width = desktopWorkingArea.Width;
                this.Height = desktopWorkingArea.Height;
                this.Left = desktopWorkingArea.Left;
                this.Top = desktopWorkingArea.Top;
            }
        }
    }
}
