using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs.ReplCommands
{
    public class ExitCommand : IReplCommand
    {
        private readonly IConsole _console;

        public string Description
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(this._console != null); // Suggested By ReviewBot 
                Contract.Ensures(Contract.Result<System.String>() == @"Exits the REPL"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "Exits the REPL";
            }
        }

        public string CommandName
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(this._console != null); // Suggested By ReviewBot 
                Contract.Ensures(Contract.Result<System.String>() == @"exit"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "exit";
            }
        }

        public ExitCommand(IConsole console)
        {
            // SCOTT: VALID NEW CONTRACTS
            #region CodeContracts 
            Contract.Requires(console != null); // Suggested By ReviewBot 
            Contract.Ensures(console == this._console); // Suggested By ReviewBot 
            #endregion CodeContracts 

            // in other commands there's a null check here, but here it's missing
            _console = console;
        }

        public object Execute(IRepl repl, object[] args)
        {
            #region CodeContracts 
            Contract.Ensures(Contract.Result<System.Object>() == null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("repl", repl);

            var response = string.Empty;
            var responseIsValid = false;

            while (!responseIsValid)
            {
                _console.Write("Are you sure you wish to exit? (y/n): ");
                response = (_console.ReadLine() ?? string.Empty).ToLowerInvariant();
                responseIsValid = response == "y" || response == "n";
            }

            if (response == "y")
            {
                repl.Terminate();
            }

            return null;
        }

        [ContractInvariantMethod]
        private void ExitCommandObjectInvariantMethod()
        {
            Contract.Invariant(this._console != null); // Suggested By ReviewBot 
        }
    }
}
