namespace ToNote.Views
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using ToNote.Controls;
    using ToNote.Models;

    /// <summary>
    /// Interaction logic for NoteView.xaml
    /// </summary>
    public partial class NoteView : UserControl
    {
        public NoteView()
        {
            InitializeComponent();

            if (rtbpanel.ShowNotes)
            {
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
        private void Button1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Note note = (Note)DataContext;

            var tagName = ((Button)sender).DataContext.ToString();
            if (!string.IsNullOrWhiteSpace(tagName) && note.Tags.Contains(tagName))
            {
                note.Tags.Remove(tagName);
                note.TagName = "";
            }
        }
    }
}
