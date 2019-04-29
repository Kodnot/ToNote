namespace ToNote.Controls
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using ToNote.Interfaces;
    using ToNote.Logic;
    using ToNote.Models;

    public class TodoControl : ContentControl, IExtendedTextBoxControl
    {
        public TodoControl(Todo todo)
        {
            this.Todo = todo;
            this.Content = todo;

            this.Loaded += (s, e) =>
            {
                //Gets the DataTemplate defined for the Todo objects.
                var template = this.ContentTemplate;

                if (template == null) return;

                //Looks for an ExtendedRichTextBox with an x:Name="rtb" attribute from the logical children of the generated ContentPresenter which has the DataTemplate applied to it.
                if (VisualTreeHelper.GetChildrenCount(this) == 0)
                    return;

                var rtb = (ExtendedRichTextBox)template.FindName("rtb", (FrameworkElement)VisualTreeHelper.GetChild(this, 0));

                if (rtb == null) return;

                extendedRTB = rtb;

                //If an ExtendedRichTextBox is found, hooks to the BackspacePressedWhileEmpty event.
                rtb.BackspacePressedWithAltShiftModifiers += (o, a) =>
                {
                    this.BackspacePressedWithAltShiftModifiers?.Invoke(this, new RoutedEventArgs());
                };

                rtb.TextChanged += (o, a) =>
                {
                    TextChanged?.Invoke(o, a);
                };

                rtb.Drop += (o, a) =>
                {
                    Drop?.Invoke(o, a);
                };
            };

            this.AllowDrop = true;
        }

        public TodoControl() { }

        public Todo Todo { get; set; }

        public event RoutedEventHandler BackspacePressedWithAltShiftModifiers;

        public event TextChangedEventHandler TextChanged;

        public new event DragEventHandler Drop;

        public bool? Initializing => extendedRTB?.Initializing;

        private ExtendedRichTextBox extendedRTB;

        public string CurrentFile => extendedRTB?.CurrentFile;

        public TextRange TextRange => extendedRTB?.TextRange;

        public void ReadFromFile(string file)
        {
            if (!this.IsLoaded)
            {
                this.Loaded += (s, e) => extendedRTB?.ReadFromFile(file);
            }
            else
                extendedRTB?.ReadFromFile(file);
        }

        public void SetKeyboardFocus()
        {
            if (extendedRTB == null) return;

            Keyboard.Focus(extendedRTB);
        }

        public TodoControl SetKeyboardFocusAfterLoaded()
        {
            this.Loaded += (s, e) =>
            {
                this.SetKeyboardFocus();
            };

            return this;
        }

        public void TrackKeyword(string keyword, Action action)
        {
            if (!this.IsLoaded)
            {
                this.Loaded += (s, e) => extendedRTB?.TrackKeyword(keyword, action);
            }
            else
                extendedRTB?.TrackKeyword(keyword, action);
        }

        public static readonly DependencyProperty ListTodosProperty = DependencyProperty.Register("ListTodo",
           typeof(Todo), typeof(TodoControl), new FrameworkPropertyMetadata(null)
           {    PropertyChangedCallback = (s, e) =>
               {
                   var control = (TodoControl)s;

                   if (!(e.NewValue is Todo newTodoValue) || newTodoValue == null) return;

                   control.Todo = newTodoValue;
                   control.Content = newTodoValue;
                   var rtbx = new ExtendedRichTextBox();
                   rtbx.ReadFromFile(newTodoValue.FileName);
                   control.extendedRTB = rtbx;
                   //control.ReadFromFile(((Todo)control.DataContext).FileName);

                   
                   control.Loaded += (sender, ev) =>
                   {
                       //Gets the DataTemplate defined for the Todo objects.
                       var template = control.ContentTemplate;

                       if (template == null) return;

                       //Looks for an ExtendedRichTextBox with an x:Name="rtb" attribute from the logical children of the generated ContentPresenter which has the DataTemplate applied to it.
                       var rtb = (ExtendedRichTextBox)template.FindName("rtb", (FrameworkElement)VisualTreeHelper.GetChild(control, 0));
                       control.ReadFromFile(((Todo)control.DataContext).FileName);
                       if (rtb == null) return;

                       control.extendedRTB = rtb;

                       //If an ExtendedRichTextBox is found, hooks to the BackspacePressedWhileEmpty event.
                       rtb.BackspacePressedWithAltShiftModifiers += (o, a) =>
                       {
                           control.BackspacePressedWithAltShiftModifiers?.Invoke(control, new RoutedEventArgs());
                       };

                       rtb.TextChanged += (o, a) =>
                       {
                           control.TextChanged?.Invoke(o, a);
                       };

                       rtb.Drop += (o, a) =>
                       {
                           control.Drop?.Invoke(o, a);
                       };
                   };
                   control.InvalidateVisual();
               }
           });

        public Todo ListTodo
        {
            get => (Todo)GetValue(ListTodosProperty);
            set => SetValue(ListTodosProperty, value);
        }
    }
}
