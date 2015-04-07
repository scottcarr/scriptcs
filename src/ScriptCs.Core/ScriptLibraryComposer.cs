using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ScriptCs.Contracts;
using ScriptCs.Logging;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public class ScriptLibraryComposer : IScriptLibraryComposer
    {
        private readonly IFileSystem _fileSystem;
        private readonly IFilePreProcessor _preProcessor;
        private readonly IPackageContainer _packageContainer;
        private readonly IPackageAssemblyResolver _packageAssemblyResolver;
        private readonly ILog _logger;

        public ScriptLibraryComposer(IFileSystem fileSystem, IFilePreProcessor preProcessor, IPackageContainer packageContainer, IPackageAssemblyResolver packageAssemblyResolver, ILog logger)
        {
            #region CodeContracts 
            Contract.Ensures(fileSystem == this._fileSystem); // Suggested By ReviewBot 
            Contract.Ensures(preProcessor == this._preProcessor); // Suggested By ReviewBot 
            Contract.Ensures(packageContainer == this._packageContainer); // Suggested By ReviewBot 
            Contract.Ensures(packageAssemblyResolver == this._packageAssemblyResolver); // Suggested By ReviewBot 
            Contract.Ensures(logger == this._logger); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("fileSystem", fileSystem);
            Guard.AgainstNullArgument("preProcessor", preProcessor);
            Guard.AgainstNullArgument("packageContainer", packageContainer);
            Guard.AgainstNullArgument("packageAssemblyResolver", packageAssemblyResolver);
            Guard.AgainstNullArgument("logger", logger);

            _fileSystem = fileSystem;
            _preProcessor = preProcessor;
            _packageContainer = packageContainer;
            _packageAssemblyResolver = packageAssemblyResolver;
            _logger = logger;
        }

        internal string GetMainScript(IPackageObject package)
        {
            var content = package.GetContentFiles()
                .Where(f => f.EndsWith("main.csx", StringComparison.InvariantCultureIgnoreCase)).ToArray();

            string script = null;
            var count = content.Length;

            if (count == 1)
            {
                script = content[0];
            }
            else if (content.Count() > 1)
            {
                _logger.WarnFormat("Script Libraries in '{0}' ignored due to multiple Main files being present", package.FullName);
                return null;
            }

            if (script != null)
            {
                _logger.DebugFormat("Found main script: {0}", script);
            }

            return script;
        }

        public virtual string ScriptLibrariesFile
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @"ScriptLibraries.csx"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "ScriptLibraries.csx";
            }
        }

        public void Compose(string workingDirectory, StringBuilder builder = null)
        {
            #region CodeContracts 
            Contract.Assume((string.IsNullOrWhiteSpace(this.ScriptLibrariesFile) || workingDirectory != null)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            if (string.IsNullOrWhiteSpace(ScriptLibrariesFile))
            {
                return;
            }

            var packagesPath = Path.Combine(workingDirectory, _fileSystem.PackagesFolder);
            var packageScriptsPath = Path.Combine(packagesPath, ScriptLibrariesFile);
            if (!_fileSystem.DirectoryExists(packagesPath) || _fileSystem.FileExists(packageScriptsPath))
            {
                return;
            }

            if (builder == null)
            {
                builder = new StringBuilder();
            }

            var namespaces = new List<string>();
            var references = new List<string>();
            var packageReferences = _packageAssemblyResolver.GetPackages(workingDirectory);
            foreach (var reference in packageReferences)
            {
                ProcessPackage(packagesPath, reference, builder, references, namespaces);
            }

            foreach (var ns in namespaces)
            {
                builder.Insert(0, String.Format("using {0};{1}", ns, Environment.NewLine));
            }

            foreach (var reference in references)
            {
                builder.Insert(0, String.Format("#r {0}{1}", reference, Environment.NewLine));
            }

            _fileSystem.WriteToFile(packageScriptsPath, builder.ToString());
        }

        protected internal virtual void ProcessPackage(
            string packagesPath,
            IPackageReference reference,
            StringBuilder builder,
            List<string> references,
            List<string> namespaces)
        {
            #region CodeContracts 
            Contract.Requires(reference != null); // Suggested By ReviewBot 
            Contract.Ensures(this._logger != null); // Suggested By ReviewBot 
            Contract.Ensures(this._packageContainer != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("reference", reference);
            Guard.AgainstNullArgument("builder", builder);
            Guard.AgainstNullArgument("references", references);
            Guard.AgainstNullArgument("namespaces", namespaces);

            _logger.DebugFormat("Finding package:{0}", reference.PackageId);
            var package = _packageContainer.FindPackage(packagesPath, reference);

            if (package == null)
            {
                _logger.WarnFormat("Package missing: {0}", reference.PackageId);
                return;
            }

            _logger.Debug("Package found");
            var script = GetMainScript(package);
            if (script != null)
            {
                script = Path.Combine(packagesPath, string.Format("{0}.{1}", package.Id, package.TextVersion), script);
                _logger.Debug("Pre-processing script");
                var result = _preProcessor.ProcessFile(script);
                var fileWithoutExtension = Path.GetFileNameWithoutExtension(script);
                var classname = fileWithoutExtension.Substring(0, fileWithoutExtension.Length - 4);
                _logger.DebugFormat("Created Script Libary class: {0}", classname);
                var classBuilder = new StringBuilder();
                classBuilder.AppendFormat("public class {0} : ScriptCs.ScriptLibraryWrapper {{{1}", classname, Environment.NewLine);
                classBuilder.AppendLine(result.Code);
                classBuilder.AppendLine("}");
                var classDefinition = classBuilder.ToString();
                _logger.TraceFormat("Class definition:{0}{0}{1}", Environment.NewLine, classDefinition);
                builder.Append(classDefinition);
                references.AddRange(result.References);
                namespaces.AddRange(result.Namespaces);
            }
        }

        [ContractInvariantMethod]
        private void ScriptLibraryComposerObjectInvariantMethod()
        {
            Contract.Invariant(this._packageContainer != null); // Suggested By ReviewBot 
            Contract.Invariant(this._logger != null); // Suggested By ReviewBot 
            Contract.Invariant((string.IsNullOrWhiteSpace(this.ScriptLibrariesFile) || this._fileSystem != null)); // Suggested By ReviewBot 
        }
    }
}
