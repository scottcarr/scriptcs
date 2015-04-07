using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public abstract class ScriptLibraryWrapper
    {
        private static IScriptHost _scriptHost;

        internal static IScriptHost ScriptHost
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<ScriptCs.Contracts.IScriptHost>() != null); // Suggested By ReviewBot 
                Contract.Ensures(Contract.Result<ScriptCs.Contracts.IScriptHost>() == ScriptCs.ScriptLibraryWrapper._scriptHost); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return _scriptHost;
            }
        }

        public static void SetHost(IScriptHost scriptHost)
        {
            _scriptHost = scriptHost;
        }

        public static T Require<T>() where T : IScriptPackContext
        {
            return _scriptHost.Require<T>();
        }

        public static IScriptEnvironment Env
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<ScriptCs.Contracts.IScriptEnvironment>() == ScriptCs.ScriptLibraryWrapper._scriptHost.Env); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return _scriptHost.Env;
            }
        }
    }
}
