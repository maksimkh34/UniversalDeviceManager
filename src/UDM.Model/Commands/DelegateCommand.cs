using System.Windows.Input;

namespace UDM.Model.Commands
{
    public class DelegateCommand(Action<object> openAction, Predicate<object> canExecutePredicate) : ICommand
    {
        public static bool DefaultCanExecute(object param) => true;

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