namespace ToNote.Controls
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using ToNote.Logic;
    using ToNote.Models;

    public class NoteRichTextBoxPanel : ItemsControl
    {
        public NoteRichTextBoxPanel()
        {
            // CTRL + Arrow Up or CTRL + Arrow Down navigates to a textbox that is above or below currently focused one, respectively.
            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Up && Keyboard.Modifiers == ModifierKeys.Control)
                    SwitchKeyboardFocusToNextRTB(this.Items.IndexOf(Keyboard.FocusedElement));

                if (e.Key == Key.Down && Keyboard.Modifiers == ModifierKeys.Control)
                    SwitchKeyboardFocusToNextRTB(this.Items.IndexOf(Keyboard.FocusedElement), false);
            };

            // Tracks if new items have been added to the panel. If they were ExtendedRichTextBoxes, sets up appropriate events.
            ((INotifyCollectionChanged)this.Items).CollectionChanged += (s, e) =>
            {
                if (e.NewItems == null) return;

                foreach (var item in e.NewItems.OfType<ExtendedRichTextBox>())
                    ConfigureRichTextBoxEvents(item);
            };
        }
            
        public Note Note
        {
            get { return (Note)GetValue(NoteProperty); }
            set { SetValue(NoteProperty, value); }
        }

        // On a new value bound to Note, generates textboxes for each file the note has and fills them with their respective content.
        public static readonly DependencyProperty NoteProperty = DependencyProperty.Register("Note",
           typeof(Note), typeof(NoteRichTextBoxPanel), new FrameworkPropertyMetadata(null) { PropertyChangedCallback = (s, e) =>
           {
               if (!(e.NewValue is Note newNoteValue) || newNoteValue == null) return;

               var panel = (NoteRichTextBoxPanel)s;

               panel.Items.Clear();

               foreach (var file in (newNoteValue.FileNames))
               {
                   var rtb = new ExtendedRichTextBox();

                   rtb.ReadFromFile(file);

                   panel.Items.Add(rtb);
               }
           }
           });

        //Command to add a new TextBox. Implemented so the command can be invoked from XAML instead of back-end;
        public ICommand AddRichTextBoxCommand
        {
            get { return (ICommand)GetValue(AddRichTextBoxCommandProperty); }
            set { SetValue(AddRichTextBoxCommandProperty, value); }
        }

        public static readonly DependencyProperty AddRichTextBoxCommandProperty = DependencyProperty.Register("AddRichTextBoxCommand",
           typeof(ICommand), typeof(NoteRichTextBoxPanel), new FrameworkPropertyMetadata(new RelayCommand<NoteRichTextBoxPanel>((panel) => 
           {
               panel.Items.Add(new ExtendedRichTextBox());
           })));

        //Command to save each ExtendedRichTextBox control's contents to a respective .rtf file
        public ICommand SaveContentsToFilesCommand
        {
            get { return (ICommand)GetValue(SaveContentsToFilesCommandProperty); }
            set { SetValue(SaveContentsToFilesCommandProperty, value); }
        }

        public static readonly DependencyProperty SaveContentsToFilesCommandProperty = DependencyProperty.Register("SaveContentsToFilesCommand",
           typeof(ICommand), typeof(NoteRichTextBoxPanel), new FrameworkPropertyMetadata(new RelayCommand<NoteRichTextBoxPanel>((panel) =>
           {
               var note = panel?.Note;

               if (note == null) return;

               var index = 0;

               var directory = ".\\Notes";

               if (!Directory.Exists(directory))
                   Directory.CreateDirectory(directory);

               foreach (var child in panel.Items)
               {
                   if (!(child is ExtendedRichTextBox rtb)) continue;

                   string file;
                   bool newfile = false;

                   if (note.FileNames.Count > index && note.FileNames[index] != null)
                       file = note.FileNames[index];
                   else
                   {
                       file = $"{directory}\\{note.Name.ToLower()}_{index}";
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

               var serializedNote = JsonConvert.SerializeObject(note);
               var metadataFileName = $"{note.Name.ToLower()}Metadata.txt";
               File.WriteAllText(metadataFileName, serializedNote);
           })));

        /// <summary>
        /// Applies appropriate events to the provided ExtendedRichTextBox control
        /// </summary>
        /// <param name="rtb"></param>
        private void ConfigureRichTextBoxEvents(ExtendedRichTextBox rtb)
        {
            rtb.BackspacePressedWhileEmpty += (s, e) =>
            {
                var index = this.Items.IndexOf(Keyboard.FocusedElement);

                SwitchKeyboardFocusToNextRTB(index);

                if (this.Items.Contains(rtb))
                    this.Items.Remove(rtb);

                Note?.DeleteFile(rtb.CurrentFile);
            };

            //Insertion of a new ExtendedRichBox at the end with a /note command
            rtb.TrackKeyword("note", () => {

                this.AddRichTextBoxCommand.Execute(this);

                //Focus has to be changed asynchronously, else an exception is thrown.
                Dispatcher.BeginInvoke((Action)(() => { Keyboard.Focus((ExtendedRichTextBox)this.Items[this.Items.Count - 1]); }));
            });
        }

        /// <summary>
        /// Gives a neighbour ExtendedRichTextBox control the focus.
        /// </summary>
        /// <param name="index">Index of current RTB in the Items Array</param>
        /// <param name="up">Whether to navigate up the list.</param>
        private void SwitchKeyboardFocusToNextRTB(int index, bool up = true)
        {
            if (up)
            {
                var rtb = Items.Cast<object>().Take(index).OfType<ExtendedRichTextBox>().LastOrDefault();

                if (rtb != null)
                {
                    Keyboard.Focus(rtb);
                }
            }
            else
            {
                var rtb = Items.Cast<object>().Skip(index + 1).OfType<ExtendedRichTextBox>().FirstOrDefault();

                if (rtb != null)
                    Keyboard.Focus(rtb);
            }
        }
    }
}
