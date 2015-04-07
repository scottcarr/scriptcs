using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public class ScriptHostFactory : IScriptHostFactory
    {
        public IScriptHost CreateScriptHost(IScriptPackManager scriptPackManager, string[] scriptArgs)
        {
            #region CodeContracts 
            Contract.Assume(scriptPackManager != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return new ScriptHost(scriptPackManager, new ScriptEnvironment(scriptArgs));
        }
    }
}