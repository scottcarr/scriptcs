using System.Linq;
using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs.ReplCommands
{
    public class UsingsCommand : IReplCommand
    {
        public string Description
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @"Displays a list of namespaces imported into REPL context."); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "Displays a list of namespaces imported into REPL context.";
            }
        }

        public string CommandName
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @"usings"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "usings";
            }
        }

        public object Execute(IRepl repl, object[] args)
        {
            #region CodeContracts 
            Contract.Assume(repl != null); // Suggested By ReviewBot 
            Contract.Assume((((repl.ScriptPackSession == null || repl.ScriptPackSession.Namespaces == null) || !(System.Linq.Enumerable.Any(repl.ScriptPackSession.Namespaces))) || ((ScriptCs.Contracts.IScriptExecutor)repl).Namespaces != null)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("repl", repl);

            var namespaces = repl.Namespaces;

            if (repl.ScriptPackSession == null || repl.ScriptPackSession.Namespaces == null || !repl.ScriptPackSession.Namespaces.Any())
                return namespaces;

            return namespaces.Union(repl.ScriptPackSession.Namespaces).OrderBy(x => x);
        }
    }
}