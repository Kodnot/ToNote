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

        }

        protected void Close_Window(object sender, RoutedEventArgs e)
        {
            if (e.Source != null)
                this.Close();
        }

    }
}
