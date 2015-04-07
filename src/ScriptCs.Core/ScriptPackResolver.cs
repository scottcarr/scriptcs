using System.Collections.Generic;
using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public class ScriptPackResolver : IScriptPackResolver
    {
        private IEnumerable<IScriptPack> _scriptPacks;

        public ScriptPackResolver(IEnumerable<IScriptPack> scriptPacks)
        {
            #region CodeContracts 
            Contract.Ensures(scriptPacks == this._scriptPacks); // Suggested By ReviewBot 
            #endregion CodeContracts 

            _scriptPacks = scriptPacks;
        }

        public IEnumerable<IScriptPack> GetPacks()
        {
            #region CodeContracts 
            Contract.Ensures(Contract.Result<System.Collections.Generic.IEnumerable<ScriptCs.Contracts.IScriptPack>>() == this._scriptPacks); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return _scriptPacks;
        }
    }
}