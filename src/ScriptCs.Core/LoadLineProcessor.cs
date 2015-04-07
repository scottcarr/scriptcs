using System;

using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public interface ILoadLineProcessor : ILineProcessor
    {
    }

    public class LoadLineProcessor : DirectiveLineProcessor, ILoadLineProcessor
    {
        private readonly IFileSystem _fileSystem;

        public LoadLineProcessor(IFileSystem fileSystem)
        {
            // SCOTT: VALID NEW CONTRACTS
            #region CodeContracts 
            Contract.Requires(fileSystem != null); // Suggested By ReviewBot 
            Contract.Ensures(fileSystem == this._fileSystem); // Suggested By ReviewBot 
            #endregion CodeContracts 

            _fileSystem = fileSystem;
        }

        protected override string DirectiveName
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @"load"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "load";
            }
        }

        protected override BehaviorAfterCode BehaviorAfterCode
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<ScriptCs.Contracts.BehaviorAfterCode>() == ScriptCs.Contracts.BehaviorAfterCode.Throw); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return BehaviorAfterCode.Throw;
            }
        }

        protected override bool ProcessLine(IFileParser parser, FileParserContext context, string line)
        {
            #region CodeContracts 
            Contract.Ensures(this._fileSystem != null); // Suggested By ReviewBot 
            Contract.Ensures(Contract.Result<System.Boolean>() == true); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("parser", parser);

            var argument = GetDirectiveArgument(line);
            var filePath = Environment.ExpandEnvironmentVariables(argument);

            var fullPath = _fileSystem.GetFullPath(filePath); // SCOTT: _fileSystem was never null checked
            if (!string.IsNullOrWhiteSpace(fullPath))
            {
                parser.ParseFile(fullPath, context);
            }

            return true;
        }

        [ContractInvariantMethod]
        private void LoadLineProcessorObjectInvariantMethod()
        {
            Contract.Invariant(this._fileSystem != null); // Suggested By ReviewBot 
        }
    }
}