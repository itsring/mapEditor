using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MapEditor.Command
{
    public class RelayCommand<T> : ICommand
    {
        Action<T> _execute;
        Predicate<T> _predicate;

        public RelayCommand(Action<T> execute) : this(execute, null) { }
        public RelayCommand(Action<T> execute, Predicate<T> predicate)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            _execute = execute;
            _predicate = predicate;
        }

        public bool CanExecute(object parameter)
        {
            return _predicate == null ? true : _predicate.Invoke((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute.Invoke((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
