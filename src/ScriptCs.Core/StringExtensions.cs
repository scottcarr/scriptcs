using System;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public static class StringExtensions
    {
        public static string DefineTrace(this string code)
        {
            #region CodeContracts 
            Contract.Ensures(Contract.Result<System.String>() != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return string.Format("#define TRACE{0}{1}", Environment.NewLine, code);
        }

        public static string UndefineTrace(this string code)
        {
            #region CodeContracts 
            Contract.Ensures(Contract.Result<System.String>() != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return string.Format("#undef TRACE{0}{1}", Environment.NewLine, code);
        }
    }
}