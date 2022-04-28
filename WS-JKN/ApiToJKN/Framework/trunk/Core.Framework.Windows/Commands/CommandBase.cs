///
/// Copyright(C) MixModes Inc. 2010
/// 

namespace Core.Framework.Windows.Commands
{
    using System;
    using System.Windows.Input;

    using Core.Framework.Windows.Utilities;

    /// <summary>
    ///     Base class for commands
    /// </summary>
    public class CommandBase : ICommand
    {
        // Private members

        #region Fields

        private readonly Predicate<object> _canExecute;

        private readonly Action<object> _execute;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandBase" /> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        public CommandBase(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandBase" /> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        /// <param name="canExecute">The can execute.</param>
        public CommandBase(Action<object> execute, Predicate<object> canExecute)
        {
            Validate.NotNull(execute, "execute");
            this._execute = execute;
            this._canExecute = canExecute;
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when changes occur that affect whether or not the command should execute
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command.
        ///     If the command does not require data to be passed, this object can be set to null.
        /// </param>
        /// <returns>
        ///     true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return this._canExecute != null ? this._canExecute(parameter) : true;
        }

        /// <summary>
        ///     Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command.
        ///     If the command does not require data to be passed, this object can be set to null.
        /// </param>
        public void Execute(object parameter)
        {
            this._execute(parameter);
        }

        #endregion
    }
}