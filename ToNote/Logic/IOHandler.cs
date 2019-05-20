namespace ToNote.Logic
{
    using Microsoft.Win32;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Forms;
    using ToNote.Controls;
    using ToNote.Interfaces;
    using ToNote.Models;


    public static class IOHandler
    {
        #if DEBUG
        private static readonly string directory = ".\\Data\\";
        #else
        public static readonly string directory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Sugma\\ToNote\\Data\\";
        #endif

        private static readonly string metadataSuffix = "_Metadata.txt";
        private static readonly string todoSuffix = "_TODO.txt";
        private static readonly string exportFileName = "\\Notes.tonote";

        public static List<Note> DeserializeNotes()
        {
            var notes = new List<Note>();

            if (Directory.Exists(directory))
            {
                foreach (var file in Directory.GetFiles(directory).Where(x => Path.GetFileName(x).Contains(metadataSuffix)))
                {
                    using (var reader = new StreamReader(file))
                        notes.Add(JsonConvert.DeserializeObject<Note>(reader.ReadToEnd()));
                }
            }

            return notes;
        }

        public static void SerializeNote(NotePanel notePanel)
        {
            if (notePanel == null) return;

            var todoIndex = 0;
            var fileIndex = 0;

            var filePath = directory + $"{notePanel.Note.Name}";

            var sanitizedPath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileName(filePath).Trim(Path.GetInvalidFileNameChars()));

            if (!Directory.Exists(sanitizedPath))
                Directory.CreateDirectory(sanitizedPath);
            else
            {
                foreach (var file in Directory.GetFiles(sanitizedPath))
                    File.Delete(file);
            }

            notePanel.Note.FileNames.Clear();
            notePanel.Note.Todos.Clear();

            foreach (var item in notePanel.Items)
            {
                string path;

                if (!(item is IExtendedTextBoxControl extendedTextBoxControl)) continue;

                if (extendedTextBoxControl is TodoControl todoControl)
                {
                    path = $"{sanitizedPath}\\{todoIndex}_{notePanel.Note.Name.ToLower()}{todoSuffix}";

                    notePanel.Note.Todos.Add(SerializeTodo(todoControl, path, todoIndex));

                    todoIndex++;
                }

                if (extendedTextBoxControl is ExtendedRichTextBox rtb)
                {
                    path = $"{sanitizedPath}\\{fileIndex}_{notePanel.Note.Name.ToLower()}";

                    SerializeTextRange(rtb.TextRange, path);

                    notePanel.Note.FileNames.Add(path);

                    fileIndex += 1;
                }
            }

            var serializedNote = JsonConvert.SerializeObject(notePanel.Note);

            var metadataFile = directory + $"{notePanel.Note.Name.ToLower()}{metadataSuffix}";

            File.WriteAllText(metadataFile, serializedNote);
        }

        public static Todo SerializeTodo(TodoControl todoControl, string path, int index)
        {
            todoControl.Todo.FileName = path;
            todoControl.Todo.Index = index;

            SerializeTextRange(todoControl.TextRange, path);

            return todoControl.Todo;
        }

        public static void SerializeTextRange(TextRange range, string path)
        {
            using (var stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                range.Save(stream, System.Windows.DataFormats.Rtf);
            }
        }

        public static void RemoveNote(Note note)
        {
            while (note.FileNames.Any())
                note.DeleteFile(note.FileNames.First());

            while (note.Todos.Any())
                note.RemoveTodo(note.Todos.First());

            var path = directory + $"{note.Name.ToLower()}";

            var metadataFile = $"{path}{metadataSuffix}";

            if (File.Exists(metadataFile))
                File.Delete(metadataFile);

            if (Directory.Exists(path))
                Directory.Delete(path);
        }

        public static void ExportNotes(IEnumerable<Note> notes)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                var result = folderDialog.ShowDialog();

                if (result == DialogResult.OK && Directory.Exists(folderDialog.SelectedPath))
                {
                    var dtos = new List<NoteDto>();

                    foreach (var note in notes)
                    {
                        var notedto = new NoteDto() { Note = note, Files = new List<NoteTextBoxDto>(), Todos = new List<NoteTodoDto>() };

                        foreach (var file in note.FileNames)
                        {
                            using (var reader = new StreamReader(file))
                                notedto.Files.Add(new NoteTextBoxDto() { FileName = file, Content = reader.ReadToEnd() });
                        }

                        foreach (var todo in note.Todos)
                        {
                            using (var reader = new StreamReader(todo.FileName))
                                notedto.Todos.Add(new NoteTodoDto() { Todo = todo, Content = reader.ReadToEnd() });
                        }

                        dtos.Add(notedto);
                    }

                    var json = JsonConvert.SerializeObject(dtos);
                    var fileName = folderDialog.SelectedPath + exportFileName;

                    File.WriteAllText(fileName, json);
                }
            }
        }

        public static List<Note> ReadNotesFromFile()
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog();

            fileDialog.Title = "Import notes";
            fileDialog.Filter = "ToNote file (*.tonote)|*.tonote";

            bool? result = fileDialog.ShowDialog();

            if (result != null && result == true && !string.IsNullOrWhiteSpace(fileDialog.FileName))
            {
                try
                {
                    var notes = new List<Note>();

                    using (var reader = new StreamReader(fileDialog.FileName))
                    {
                        var dtos = JsonConvert.DeserializeObject<List<NoteDto>>(reader.ReadToEnd());

                        foreach (var dto in dtos)
                        {
                            notes.Add(dto.Note);

                            foreach (var file in dto.Files)
                            {
                                if (!File.Exists(file.FileName))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(file.FileName));
                                    File.Create(file.FileName).Close();

                                    File.WriteAllText(file.FileName, file.Content);
                                }
                            }

                            foreach (var todoDto in dto.Todos)
                            {
                                if (!File.Exists(todoDto.Todo.FileName))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(todoDto.Todo.FileName));
                                    File.Create(todoDto.Todo.FileName).Close();

                                    File.WriteAllText(todoDto.Todo.FileName, todoDto.Content);
                                }
                            }
                        }
                    }

                    return notes;
                }
                catch (JsonReaderException ex)
                {
                    System.Windows.MessageBox.Show("Invalid file selected. Please select a valid ToNote file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return null;
        }
    }
}
