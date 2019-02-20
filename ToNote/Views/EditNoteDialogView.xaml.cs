namespace ToNote.Views
{
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using ToNote.ViewModels;

    /// <summary>
    /// Interaction logic for EditNoteDialogView.xaml
    /// </summary>
    public partial class EditNoteDialogView : UserControl
    {
        public EditNoteDialogView()
        {
            InitializeComponent();

            this.Loaded += (s, e) =>
            {
                var note = ((EditNoteDialogViewModel)this.DataContext).Note;
                var file = $"{note.Name.ToLower()}.rtf";

                if (File.Exists(file))
                {
                    var text = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                    using (var stream = new FileStream(file, FileMode.Open))
                    {
                        text.Load(stream, DataFormats.Rtf);
                    }
                }
            };

            button_save.Click += (s, e) => 
            {
                var note = ((EditNoteDialogViewModel)this.DataContext).Note;
                var file = $"{note.Name.ToLower()}.rtf";

                using (var stream = new FileStream(file, FileMode.OpenOrCreate))
                {
                    var text = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                    text.Save(stream, DataFormats.Rtf);
                }
            };
        }
    }
}
