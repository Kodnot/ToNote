using Newtonsoft.Json;
using ToNote.Models;

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
                var file = note.FileNames[0];

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

                note.Description = "Test description";
                note.FileNames.Add(file);
                var serializedNote = JsonConvert.SerializeObject(note);
                var metadataFileName = $"{note.Name.ToLower()}Metadata.txt";
                File.WriteAllText(metadataFileName, serializedNote);
            };
        }
    }
}
