using ScriptCs.Contracts;
using System.Text;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public class NullScriptLibraryComposer : IScriptLibraryComposer
    {
        public void Compose(string workingDirectory, StringBuilder builder = null)
        {
        }

        public string ScriptLibrariesFile
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == string.Empty); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return string.Empty;
            }
        }
    }
}
