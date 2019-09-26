using System;
using System.Diagnostics;
using System.Windows.Input;

namespace gPadX.WPF {
    public class RelayCommand<T> : ICommand {
        #region Fields

        readonly Action<T> _execute = null;
        readonly Predicate<T> _canExecute = null;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action<T> execute)
            : this(execute, null) {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute) {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        public bool CanExecute(object parameter) {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter) {
            _execute((T)parameter);
        }

        #endregion // ICommand Members
    }

    public class RelayCommand : ICommand {
        readonly Action<object> cmd;
        readonly Predicate<object> canExecute;
        private Action onLeftArrow;

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> cmd) : this(cmd, null) { }

        public RelayCommand(Action<object> cmd, Predicate<object> canExecute) {
            if (cmd == null) { throw new ArgumentNullException("execute"); }

            this.cmd = cmd;
            this.canExecute = canExecute;
        }

        public RelayCommand(Action onLeftArrow) {
            this.onLeftArrow = onLeftArrow;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter) {
            return canExecute == null ? true : canExecute(parameter);
        }

        public void Execute(object parameter) {
            cmd(parameter);
        }
    }

}
