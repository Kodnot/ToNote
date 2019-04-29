namespace ToNote
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using ToNote.Controls;

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

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var children = GetLogicalChildCollection<TodoControl>(this);
            sadas.ItemsSource = children;
            //MessageBox.Show("Bybysnx");
        }



        public static List<T> GetLogicalChildCollection<T>(object parent) where T : DependencyObject
        {
            List<T> logicalCollection = new List<T>();
            GetLogicalChildCollection(parent as DependencyObject, logicalCollection);
            return logicalCollection;
        }

        private static void GetLogicalChildCollection<T>(DependencyObject parent, List<T> logicalCollection) where T : DependencyObject
        {
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            foreach (object child in children)
            {
                if (child is DependencyObject)
                {
                    DependencyObject depChild = child as DependencyObject;
                    if (child is T)
                    {
                        logicalCollection.Add(child as T);
                    }
                    GetLogicalChildCollection(depChild, logicalCollection);
                }
            }
        }
    }
}
