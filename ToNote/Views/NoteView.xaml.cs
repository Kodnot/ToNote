namespace ToNote.Views
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using ToNote.Controls;

    /// <summary>
    /// Interaction logic for NoteView.xaml
    /// </summary>
    public partial class NoteView : UserControl
    {
        public NoteView()
        {
            InitializeComponent();


            this.AllowDrop = true;

            this.PreviewMouseMove += (s, e) =>
            {
                var point = e.GetPosition(this);

                Mouse.OverrideCursor = point.X <= 25 && point.Y <= 25 ? Cursors.SizeAll : null;
            };

            this.MouseLeave += (s, e) =>
            {
                Mouse.OverrideCursor = null;
            };

            this.PreviewMouseLeftButtonDown += (s, e) =>
            {
                var point = e.GetPosition(this);

                if (point.X <= 25 && point.Y <= 25)
                    DragHandler.HandleMouseLeftButtonDown(s, e);
            };

            this.Drop += DragHandler.HandleDrop;

            this.PreviewDragOver += (s, e) =>
            {
                e.Handled = true;
            };
        }
    }
}
