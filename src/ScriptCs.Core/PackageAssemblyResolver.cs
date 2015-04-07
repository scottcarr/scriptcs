using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScriptCs.Contracts;
using ScriptCs.Logging;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public class PackageAssemblyResolver : IPackageAssemblyResolver
    {
        private readonly IFileSystem _fileSystem;
        private readonly IPackageContainer _packageContainer;
        private readonly ILog _logger;
        private readonly IAssemblyUtility _assemblyUtility;

        private List<IPackageReference> _topLevelPackages;

        public PackageAssemblyResolver(
            IFileSystem fileSystem, IPackageContainer packageContainer, ILog logger, IAssemblyUtility assemblyUtility)
        {
            #region CodeContracts 
            Contract.Requires(packageContainer != null); // Suggested By ReviewBot 
            Contract.Ensures(this._topLevelPackages == null); // Suggested By ReviewBot 
            Contract.Ensures(fileSystem == this._fileSystem); // Suggested By ReviewBot 
            Contract.Ensures(packageContainer == this._packageContainer); // Suggested By ReviewBot 
            Contract.Ensures(logger == this._logger); // Suggested By ReviewBot 
            Contract.Ensures(assemblyUtility == this._assemblyUtility); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("fileSystem", fileSystem);
            Guard.AgainstNullArgumentProperty("fileSystem", "PackagesFolder", fileSystem.PackagesFolder);
            Guard.AgainstNullArgumentProperty("fileSystem", "PackagesFile", fileSystem.PackagesFile);

            Guard.AgainstNullArgument("packageContainer", packageContainer);
            Guard.AgainstNullArgument("logger", logger);
            Guard.AgainstNullArgument("assemblyUtility", assemblyUtility);

            _fileSystem = fileSystem;
            _packageContainer = packageContainer;
            _logger = logger;
            _assemblyUtility = assemblyUtility;
        }

        public void SavePackages()
        {
            #region CodeContracts 
            Contract.Assume(((ScriptCs.Contracts.IFileSystem)this._fileSystem).CurrentDirectory != null); // Suggested By ReviewBot 
            Contract.Assume(this._fileSystem.PackagesFolder != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            var packagesFolder = Path.Combine(_fileSystem.CurrentDirectory, _fileSystem.PackagesFolder);
            if (!_fileSystem.DirectoryExists(packagesFolder))
            {
                _logger.Warn("Packages directory does not exist!");
                return;
            }

            _packageContainer.CreatePackageFile();
        }

        public IEnumerable<IPackageReference> GetPackages(string workingDirectory)
        {
            #region CodeContracts 
            Contract.Ensures(this._topLevelPackages.ToArray() != null); // Suggested By ReviewBot 
            Contract.Ensures(this._topLevelPackages != null); // Suggested By ReviewBot 
            Contract.Ensures(this._packageContainer != null); // Suggested By ReviewBot 
            Contract.Ensures(this._fileSystem.PackagesFile != null); // Suggested By ReviewBot 
            Contract.Ensures(0 <= this._fileSystem.PackagesFile.Length); // Suggested By ReviewBot 
            Contract.Ensures(0 <= this._topLevelPackages.Count); // Suggested By ReviewBot 
            Contract.Assume(workingDirectory != null); // Suggested By ReviewBot 
            Contract.Assume(this._fileSystem.PackagesFile != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            var packagesFile = Path.Combine(workingDirectory, _fileSystem.PackagesFile);
            _topLevelPackages = _packageContainer.FindReferences(packagesFile).ToList();
            return _topLevelPackages.ToArray();
        }

        public IEnumerable<string> GetAssemblyNames(string workingDirectory)
        {
            var packages = GetPackages(workingDirectory).ToList();
            if (!packages.Any())
            {
                return Enumerable.Empty<string>();
            }

            var packagesFile = Path.Combine(workingDirectory, _fileSystem.PackagesFile);
            var packagesFolder = Path.Combine(workingDirectory, _fileSystem.PackagesFolder);

            var names = new List<string>();
            GetAssemblyNames(packagesFolder, packages, names, _fileSystem.FileExists(packagesFile));
            return names;
        }

        private void GetAssemblyNames(
            string packageDir,
            IEnumerable<IPackageReference> packageReferences,
            ICollection<string> names,
            bool strictLoad)
        {
            foreach (var packageReference in packageReferences)
            {
                var packageObject = _packageContainer.FindPackage(packageDir, packageReference);
                if (packageObject == null)
                {
                    _logger.WarnFormat(
                        "Cannot find: {0} {1}",
                        packageReference.PackageId,
                        packageReference.Version);

                    continue;
                }

                var compatibleDlls = packageObject.GetCompatibleDlls(packageReference.FrameworkName);
                if (compatibleDlls == null)
                {
                    _logger.WarnFormat(
                        "Cannot find compatible binaries for {0} in: {1} {2}",
                        packageReference.FrameworkName,
                        packageReference.PackageId,
                        packageReference.Version);

                    continue;
                }

                foreach (var name in compatibleDlls
                    .Select(packageFile => Path.Combine(packageDir, packageObject.FullName, packageFile))
                    .Where(path => _assemblyUtility.IsManagedAssembly(path))
                    .Concat(packageObject.FrameworkAssemblies)
                    .Where(name => !names.Contains(name)))
                {
                    names.Add(name);
                    _logger.Debug("Found: " + name);
                }

                if (packageObject.Dependencies == null || !packageObject.Dependencies.Any() || !strictLoad)
                {
                    continue;
                }

                var dependencyReferences = packageObject.Dependencies
                    .Where(dependency =>
                        _topLevelPackages.All(topLevelPackage => topLevelPackage.PackageId != dependency.Id))
                    .Select(dependency =>
                        new PackageReference(dependency.Id, dependency.FrameworkName, dependency.Version));

                GetAssemblyNames(packageDir, dependencyReferences, names, true);
            }
        }

        [ContractInvariantMethod]
        private void PackageAssemblyResolverObjectInvariantMethod()
        {
            Contract.Invariant(this._packageContainer != null); // Suggested By ReviewBot 
        }
    }
}
