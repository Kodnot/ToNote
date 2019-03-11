namespace ToNote.Logic
{
    using System;
    using System.Windows.Input;

    public class RelayCommand<T> : ICommand
    {
        readonly Action<T> _execute;

        readonly Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new NullReferenceException("Action<object> execute null");

            _canExecute = canExecute;
        }

        public RelayCommand(Action<T> execute) : this(execute, null)
        {

        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute.Invoke((T)parameter);
        }
    }

    public class RelayCommand : ICommand
    {
        readonly Action _execute;

        readonly Predicate<object> _canExecute;

        public RelayCommand(Action execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new NullReferenceException("Action<object>_execute is null");
            _canExecute = canExecute;
        }

        public RelayCommand(Action execute) : this(execute, null)
        {

        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute.Invoke();
        }
    }
}
