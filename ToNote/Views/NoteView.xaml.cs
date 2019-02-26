namespace ToNote.Views
{
    using Newtonsoft.Json;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
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

            this.Loaded += (s, e) =>
            {
                var note = this.DataContext as Note;

                foreach (var file in note.FileNames)
                {
                    var rtb = new ExtendedRichTextBox() { Style = App.Current.TryFindResource("NoteContentRichTextBoxStyle") as Style };
                    rtb.BackspacePressedWhileEmpty += (o, a) =>
                    {
                        if (notespanel.Items.Contains(rtb))
                        notespanel.Items.Remove(rtb);

                        if (File.Exists(file))
                            File.Delete(file);

                        if (note.FileNames.Contains(file))
                            note.FileNames.Remove(file);
                    };

                    if (File.Exists(file))
                    {
                        var text = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                        if (file != null)
                            using (var stream = new FileStream(file, FileMode.Open))
                            {
                                text.Load(stream, DataFormats.Rtf);
                            }
                    }

                    notespanel.Items.Add(rtb);
                }
            };

            button_addrtb.Click += (s, e) => 
            {
                notespanel.Items.Add(new ExtendedRichTextBox() { Style = App.Current.TryFindResource("NoteContentRichTextBoxStyle") as Style });
            };

            button_save.Click += (s, e) => 
            {
                var note = this.DataContext as Note;

                var index = 0;

                foreach (var child in notespanel.Items)
                {
                    if (child is ExtendedRichTextBox rtb)
                    {
                        string file;
                        bool newfile = false;

                        if (note.FileNames.Count > index && note.FileNames[index] != null)
                            file = note.FileNames[index];
                        else
                        {
                            file = $"{note.Name.ToLower()}_{index}";
                            newfile = true;
                        }

                        using (var stream = new FileStream(file, FileMode.OpenOrCreate))
                        {
                            var text = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                            text.Save(stream, DataFormats.Rtf);
                        }

                        if (newfile)
                            note.FileNames.Add(file);

                        index += 1;
                    }
                }

                var serializedNote = JsonConvert.SerializeObject(note);
                var metadataFileName = $"{note.Name.ToLower()}Metadata.txt";
                File.WriteAllText(metadataFileName, serializedNote);
            };
        }
    }
}
