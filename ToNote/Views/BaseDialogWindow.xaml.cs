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

            btn_CloseWindow.Click += (s, e) =>
            {
                if (e.Source != null)
                    this.Close();
            };
        }
    }
}
