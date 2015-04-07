using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Diagnostics.Contracts;

namespace ScriptCs.Extensions
{
    internal static class TypeExtensions
    {
        internal static IEnumerable<MethodInfo> GetExtensionMethods(this Type type)
        {
            #region CodeContracts 
            Contract.Ensures(Contract.Result<System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo>>() != null); // Suggested By ReviewBot 
            Contract.Ensures(type.Assembly != null); // Suggested By ReviewBot 
            Contract.Ensures(0 <= System.Linq.Enumerable.Count(Contract.Result<System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo>>())); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return type.Assembly.GetExportedTypes().Where(x => !x.IsGenericType && !x.IsNested && x.IsSealed).
                SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.Public)).Where(x => x.IsDefined(typeof(ExtensionAttribute), false)).
                Where(x => x.GetParameters()[0].ParameterType == type);
        }

        internal static IEnumerable<MethodInfo> GetAllMethods(this Type type)
        {
            #region CodeContracts 
            Contract.Ensures(Contract.Result<System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo>>() != null); // Suggested By ReviewBot 
            Contract.Ensures(type.Assembly != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).
                    Where(m => !m.IsSpecialName).Union(type.GetExtensionMethods()).OrderBy(x => x.Name);
        }
    }
}