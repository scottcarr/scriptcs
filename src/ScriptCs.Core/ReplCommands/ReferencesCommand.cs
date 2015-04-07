using System.Linq;
using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs.ReplCommands
{
    public class ReferencesCommand : IReplCommand
    {
        public string CommandName
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @"references"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "references";
            }
        }

        public string Description
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @"Displays a list of assemblies referenced from the REPL context."); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "Displays a list of assemblies referenced from the REPL context.";
            }
        }

        public object Execute(IRepl repl, object[] args)
        {
            #region CodeContracts 
            Contract.Assume(repl != null); // Suggested By ReviewBot 
            Contract.Assume((((ScriptCs.Contracts.IScriptExecutor)repl).References == null || ((ScriptCs.Contracts.AssemblyReferences)((ScriptCs.Contracts.IScriptExecutor)repl).References).Assemblies != null)); // Suggested By ReviewBot 
            Contract.Assume((((ScriptCs.Contracts.IScriptExecutor)repl).References == null || ((ScriptCs.Contracts.AssemblyReferences)((ScriptCs.Contracts.IScriptExecutor)repl).References).Paths != null)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("repl", repl);

            return repl.References != null
                ? repl.References.Assemblies.Select(x => x.FullName).Union(repl.References.Paths)
                : null;
        }
    }
}