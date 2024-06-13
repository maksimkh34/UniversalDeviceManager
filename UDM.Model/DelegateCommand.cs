using System.Windows.Input;

namespace UDM.Model
{
    public class DelegateCommand(Action<object> openAction, Predicate<object> canExecutePredicate) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return canExecutePredicate(parameter!);
        }

        public void Execute(object? parameter)
        {
            openAction(parameter!);
        }
    }
}
