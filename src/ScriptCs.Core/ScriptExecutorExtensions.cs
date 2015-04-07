using System;
using System.Linq;

using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public static class ScriptExecutorExtensions
    {
        public static void ImportNamespaces(this IScriptExecutor executor, params Type[] typesFromReferencedAssembly)
        {
            #region CodeContracts 
            Contract.Requires(executor != null); // Suggested By ReviewBot 
            Contract.Requires(typesFromReferencedAssembly != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("executor", executor);
            Guard.AgainstNullArgument("typesFromReferencedAssembly", typesFromReferencedAssembly);

            var namespaces = typesFromReferencedAssembly.Select(t => t.Namespace);
            executor.ImportNamespaces(namespaces.ToArray());
        }

        public static void ImportNamespace<T>(this IScriptExecutor executor)
        {
            // SCOTT: VALID NEW CONTRACTS
            // SCOTT: I'm not 100% sure about this one, can this be null?  I think it can in some edge case, right?
            #region CodeContracts 
            Contract.Requires(executor != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            executor.ImportNamespaces(typeof(T));
        }

        public static void AddReferences(this IScriptExecutor executor, params Type[] typesFromReferencedAssembly)
        {
            #region CodeContracts 
            Contract.Requires(executor != null); // Suggested By ReviewBot 
            Contract.Requires(typesFromReferencedAssembly != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("executor", executor);
            Guard.AgainstNullArgument("typeFromReferencedAssembly", typesFromReferencedAssembly);

            var paths = typesFromReferencedAssembly.Select(t => t.Assembly.Location);

            executor.AddReferences(paths.ToArray());
        }

        public static void AddReference<T>(this IScriptExecutor executor)
        {
            // SCOTT: VALID NEW CONTRACTS
            // SCOTT: I'm not 100% sure about this one, can this be null?  I think it can in some edge case, right?
            #region CodeContracts 
            Contract.Requires(executor != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            executor.AddReferences(typeof(T));
        }

        public static void AddReferenceAndImportNamespaces(this IScriptExecutor executor, Type[] types)
        {
            // SCOTT: VALID NEW CONTRACTS
            // SCOTT: I'm not 100% sure about this one, can this be null?  I think it can in some edge case, right?
            #region CodeContracts 
            Contract.Requires(executor != null); // Suggested By ReviewBot 
            Contract.Requires(types != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            executor.AddReferences(types);
            executor.ImportNamespaces(types);
        }
    }
}
