using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MapEditor.Command
{
    internal class SimpleCommand : ICommand
    {
        #region Fields
        private Func<bool> canMethod;
        private Action method;
        #endregion
        #region Constructors
        public SimpleCommand(Action executeMethod, Func<bool> canExecuteMethod = null)
        {
            if (executeMethod == null)
                throw new ArgumentNullException(nameof(executeMethod));

            method = executeMethod;
            canMethod = canExecuteMethod ?? (() => true);
        }

        #endregion

        #region Methods
        public bool CanExecute()
        {
            return canMethod();
        }

        public void Execute()
        {
            method();
        }
        #endregion

        #region ICommand Implementation
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return CanExecute();
        }

        public void Execute(object parameter)
        {
            Execute();
        }
        #endregion
    }
}
