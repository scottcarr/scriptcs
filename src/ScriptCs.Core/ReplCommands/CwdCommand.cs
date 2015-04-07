using System;
using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs.ReplCommands
{
    public class CwdCommand : IReplCommand
    {
        private readonly IConsole _console;

        public CwdCommand(IConsole console)
        {
            #region CodeContracts 
            Contract.Requires(console != null); // Suggested By ReviewBot 
            Contract.Ensures(console == this._console); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("console", console);

            _console = console;
        }

        public string Description
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(this._console != null); // Suggested By ReviewBot 
                Contract.Ensures(Contract.Result<System.String>() == @"Displays the current working directory."); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "Displays the current working directory.";
            }
        }

        public string CommandName
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(this._console != null); // Suggested By ReviewBot 
                Contract.Ensures(Contract.Result<System.String>() == @"cwd"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "cwd";
            }
        }

        public object Execute(IRepl repl, object[] args)
        {
            #region CodeContracts 
            Contract.Ensures(this._console != null); // Suggested By ReviewBot 
            Contract.Ensures(repl.FileSystem != null); // Suggested By ReviewBot 
            Contract.Ensures(Contract.Result<System.Object>() == null); // Suggested By ReviewBot 
            Contract.Assume(repl != null); // Suggested By ReviewBot 
            Contract.Assume(repl.FileSystem != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("repl", repl);

            var dir = repl.FileSystem.CurrentDirectory;

            var originalColor = _console.ForegroundColor;
            _console.ForegroundColor = ConsoleColor.Yellow;
            try
            {
                _console.WriteLine(dir);
            }
            finally
            {
                _console.ForegroundColor = originalColor;
            }

            return null;
        }

        [ContractInvariantMethod]
        private void CwdCommandObjectInvariantMethod()
        {
            Contract.Invariant(this._console != null); // Suggested By ReviewBot 
        }
    }
}
