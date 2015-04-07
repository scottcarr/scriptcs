using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public interface IUsingLineProcessor : ILineProcessor
    {
    }

    public class UsingLineProcessor : IUsingLineProcessor
    {
        private const string UsingString = "using ";

        public bool ProcessLine(IFileParser parser, FileParserContext context, string line, bool isBeforeCode)
        {
            #region CodeContracts 
            Contract.Assume(line != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("context", context);

            if (!IsUsingLine(line))
            {
                return false;
            }

            var @namespace = GetNamespace(line);
            if (!context.Namespaces.Contains(@namespace))
            {
                context.Namespaces.Add(@namespace);
            }

            return true;
        }

        private static bool IsUsingLine(string line)
        {
            return line.Trim(' ').StartsWith(UsingString) && !line.Contains("{") && line.Contains(";") && !line.Contains("=");
        }

        private static string GetNamespace(string line)
        {
            #region CodeContracts 
            Contract.Ensures(Contract.Result<System.String>() != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return line.Trim(' ')
                .Replace(UsingString, string.Empty)
                .Replace("\"", string.Empty)
                .Replace(";", string.Empty);
        }
    }
}