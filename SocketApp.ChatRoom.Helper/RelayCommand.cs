using System;
using System.Diagnostics;
using System.Windows.Input;

namespace SocketApp.ChatRoom.Helper
{
    public class RelayCommand : ICommand
    {
        #region Members

        private readonly Func<Boolean> _canExecute;
        private readonly Action _execute;

        #endregion Members

        #region Constructors

        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action execute, Func<Boolean> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            this._execute = execute;
            this._canExecute = canExecute;
        }

        #endregion Constructors

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (this._canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (this._canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        [DebuggerStepThrough]
        public Boolean CanExecute(Object parameter)
        {
            return this._canExecute == null ? true : this._canExecute();
        }

        public void Execute(Object parameter)
        {
            this._execute();
        }

        #endregion ICommand Members
    }
}