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
    using System.Windows.Threading;
    using ToNote.Interfaces;
    using ToNote.Logic;
    using ToNote.Models;

    public class NotePanel : ItemsControl
    {
        public NotePanel()
        {
            // CTRL + Plus or CTRL + Minus Down navigates to an item that is above or below currently focused one, respectively.
            this.PreviewKeyDown += (s, e) =>
            {
                if ((e.Key == Key.Subtract || e.Key == Key.OemMinus) && Keyboard.Modifiers == ModifierKeys.Control)
                    SwitchKeyboardFocusToNextETBC(this.Items.IndexOf(lastFocused));

                if ((e.Key == Key.Add || e.Key == Key.OemPlus) && Keyboard.Modifiers == ModifierKeys.Control)
                    SwitchKeyboardFocusToNextETBC(this.Items.IndexOf(lastFocused), false);
            };

            // Tracks if new items have been added to the panel. If they were IExtendedTextBoxControls, sets up appropriate events.
            ((INotifyCollectionChanged)this.Items).CollectionChanged += (s, e) =>
            {
                if (e.NewItems == null) return;

                foreach (var item in e.NewItems.OfType<IExtendedTextBoxControl>())
                    ConfigureExtendedTextBoxControlEvents(item);

            };

            Status = "No changes.";

            dt = new DispatcherTimer();

            dt.Tick += (o, a) =>
            {
                if (_unsavedChanges)
                {
                    SaveContentsToFilesCommand.Execute(this);
                    Status = "Last autosaved at " + DateTime.Now.ToString("HH:mm:ss");
                }

                dt.Stop();
            };
            dt.Interval = new TimeSpan(0, 0, 5);
            dt.Start();
        }

        private IExtendedTextBoxControl lastFocused;

        private DispatcherTimer dt;

        private bool _unsavedChanges = false;

        public Note Note
        {
            get => (Note)GetValue(NoteProperty);
            set => SetValue(NoteProperty, value);
        }

        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status",
           typeof(string), typeof(NotePanel), new FrameworkPropertyMetadata(null));

        public string Status
        {
            get => (string)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public static readonly DependencyProperty MainWindowProperty = DependencyProperty.Register("MainWindow",
           typeof(Window), typeof(NotePanel), new FrameworkPropertyMetadata(null) { PropertyChangedCallback = (s, e) =>
           {
               if (!(e.NewValue is Window window)) return;

               var panel = (NotePanel)s;

               window.Closing += (o, a) =>
               {
                    panel.SaveContentsToFilesCommand.Execute(panel);
               };
           }
           });

        public Window MainWindow
        {
            get => (Window)GetValue(MainWindowProperty);
            set => SetValue(MainWindowProperty, value);
        }

        // On a new value bound to Note, generates textboxes for each file or Todo the note has and fills them with their respective contents.
        public static readonly DependencyProperty NoteProperty = DependencyProperty.Register("Note",
           typeof(Note), typeof(NotePanel), new FrameworkPropertyMetadata(null)
           {
               PropertyChangedCallback = (s, e) =>
               {
                   if (!(e.NewValue is Note newNoteValue) || newNoteValue == null) return;

                   var panel = (NotePanel)s;

                   panel.Items.Clear();
                   if (panel.ShowNotes)
                   {
                       foreach (var file in (newNoteValue.FileNames))
                       {
                           var rtb = new ExtendedRichTextBox();

                           rtb.ReadFromFile(file);

                           panel.Items.Add(rtb);
                       }
                   }
                   else
                       panel.dt.Interval = new TimeSpan(0, 0, 0, 0, 500);

                   int i = 0;

                   foreach (var todo in newNoteValue.Todos.OrderBy(x => x.Index))
                   {
                       TodoControl todoControl = new TodoControl(todo, panel.ShowNotes);

                       todoControl.ReadFromFile(todo.FileName);

                       if (!panel.ShowNotes)
                       {
                           panel.Items.Insert(i++, todoControl);
                       }
                       else
                           panel.Items.Insert(todo.Index, todoControl);
                   }

                   panel.SaveNoteEvent += (se, ev) =>
                   {
                       panel.SaveContentsToFilesCommand.Execute(panel);
                   };
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
               var rtb = new ExtendedRichTextBox();
               if (panel.lastFocused != null)
               {
                   var index = panel.Items.IndexOf(panel.lastFocused) + 1;
                   panel.Items.Insert(index, rtb);
               }
               else
                   panel.Items.Add(rtb);
               rtb.SetKeyboardFocus();
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

               var directory = $".\\Data\\{note.Name}";

               directory = Path.Combine(Path.GetDirectoryName(directory), Path.GetFileName(directory).Trim(Path.GetInvalidFileNameChars()));

               if (!Directory.Exists(directory))
                   Directory.CreateDirectory(directory);
               else
               {
                   foreach (var file in Directory.GetFiles(directory))
                   {
                       File.Delete(file);
                   }
               }

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

               panel._unsavedChanges = false;
               panel.Status = "Saved at " + DateTime.Now.ToString("HH:mm:ss");
           })));

        public ICommand AddTodoControlCommand
        {
            get => (ICommand)GetValue(AddTodoControlCommandProperty);
            set => SetValue(AddTodoControlCommandProperty, value);
        }

        public static readonly DependencyProperty AddTodoControlCommandProperty = DependencyProperty.Register("AddTodoControlCommand",
           typeof(ICommand), typeof(NotePanel), new FrameworkPropertyMetadata(new RelayCommand<NotePanel>((panel) =>
           {
               var todoControl = new TodoControl(new Todo(), true).SetKeyboardFocusAfterLoaded();

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
            extendedTextBoxControl.BackspacePressedWithAltShiftModifiers += (s, e) =>
            {
                var index = this.Items.IndexOf(lastFocused);

                if (this.Items.Contains(extendedTextBoxControl))
                    this.Items.Remove(extendedTextBoxControl);

                if (extendedTextBoxControl is ExtendedRichTextBox ertb)
                    Note?.DeleteFile(ertb.CurrentFile);

                if (extendedTextBoxControl is TodoControl todoControl)
                    Note?.RemoveTodo(todoControl.Todo);
                SwitchKeyboardFocusToNextETBC(index);

            };

            extendedTextBoxControl.GotKeyboardFocus += (s, e) =>
            {
                lastFocused = (IExtendedTextBoxControl)s;
            };

            //Insertion of a new ExtendedRichBox at the end with a /note command
            extendedTextBoxControl.TrackKeyword("note", () =>
            {

                this.AddRichTextBoxCommand.Execute(this);

            });

            extendedTextBoxControl.TrackKeyword("todo", () =>
            {
                var todoControl = new TodoControl(new Todo(), true).SetKeyboardFocusAfterLoaded();

                var index = this.Items.IndexOf(extendedTextBoxControl) + 1;

                if (extendedTextBoxControl is ExtendedRichTextBox rtb)
                {
                    var leftRange = new TextRange(rtb.Document.ContentStart, rtb.CommandExecutionPointer);
                    var rightRange = new TextRange(rtb.CommandExecutionPointer, rtb.Document.ContentEnd);

                    this.Items.Insert(index, todoControl);

                    if (rightRange.Text?.Length >= 2 && rightRange.Text[0] == '\r' && rightRange.Text[1] == '\n')
                    {
                        rightRange = new TextRange(rtb.CommandExecutionPointer.GetNextContextPosition(LogicalDirection.Forward).GetNextContextPosition(LogicalDirection.Forward), rtb.Document.ContentEnd);
                    }

                    var newRtb = new ExtendedRichTextBox();
                    using (var stream = new MemoryStream())
                    {
                        if (!String.IsNullOrWhiteSpace(rightRange.Text))
                        {
                            rightRange.Save(stream, DataFormats.Rtf);
                            newRtb.TextRange.Load(stream, DataFormats.Rtf);
                            this.Items.Insert(index + 1, newRtb);
                            stream.SetLength(0);
                        }

                        leftRange.Save(stream, DataFormats.Rtf);
                        rtb.TextRange.Load(stream, DataFormats.Rtf);
                    }
                }
                else
                    this.Items.Insert(index, todoControl);
            });

            extendedTextBoxControl.TextChanged += (s, e) =>
            {
                if (extendedTextBoxControl.Initializing != true)
                {
                    InitializeAutosaveDispatcher();
                }
            };

            extendedTextBoxControl.Drop += (s, e) =>
            {
                InitializeAutosaveDispatcher();
            };

            extendedTextBoxControl.LostKeyboardFocus += (s, e) =>
            {
                SaveContentsToFilesCommand.Execute(this);
            };

        }

        private void InitializeAutosaveDispatcher()
        {
            Status = "Unsaved changes.";


            _unsavedChanges = true;

            if (!dt.IsEnabled)
                dt.Start();
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

        public bool ShowNotes
        {
            get => (bool)GetValue(ShowNotesProperty);
            set => SetValue(ShowNotesProperty, value);
        }

        public static readonly DependencyProperty ShowNotesProperty = DependencyProperty.Register("ShowNotes",
            typeof(bool), typeof(NotePanel), new FrameworkPropertyMetadata(true));

        public RoutedEventHandler SaveNoteEvent;
    }
}
