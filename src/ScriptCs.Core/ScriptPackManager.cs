using System;
using System.Collections.Generic;
using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public class ScriptPackManager : IScriptPackManager
    {
        private readonly IDictionary<Type, IScriptPackContext> _contexts = new Dictionary<Type, IScriptPackContext>();

        public ScriptPackManager(IEnumerable<IScriptPackContext> contexts)
        {
            #region CodeContracts 
            Contract.Requires(contexts != null); // Suggested By ReviewBot 
            Contract.Ensures(this._contexts != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("contexts", contexts);

            foreach (var context in contexts)
            {
                _contexts[context.GetType()] = context;
            }
        }

        public TContext Get<TContext>() where TContext : IScriptPackContext
        {
            return (TContext)_contexts[typeof(TContext)];
        }
    }
}