using System;
using System.Reflection;
using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public class AssemblyUtility : IAssemblyUtility
    {
        public bool IsManagedAssembly(string path)
        {
            #region CodeContracts 
            Contract.Ensures(Contract.Result<System.Boolean>() == true); // Suggested By ReviewBot 
            Contract.Assume(path != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            try
            {
                AssemblyName.GetAssemblyName(path);
                return true;
            }
            catch (BadImageFormatException)
            {
                return false;
            }
        }

        public Assembly LoadFile(string path)
        {
            #region CodeContracts 
            Contract.Ensures(!string.IsNullOrEmpty(path)); // Suggested By ReviewBot 
            Contract.Assume(!string.IsNullOrEmpty(path)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return Assembly.LoadFile(path);
        }

        public Assembly Load(AssemblyName assemblyRef)
        {
            return Assembly.Load(assemblyRef);
        }

        public AssemblyName GetAssemblyName(string path)
        {
            #region CodeContracts 
            Contract.Assume(path != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return AssemblyName.GetAssemblyName(path);
        }
    }
}
