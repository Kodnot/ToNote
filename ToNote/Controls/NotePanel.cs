namespace ToNote.Controls
{
    using Newtonsoft.Json;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using ToNote.Interfaces;
    using ToNote.Logic;
    using ToNote.Models;

    public class NotePanel : ItemsControl
    {
        public NotePanel()
        {
            // CTRL + Arrow Up or CTRL + Arrow Down navigates to an item that is above or below currently focused one, respectively.
            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Up && Keyboard.Modifiers == ModifierKeys.Control)
                    SwitchKeyboardFocusToNextRTB(this.Items.IndexOf(lastFocused));

                if (e.Key == Key.Down && Keyboard.Modifiers == ModifierKeys.Control)
                    SwitchKeyboardFocusToNextRTB(this.Items.IndexOf(lastFocused), false);
            };

            // Tracks if new items have been added to the panel. If they were ExtendedRichTextBoxes or TodoControls, sets up appropriate events.
            ((INotifyCollectionChanged)this.Items).CollectionChanged += (s, e) =>
            {
                if (e.NewItems == null) return;

                foreach (var item in e.NewItems.OfType<IExtendedTextBoxControl>())
                    ConfigureRichTextBoxEvents(item);

            };
        }

        private IExtendedTextBoxControl lastFocused;
            
        public Note Note
        {
            get => (Note)GetValue(NoteProperty);
            set => SetValue(NoteProperty, value);
        }

        // On a new value bound to Note, generates textboxes for each file the note has and fills them with their respective content.
        public static readonly DependencyProperty NoteProperty = DependencyProperty.Register("Note",
           typeof(Note), typeof(NotePanel), new FrameworkPropertyMetadata(null) { PropertyChangedCallback = (s, e) =>
           {
               if (!(e.NewValue is Note newNoteValue) || newNoteValue == null) return;

               var panel = (NotePanel)s;

               panel.Items.Clear();

               foreach (var file in (newNoteValue.FileNames))
               {
                   var rtb = new ExtendedRichTextBox()
                   { Style = App.Current.TryFindResource("NoteContentRichTextBoxStyle") as Style };

                   rtb.ReadFromFile(file);

                   panel.Items.Add(rtb);
               }
           }
           });

        //Command to add a new TextBox. Implemented so the command can be invoked from XAML instead of back-end;
        public ICommand AddRichTextBoxCommand
        {
            get => (ICommand)GetValue(AddRichTextBoxCommandProperty);
            set => SetValue(AddRichTextBoxCommandProperty, value);
        }

        public static readonly DependencyProperty AddRichTextBoxCommandProperty = DependencyProperty.Register("AddRichTextBoxCommand",
           typeof(ICommand), typeof(NotePanel), new FrameworkPropertyMetadata(new RelayCommand<NotePanel>((panel) => 
           {
               var rtb = new ExtendedRichTextBox() { Style = App.Current.TryFindResource("NoteContentRichTextBoxStyle") as Style };
               panel.Items.Add(rtb);
               Keyboard.Focus(rtb);
           })));

        //Command to save each ExtendedRichTextBox control's contents to a respective .rtf file
        public ICommand SaveContentsToFilesCommand
        {
            get => (ICommand)GetValue(SaveContentsToFilesCommandProperty);
            set => SetValue(SaveContentsToFilesCommandProperty, value);
        }

        public static readonly DependencyProperty SaveContentsToFilesCommandProperty = DependencyProperty.Register("SaveContentsToFilesCommand",
           typeof(ICommand), typeof(NotePanel), new FrameworkPropertyMetadata(new RelayCommand<NotePanel>((panel) =>
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

        public ICommand AddTodoNoteCommand
        {
            get => (ICommand)GetValue(AddTodoControlCommandProperty);
            set => SetValue(AddTodoControlCommandProperty, value);
        }

        public static readonly DependencyProperty AddTodoControlCommandProperty = DependencyProperty.Register("AddTodoControlCommand",
           typeof(ICommand), typeof(NotePanel), new FrameworkPropertyMetadata(new RelayCommand<NotePanel>((panel) =>
           {
               var todoControl = new TodoControl(new Todo()).SetKeyboardFocusAfterLoaded();

               if (panel.lastFocused != null)
               {
                   var index = panel.Items.IndexOf(panel.lastFocused) + 1;
                   panel.Items.Insert(index, todoControl);
               }
               else
                   panel.Items.Add(todoControl);
           })));

        /// <summary>
        /// Applies appropriate events to the provided ExtendedRichTextBox control
        /// </summary>
        /// <param name="rtb"></param>
        private void ConfigureRichTextBoxEvents(IExtendedTextBoxControl rtb)
        {
            rtb.BackspacePressedWhileEmpty += (s, e) =>
            {
                var index = this.Items.IndexOf(lastFocused);

                SwitchKeyboardFocusToNextRTB(index);

                if (this.Items.Contains(rtb))
                    this.Items.Remove(rtb);

                //Only check for ExtendedRichTextBox because TODOs lack serialization
                if (rtb is ExtendedRichTextBox ertb)
                    Note?.DeleteFile(ertb.CurrentFile);
            };

            rtb.GotKeyboardFocus += (s, e) =>
            {
                lastFocused = (IExtendedTextBoxControl)s;
            };
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
                var rtb = Items.Cast<object>().Take(index).OfType<IExtendedTextBoxControl>().LastOrDefault();

                if (rtb != null)
                    rtb.SetKeyboardFocus();
            }
            else
            {
                var rtb = Items.Cast<object>().Skip(index + 1).OfType<IExtendedTextBoxControl>().FirstOrDefault();

                if (rtb != null)
                    rtb.SetKeyboardFocus();
            }
        }
    }
}
