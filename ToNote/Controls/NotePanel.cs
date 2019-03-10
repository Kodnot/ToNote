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
                    SwitchKeyboardFocusToNextETBC(this.Items.IndexOf(lastFocused));

                if (e.Key == Key.Down && Keyboard.Modifiers == ModifierKeys.Control)
                    SwitchKeyboardFocusToNextETBC(this.Items.IndexOf(lastFocused), false);
            };

            // Tracks if new items have been added to the panel. If they were IExtendedTextBoxControls, sets up appropriate events.
            ((INotifyCollectionChanged)this.Items).CollectionChanged += (s, e) =>
            {
                if (e.NewItems == null) return;

                foreach (var item in e.NewItems.OfType<IExtendedTextBoxControl>())
                    ConfigureExtendedTextBoxControlEvents(item);

            };
        }

        private IExtendedTextBoxControl lastFocused;
            
        public Note Note
        {
            get => (Note)GetValue(NoteProperty);
            set => SetValue(NoteProperty, value);
        }

        // On a new value bound to Note, generates textboxes for each file or Todo the note has and fills them with their respective contents.
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

               foreach (var todo in newNoteValue.Todos.OrderBy(x => x.Index))
               {
                   var todoControl = new TodoControl(todo);

                   todoControl.ReadFromFile(todo.FileName);

                   panel.Items.Insert(todo.Index, todoControl);
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

        //Command to save each IExtendedTextBoxControl's contents to a respective .rtf file
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

               var todoIndex = 0;
               var fileIndex = 0;

               var directory = ".\\Data";

               if (!Directory.Exists(directory))
                   Directory.CreateDirectory(directory);

               foreach (var child in panel.Items)
               {
                   if (!(child is IExtendedTextBoxControl extendedTextBoxControl)) continue;

                   string file = null;
                   TextRange range = null;
                  
                   if (extendedTextBoxControl is TodoControl todoControl)
                   {
                       file = $"{directory}\\{todoIndex}_{note.Name.ToLower()}_TODO";

                       todoControl.Todo.FileName = file;

                       todoControl.Todo.Index = panel.Items.IndexOf(todoControl);

                       if (!note.Todos.Contains(todoControl.Todo))
                           note.Todos.Add(todoControl.Todo);

                       range = todoControl.TextRange;

                       todoIndex += 1;
                   }

                   if (extendedTextBoxControl is ExtendedRichTextBox rtb)
                   {
                       range = rtb.TextRange;
                       bool newfile = false;

                       if (note.FileNames.Count > fileIndex && note.FileNames[fileIndex] != null)
                           file = note.FileNames[fileIndex];
                       else
                       {
                           file = $"{directory}\\{fileIndex}_{note.Name.ToLower()}";
                           newfile = true;
                       }

                       if (newfile)
                           note.FileNames.Add(file);

                       fileIndex += 1;
                   }

                   using (var stream = new FileStream(file, FileMode.OpenOrCreate))
                   {
                       range?.Save(stream, DataFormats.Rtf);
                   }
               }

               var serializedNote = JsonConvert.SerializeObject(note);
               var metadataFileName = $"{note.Name.ToLower()}Metadata.txt";
               File.WriteAllText(metadataFileName, serializedNote);
           })));

        public ICommand AddTodoControlCommand
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
               {
                   panel.Items.Add(todoControl);
               }
           })));

        /// <summary>
        /// Applies appropriate events to the provided ExtendedRichTextBox control
        /// </summary>
        /// <param name="extendedTextBoxControl"></param>
        private void ConfigureExtendedTextBoxControlEvents(IExtendedTextBoxControl extendedTextBoxControl)
        {
            extendedTextBoxControl.BackspacePressedWhileEmpty += (s, e) =>
            {
                var index = this.Items.IndexOf(lastFocused);

                SwitchKeyboardFocusToNextETBC(index);

                if (this.Items.Contains(extendedTextBoxControl))
                    this.Items.Remove(extendedTextBoxControl);

                if (extendedTextBoxControl is ExtendedRichTextBox ertb)
                    Note?.DeleteFile(ertb.CurrentFile);

                if (extendedTextBoxControl is TodoControl todoControl)
                    Note?.RemoveTodo(todoControl.Todo);
            };

            extendedTextBoxControl.GotKeyboardFocus += (s, e) =>
            {
                lastFocused = (IExtendedTextBoxControl)s;
            };
        }

        /// <summary>
        /// Gives a neighbour IExtendedTextBoxControl keyboard focus.
        /// </summary>
        /// <param name="index">Index of current IExtendedTextBoxControl in the Items Array</param>
        /// <param name="up">Whether to navigate up the list.</param>
        private void SwitchKeyboardFocusToNextETBC(int index, bool up = true)
        {
            if (up)
            {
                var etbc = Items.Cast<object>().Take(index).OfType<IExtendedTextBoxControl>().LastOrDefault();

                if (etbc != null)
                    etbc.SetKeyboardFocus();
            }
            else
            {
                var etbc = Items.Cast<object>().Skip(index + 1).OfType<IExtendedTextBoxControl>().FirstOrDefault();

                if (etbc != null)
                    etbc.SetKeyboardFocus();
            }
        }
    }
}
