using System.Linq;
using System.Runtime.Versioning;
using ScriptCs.Contracts;
using ScriptCs.Logging;
using System.Diagnostics.Contracts;

namespace ScriptCs.ReplCommands
{
    public class InstallCommand : IReplCommand
    {
        private readonly IPackageInstaller _packageInstaller;
        private readonly IPackageAssemblyResolver _packageAssemblyResolver;
        private readonly ILog _logger;
        private readonly IInstallationProvider _installationProvider;

        public InstallCommand(
            IPackageInstaller packageInstaller,
            IPackageAssemblyResolver packageAssemblyResolver,
            ILog logger,
            IInstallationProvider installationProvider)
        {
            #region CodeContracts 
            Contract.Ensures(packageInstaller == this._packageInstaller); // Suggested By ReviewBot 
            Contract.Ensures(packageAssemblyResolver == this._packageAssemblyResolver); // Suggested By ReviewBot 
            Contract.Ensures(logger == this._logger); // Suggested By ReviewBot 
            Contract.Ensures(installationProvider == this._installationProvider); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("packageInstaller", packageInstaller);
            Guard.AgainstNullArgument("packageAssemblyResolver", packageAssemblyResolver);
            Guard.AgainstNullArgument("logger", logger);
            Guard.AgainstNullArgument("installationProvider", installationProvider);

            _packageInstaller = packageInstaller;
            _packageAssemblyResolver = packageAssemblyResolver;
            _logger = logger;
            _installationProvider = installationProvider;
        }

        public string Description
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @"Installs a Nuget package. I.e. :install <package> <version>"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "Installs a Nuget package. I.e. :install <package> <version>";
            }
        }

        public string CommandName
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @"install"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "install";
            }
        }

        public object Execute(IRepl repl, object[] args)
        {
            #region CodeContracts 
            Contract.Ensures(Contract.Result<System.Object>() == null); // Suggested By ReviewBot 
            Contract.Assume(((args == null || args.Length == 0) || this._installationProvider != null)); // Suggested By ReviewBot 
            Contract.Assume(((args == null || args.Length == 0) || this._packageInstaller != null)); // Suggested By ReviewBot 
            Contract.Assume(((args == null || args.Length == 0) || this._packageAssemblyResolver != null)); // Suggested By ReviewBot 
            Contract.Assume(((args == null || args.Length == 0) || repl != null)); // Suggested By ReviewBot 
            Contract.Assume(((args == null || args.Length == 0) || this._logger != null)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("repl", repl);

            if (args == null || args.Length == 0)
            {
                return null;
            }

            string version = null;
            if (args.Length >= 2)
            {
                version = args[1].ToString();
            }

            var allowPre = args.Length >= 3 && args[2].ToString().ToUpperInvariant() == "PRE";

            _logger.InfoFormat("Installing {0}", args[0]);

            _installationProvider.Initialize();

            var packageRef = new PackageReference(
                args[0].ToString(), new FrameworkName(".NETFramework,Version=v4.0"), version);

            _packageInstaller.InstallPackages(new[] { packageRef }, allowPre);
            _packageAssemblyResolver.SavePackages();

            var dlls = _packageAssemblyResolver.GetAssemblyNames(repl.FileSystem.CurrentDirectory)
                .Except(repl.References.Paths).ToArray();

            repl.AddReferences(dlls);

            foreach (var dll in dlls)
            {
                _logger.InfoFormat("Added reference to {0}", dll);
            }

            return null;
        }
    }
}
