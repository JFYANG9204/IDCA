using System;
using System.Windows.Input;

namespace IDCA.Mvvm
{
    public class DelegateCommand : ICommand
    {
        public Action<object?>? ExecuteAction;
        public Func<object?, bool>? CanExecuteFunc;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc(parameter);
        }

        public void Execute(object? parameter)
        {
            ExecuteAction?.Invoke(parameter);
        }
    }
}
