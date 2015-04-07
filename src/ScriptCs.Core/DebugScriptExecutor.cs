using ScriptCs.Contracts;
using ScriptCs.Logging;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public class DebugScriptExecutor : ScriptExecutor
    {
        public DebugScriptExecutor(IFileSystem fileSystem, IFilePreProcessor filePreProcessor, IScriptEngine scriptEngine, ILog logger, IScriptLibraryComposer composer)
            : base(fileSystem, filePreProcessor, scriptEngine, logger, composer)
        {
            #region CodeContracts 
            Contract.Requires(fileSystem != null); // Suggested By ReviewBot 
            Contract.Ensures(this.FileSystem != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

        }
    }
}
