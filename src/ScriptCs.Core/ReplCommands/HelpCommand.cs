using System.Linq;
using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs.ReplCommands
{
    public class HelpCommand : IReplCommand
    {
        private readonly IConsole _console;

        public HelpCommand(IConsole console)
        {
            // SCOTT: VALID NEW CONTRACTS
            #region CodeContracts 
            Contract.Requires(console != null); // Suggested By ReviewBot 
            Contract.Ensures(console == this._console); // Suggested By ReviewBot 
            #endregion CodeContracts 

            // SCOTT: here would be a null check
            _console = console;
        }

        public string Description
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(this._console != null); // Suggested By ReviewBot 
                Contract.Ensures(Contract.Result<System.String>() == @"Shows this help."); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "Shows this help.";
            }
        }

        public string CommandName
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(this._console != null); // Suggested By ReviewBot 
                Contract.Ensures(Contract.Result<System.String>() == @"help"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "help";
            }
        }

        public object Execute(IRepl repl, object[] args)
        {
            #region CodeContracts 
            Contract.Ensures(repl.Commands != null); // Suggested By ReviewBot 
            Contract.Ensures(this._console != null); // Suggested By ReviewBot 
            Contract.Ensures(Contract.Result<System.Object>() == null); // Suggested By ReviewBot 
            Contract.Assume(repl != null); // Suggested By ReviewBot 
            Contract.Assume(repl.Commands != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("repl", repl);

            _console.WriteLine("The following commands are available in the REPL:");
            foreach (var command in repl.Commands.OrderBy(x => x.Key))
            {
                _console.WriteLine(string.Format(":{0,-15}{1,10}", command.Key, command.Value.Description));
            }

            return null;
        }

        [ContractInvariantMethod]
        private void HelpCommandObjectInvariantMethod()
        {
            Contract.Invariant(this._console != null); // Suggested By ReviewBot 
        }
    }
}