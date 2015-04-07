using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Diagnostics.Contracts;

namespace ScriptCs.Extensions
{
    internal static class MethodInfoExtensions
    {
        internal static IEnumerable<ParameterInfo> GetParametersWithoutExtensions(this MethodInfo method)
        {
            #region CodeContracts 
            Contract.Ensures(Contract.Result<System.Collections.Generic.IEnumerable<System.Reflection.ParameterInfo>>() != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            IEnumerable<ParameterInfo> methodParams = method.GetParameters();
            if (method.IsDefined(typeof(ExtensionAttribute), false))
            {
                methodParams = methodParams.Skip(1);
            }

            return methodParams;
        }
    }
}