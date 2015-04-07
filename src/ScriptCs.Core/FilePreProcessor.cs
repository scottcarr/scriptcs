using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScriptCs.Contracts;
using ScriptCs.Logging;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public class FilePreProcessor : IFilePreProcessor
    {
        private readonly ILog _logger;

        private readonly IEnumerable<ILineProcessor> _lineProcessors;

        private readonly IFileSystem _fileSystem;

        public FilePreProcessor(IFileSystem fileSystem, ILog logger, IEnumerable<ILineProcessor> lineProcessors)
        {
            // SCOTT: VALID NEW CONTRACTS
            #region CodeContracts 
            Contract.Requires(logger != null); // Suggested By ReviewBot 
            Contract.Ensures(fileSystem == this._fileSystem); // Suggested By ReviewBot 
            Contract.Ensures(logger == this._logger); // Suggested By ReviewBot 
            Contract.Ensures(lineProcessors == this._lineProcessors); // Suggested By ReviewBot 
            #endregion CodeContracts 

            _fileSystem = fileSystem;
            _logger = logger;
            _lineProcessors = lineProcessors;
        }

        public virtual FilePreProcessorResult ProcessFile(string path)
        {
            return Process(context => ParseFile(path, context));
        }

        public virtual FilePreProcessorResult ProcessScript(string script)
        {
            var scriptLines = _fileSystem.SplitLines(script).ToList();
            return Process(context => ParseScript(scriptLines, context));
        }

        protected virtual FilePreProcessorResult Process(Action<FileParserContext> parseAction)
        {
            #region CodeContracts 
            Contract.Requires(parseAction != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("parseAction", parseAction);

            var context = new FileParserContext();

            _logger.DebugFormat("Starting pre-processing");

            parseAction(context);

            var code = GenerateCode(context);

            _logger.DebugFormat("Pre-processing finished successfully");

            return new FilePreProcessorResult
            {
                Namespaces = context.Namespaces,
                LoadedScripts = context.LoadedScripts,
                References = context.References,
                Code = code
            };
        }

        protected virtual string GenerateCode(FileParserContext context)
        {
            #region CodeContracts 
            Contract.Requires(context != null); // Suggested By ReviewBot 
            Contract.Requires(context.BodyLines != null); // Suggested By ReviewBot 
            Contract.Ensures(context.BodyLines != null); // Suggested By ReviewBot 
            Contract.Ensures(this._fileSystem != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("context", context);
            return string.Join(_fileSystem.NewLine, context.BodyLines);
        }

        public virtual void ParseFile(string path, FileParserContext context)
        {
            #region CodeContracts 
            Contract.Ensures(context.LoadedScripts != null); // Suggested By ReviewBot 
            Contract.Ensures(this._logger != null); // Suggested By ReviewBot 
            Contract.Ensures(1 <= context.LoadedScripts.Count); // Suggested By ReviewBot 
            Contract.Ensures(0 <= context.LoadedScripts.Count); // Suggested By ReviewBot 
            Contract.Assume(context != null); // Suggested By ReviewBot 
            Contract.Assume(context.LoadedScripts != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("path", path);
            Guard.AgainstNullArgument("context", context);

            var fullPath = _fileSystem.GetFullPath(path);
            var filename = Path.GetFileName(path);

            if (context.LoadedScripts.Contains(fullPath))
            {
                _logger.DebugFormat("Skipping {0} because it's already been loaded.", filename);
                return;
            }

            _logger.DebugFormat("Processing {0}...", filename);

            // Add script to loaded collection before parsing to avoid loop.
            context.LoadedScripts.Add(fullPath);

            var scriptLines = _fileSystem.ReadFileLines(fullPath).ToList();

            InsertLineDirective(fullPath, scriptLines);
            InDirectory(fullPath, () => ParseScript(scriptLines, context));
        }

        public virtual void ParseScript(List<string> scriptLines, FileParserContext context)
        {
            #region CodeContracts 
            Contract.Assume(scriptLines != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("scriptLines", scriptLines);
            Guard.AgainstNullArgument("context", context);

            var codeIndex = scriptLines.FindIndex(IsNonDirectiveLine);

            for (var index = 0; index < scriptLines.Count; index++)
            {
                var line = scriptLines[index];
                var isBeforeCode = index < codeIndex || codeIndex < 0;

                var wasProcessed = _lineProcessors.Any(x => x.ProcessLine(this, context, line, isBeforeCode));

                if (!wasProcessed)
                {
                    context.BodyLines.Add(line);
                }
            }
        }

        protected virtual void InsertLineDirective(string path, List<string> fileLines)
        {
            #region CodeContracts 
            Contract.Requires(fileLines != null); // Suggested By ReviewBot 
            Contract.Ensures(0 <= fileLines.Count); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("fileLines", fileLines);

            var bodyIndex = fileLines.FindIndex(line => IsNonDirectiveLine(line) && !IsUsingLine(line));
            if (bodyIndex == -1)
            {
                return;
            }

            var directiveLine = string.Format("#line {0} \"{1}\"", bodyIndex + 1, path);
            fileLines.Insert(bodyIndex, directiveLine);
        }

        private void InDirectory(string path, Action action)
        {
            var oldCurrentDirectory = _fileSystem.CurrentDirectory;
            _fileSystem.CurrentDirectory = _fileSystem.GetWorkingDirectory(path);

            action();

            _fileSystem.CurrentDirectory = oldCurrentDirectory;
        }

        private bool IsNonDirectiveLine(string line)
        {
            #region CodeContracts 
            Contract.Ensures(this._lineProcessors != null); // Suggested By ReviewBot 
            Contract.Ensures(line.Trim() != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            var directiveLineProcessors =
                _lineProcessors.OfType<IDirectiveLineProcessor>();

            return line.Trim() != string.Empty && !directiveLineProcessors.Any(lp => lp.Matches(line));
        }

        private static bool IsUsingLine(string line)
        {
            return line.TrimStart(' ').StartsWith("using ") && !line.Contains("{") && line.Contains(";");
        }

        [ContractInvariantMethod]
        private void FilePreProcessorObjectInvariantMethod()
        {
            Contract.Invariant(this._fileSystem != null); // Suggested By ReviewBot 
            Contract.Invariant(this._logger != null); // Suggested By ReviewBot 
        }
    }
}
