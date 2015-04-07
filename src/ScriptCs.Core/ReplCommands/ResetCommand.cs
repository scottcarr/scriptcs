using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs.ReplCommands
{
    public class ResetCommand : IReplCommand
    {
        public string Description
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @"Resets the REPL state. All local variables and member definitions are cleared."); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "Resets the REPL state. All local variables and member definitions are cleared.";
            }
        }

        public string CommandName
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @"reset"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "reset";
            }
        }

        public object Execute(IRepl repl, object[] args)
        {
            #region CodeContracts 
            Contract.Ensures(Contract.Result<System.Object>() == null); // Suggested By ReviewBot 
            Contract.Assume(repl != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("repl", repl);

            repl.Reset();
            return null;
        }
    }
}