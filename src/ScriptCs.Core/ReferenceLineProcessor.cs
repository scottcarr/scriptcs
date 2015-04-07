using System;
using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public interface IReferenceLineProcessor : ILineProcessor
    {
    }

    public class ReferenceLineProcessor : DirectiveLineProcessor, IReferenceLineProcessor
    {
        private readonly IFileSystem _fileSystem;

        public ReferenceLineProcessor(IFileSystem fileSystem)
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
                Contract.Ensures(Contract.Result<System.String>() == @"r"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "r";
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

            Guard.AgainstNullArgument("context", context);

            var argument = GetDirectiveArgument(line);
            var assemblyPath = Environment.ExpandEnvironmentVariables(argument);

            var referencePath = _fileSystem.GetFullPath(assemblyPath);
            var referencePathOrName = _fileSystem.FileExists(referencePath) ? referencePath : argument;

            if (!string.IsNullOrWhiteSpace(referencePathOrName) && !context.References.Contains(referencePathOrName))
            {
                context.References.Add(referencePathOrName);
            }

            return true;
        }

        [ContractInvariantMethod]
        private void ReferenceLineProcessorObjectInvariantMethod()
        {
            Contract.Invariant(this._fileSystem != null); // Suggested By ReviewBot 
        }
    }
}