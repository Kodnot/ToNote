using System.Windows;
using ToNote;
using ToNote.Models;
using ToNote.ViewModels;
using Xunit;
using ToNoteTests.Core;
using Shouldly;
using ToNote.Views;
using System.Reflection;

namespace ToNoteTests.UI
{
    public class NoteViewOrderTests
    {
        [UIFact]
        public void ReorderNotes()
        {
            // Arrange
            var app = new ToNote.App();
            app.InitializeComponent();

            var window = new MainView();
            window.Resources = app.Resources;
            window.Show();

            var viewmodel = window.DataContext as MainViewModel;

            viewmodel.Notes.Add(new Note("Test1"));
            viewmodel.Notes.Add(new Note("Test2"));

            window.UpdateLayout();


            // Act

            var noteView1 = VisualTreeService.FindChild<NoteView>(window, x => (x.DataContext as Note).Name == "Test1");
            var noteView2 = VisualTreeService.FindChild<NoteView>(window, x => (x.DataContext as Note).Name == "Test2");

            var data = new DataObject();
            data.SetData("Source", noteView1);

            var point = noteView2.TransformToAncestor(window)
                              .Transform(new Point(0, 0));

            var constructors = typeof(DragEventArgs).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            var dragArgObj = constructors[0].Invoke(new object[] { data, DragDropKeyStates.LeftMouseButton, DragDropEffects.Move, noteView2, point });

            var dragArgs = dragArgObj as DragEventArgs;
            dragArgs.RoutedEvent = DragDrop.DropEvent;
            dragArgs.Source = noteView2;


            noteView2.RaiseEvent(dragArgs);


            // Assert

            var noteViewTest = VisualTreeService.FindChild<NoteView>(window);

            (noteViewTest.DataContext as Note).Name.ShouldBe("Test2");

            window.Close();

            app.Shutdown(0);
        }
    }
}
