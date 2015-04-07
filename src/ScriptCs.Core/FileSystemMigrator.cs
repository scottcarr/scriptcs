using System.Collections.Generic;
using System.Linq;
using ScriptCs.Contracts;
using ScriptCs.Logging;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public class FileSystemMigrator : IFileSystemMigrator
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILog _logger;
        private readonly Dictionary<string, string> _fileCopies;
        private readonly Dictionary<string, string> _directoryMoves;
        private readonly Dictionary<string, string> _directoryCopies;

        public FileSystemMigrator(IFileSystem fileSystem, ILog logger)
        {
            #region CodeContracts 
            Contract.Requires(fileSystem != null); // Suggested By ReviewBot 
            Contract.Ensures(this._directoryCopies != null); // Suggested By ReviewBot 
            Contract.Ensures(this._fileCopies != null); // Suggested By ReviewBot 
            Contract.Ensures(this._directoryMoves != null); // Suggested By ReviewBot 
            Contract.Ensures(this._fileSystem != null); // Suggested By ReviewBot 
            Contract.Ensures(fileSystem == this._fileSystem); // Suggested By ReviewBot 
            Contract.Ensures(logger == this._logger); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("fileSystem", fileSystem);
            Guard.AgainstNullArgument("logger", logger);

            _fileSystem = fileSystem;
            _logger = logger;

            _fileCopies = new Dictionary<string, string>
            {
                { "packages.config", _fileSystem.PackagesFile },
                { "nuget.config", _fileSystem.NugetFile },
            };

            _directoryMoves = new Dictionary<string, string>
            {
                { ".cache", _fileSystem.DllCacheFolder },
            };

            _directoryCopies = new Dictionary<string, string>
            {
                { "bin", _fileSystem.BinFolder },
                { "packages", _fileSystem.PackagesFolder },
            };
        }

        public void Migrate()
        {
            #region CodeContracts 
            Contract.Ensures(System.Linq.Enumerable.Count(this._fileCopies) == this._fileCopies.Count); // Suggested By ReviewBot 
            Contract.Ensures(0 <= System.Linq.Enumerable.Count(this._fileCopies)); // Suggested By ReviewBot 
            Contract.Ensures(0 <= this._fileCopies.Count); // Suggested By ReviewBot 
            #endregion CodeContracts 

            foreach (var copy in _fileCopies
                .Where(copy => _fileSystem.FileExists(copy.Value)))
            {
                _logger.DebugFormat(
                    "Not performing migration since file '{0}' already exists.",
                    copy.Value);

                return;
            }

            foreach (var action in _directoryMoves.Concat(_directoryCopies)
                .Where(action => _fileSystem.DirectoryExists(action.Value)))
            {
                _logger.DebugFormat(
                    "Not performing migration since directory '{0}' already exists.",
                    action.Value);

                return;
            }

            foreach (var copy in _fileCopies
                .Where(copy => _fileSystem.FileExists(copy.Key)))
            {
                _logger.InfoFormat(
                    "Copying file '{0}' to '{1}'...", copy.Key, copy.Value);

                _fileSystem.Copy(copy.Key, copy.Value, false);
            }

            foreach (var move in _directoryMoves
                .Where(move => _fileSystem.DirectoryExists(move.Key)))
            {
                _logger.InfoFormat(
                    "Moving directory '{0}' to '{1}'...", move.Key, move.Value);

                _fileSystem.MoveDirectory(move.Key, move.Value);
            }

            foreach (var copy in _directoryCopies
                .Where(copy => _fileSystem.DirectoryExists(copy.Key)))
            {
                _logger.InfoFormat(
                    "Copying directory '{0}' to '{1}'...", copy.Key, copy.Value);

                _fileSystem.CopyDirectory(copy.Key, copy.Value, false);
            }
        }
    }
}
