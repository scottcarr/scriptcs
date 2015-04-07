using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs.ReplCommands
{
    public class VarsCommand : IReplCommand
    {
        public string CommandName
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @"vars"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "vars";
            }
        }

        public string Description
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @"Displays a list of variables defined within the REPL, along with their types and values."); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "Displays a list of variables defined within the REPL, along with their types and values.";
            }
        }

        public object Execute(IRepl repl, object[] args)
        {
            #region CodeContracts 
            Contract.Assume(repl != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("repl", repl);

            var replEngine = repl.ScriptEngine as IReplEngine;
            return replEngine != null ? replEngine.GetLocalVariables(repl.ScriptPackSession) : null;
        }
    }
}