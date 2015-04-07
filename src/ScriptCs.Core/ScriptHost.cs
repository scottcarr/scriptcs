using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public class ScriptHost : IScriptHost
    {
        private readonly IScriptPackManager _scriptPackManager;

        public ScriptHost(IScriptPackManager scriptPackManager, ScriptEnvironment environment)
        {
            #region CodeContracts 
            Contract.Requires(scriptPackManager != null); // Suggested By ReviewBot 
            Contract.Ensures(scriptPackManager == this._scriptPackManager); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("scriptPackManager", scriptPackManager);

            _scriptPackManager = scriptPackManager;
            Env = environment;
        }

        public IScriptEnvironment Env { get; private set; }

        public T Require<T>() where T : IScriptPackContext
        {
            #region CodeContracts 
            Contract.Ensures(this._scriptPackManager != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return _scriptPackManager.Get<T>();
        }

        [ContractInvariantMethod]
        private void ScriptHostObjectInvariantMethod()
        {
            Contract.Invariant(this._scriptPackManager != null); // Suggested By ReviewBot 
        }
    }
}
