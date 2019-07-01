using System;
using System.Windows.Input;

namespace SocketApp.ChatRoom.Helper
{
    public class RelayCommand : ICommand
    {
        #region Members

        private readonly Func<Boolean> CanExecuteField;
        private readonly Action ExecuteField;

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

            this.ExecuteField = execute;
            this.CanExecuteField = canExecute;
        }

        #endregion Constructors

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (this.CanExecuteField != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (this.CanExecuteField != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public bool CanExecute(Object parameter)
        {
            return this.CanExecuteField == null ? true : this.CanExecuteField();
        }

        public void Execute(Object parameter)
        {
            this.ExecuteField();
        }

        #endregion ICommand Members
    }
}