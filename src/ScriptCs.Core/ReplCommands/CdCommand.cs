using System.IO;
using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs.ReplCommands
{
    public class CdCommand : IReplCommand
    {
        public string Description
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @"Changes the working directory to the path provided."); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "Changes the working directory to the path provided.";
            }
        }

        public string CommandName
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @"cd"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "cd";
            }
        }

        public object Execute(IRepl repl, object[] args)
        {
            #region CodeContracts 
            Contract.Ensures(Contract.Result<System.Object>() == null); // Suggested By ReviewBot 
            Contract.Assume(((args == null || args.Length == 0) || repl != null)); // Suggested By ReviewBot 
            Contract.Assume(((args == null || args.Length == 0) || ((ScriptCs.Contracts.IScriptExecutor)repl).FileSystem != null)); // Suggested By ReviewBot 
            Contract.Assume(((args == null || args.Length == 0) || ((ScriptCs.Contracts.IFileSystem)((ScriptCs.Contracts.IScriptExecutor)repl).FileSystem).CurrentDirectory != null)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("repl", repl);

            if (args == null || args.Length == 0)
            {
                return null;
            }

            var path = args[0].ToString();

            repl.FileSystem.CurrentDirectory = Path.GetFullPath(Path.Combine(repl.FileSystem.CurrentDirectory, path));

            return null;
        }
    }
}