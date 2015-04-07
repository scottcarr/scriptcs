using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs.ReplCommands
{
    public class ClearCommand : IReplCommand
    {
        private readonly IConsole _console;

        public string Description
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(this._console != null); // Suggested By ReviewBot 
                Contract.Ensures(Contract.Result<System.String>() == @"Clears the console window."); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "Clears the console window.";
            }
        }

        public ClearCommand(IConsole console)
        {
            #region CodeContracts 
            Contract.Requires(console != null); // Suggested By ReviewBot 
            Contract.Ensures(console == this._console); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("console", console);

            _console = console;
        }

        public string CommandName
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(this._console != null); // Suggested By ReviewBot 
                Contract.Ensures(Contract.Result<System.String>() == @"clear"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "clear";
            }
        }

        public object Execute(IRepl repl, object[] args)
        {
            #region CodeContracts 
            Contract.Ensures(this._console != null); // Suggested By ReviewBot 
            Contract.Ensures(Contract.Result<System.Object>() == null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            _console.Clear();
            return null;
        }

        [ContractInvariantMethod]
        private void ClearCommandObjectInvariantMethod()
        {
            Contract.Invariant(this._console != null); // Suggested By ReviewBot 
        }
    }
}