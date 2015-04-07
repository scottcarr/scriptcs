using System.Linq;
using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs.ReplCommands
{
    using System;
    using System.Globalization;

    public class AliasCommand : IReplCommand
    {
        private readonly IConsole _console;

        public AliasCommand(IConsole console)
        {
            // SCOTT: VALID NEW CONTRACTS
            #region CodeContracts 
            Contract.Requires(console != null); // Suggested By ReviewBot 
            Contract.Ensures(console == this._console); // Suggested By ReviewBot 
            #endregion CodeContracts 

            // SCOTT: in other IReplCommands there's a null check here, but here it's missing
            _console = console;
        }

        public string Description
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(this._console != null); // Suggested By ReviewBot 
                Contract.Ensures(Contract.Result<System.String>() == @"Allows you to alias a command with a custom name"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "Allows you to alias a command with a custom name";
            }
        }

        public string CommandName
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(this._console != null); // Suggested By ReviewBot 
                Contract.Ensures(Contract.Result<System.String>() == @"alias"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "alias";
            }
        }

        public object Execute(IRepl repl, object[] args)
        {
            #region CodeContracts 
            Contract.Ensures(this._console != null); // Suggested By ReviewBot 
            Contract.Ensures(Contract.Result<System.Object>() == null); // Suggested By ReviewBot 
            Contract.Assume((args != null || this._console != null)); // Suggested By ReviewBot 
            Contract.Assume((args.Length == 2 || this._console != null)); // Suggested By ReviewBot 
            Contract.Assume(((args == null || args.Length != 2) || repl != null)); // Suggested By ReviewBot 
            Contract.Assume(((args == null || args.Length != 2) || repl.Commands != null)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("repl", repl);

            if (args == null || args.Length != 2)
            {
                _console.WriteLine("You must specifiy the command name and alias, e.g. :alias \"clear\" \"cls\"");
                return null;
            }

            var commandName = args[0].ToString();
            var alias = args[1].ToString();

            if (repl.Commands.Any(x => string.Equals(x.Key, alias, StringComparison.InvariantCultureIgnoreCase)))
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "\"{0}\" cannot be used as an alias since it is the name of an existing command.",
                    alias);

                _console.WriteLine(message);
                return null;
            }

            IReplCommand command;
            if (!repl.Commands.TryGetValue(commandName, out command))
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture, "There is no command named or aliased \"{0}\".", alias);

                _console.WriteLine(message);
                return null;
            }

            repl.Commands[alias] = command;
            _console.WriteLine(string.Format("Aliased \"{0}\" as \"{1}\".", commandName, alias));

            return null;
        }

        [ContractInvariantMethod]
        private void AliasCommandObjectInvariantMethod()
        {
            Contract.Invariant(this._console != null); // Suggested By ReviewBot 
        }
    }
}