using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public interface IShebangLineProcessor : ILineProcessor
    {
    }

    public class ShebangLineProcessor : DirectiveLineProcessor, IShebangLineProcessor
    {
        protected override string DirectiveName
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @"!/usr/bin/env"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "!/usr/bin/env";
            }
        }

        protected override bool ProcessLine(IFileParser parser, FileParserContext context, string line)
        {
            #region CodeContracts 
            Contract.Ensures(Contract.Result<System.Boolean>() == true); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return true;
        }
    }
}