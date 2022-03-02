using System;
using System.Windows.Input;

namespace IDCA.Mvvm
{
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T?>? _execute;
        private readonly Func<T?, bool>? _canExecute;

        public event EventHandler? CanExecuteChanged;

        public DelegateCommand(Action<T?> execute, Func<T?, bool>? canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            if (canExecute != null)
            {
                _canExecute = canExecute;
            }
        }

        public DelegateCommand(Action<T?> execute) : this(execute, null)
        {
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke((T)parameter) ?? false;
        }

        public void Execute(object? parameter)
        {
            _execute?.Invoke((T)parameter);
        }

    }
}
