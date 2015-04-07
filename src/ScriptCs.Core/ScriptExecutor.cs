using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using ScriptCs.Logging;
using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public class ScriptExecutor : IScriptExecutor
    {
        public static readonly string[] DefaultReferences =
        {
            "System",
            "System.Core",
            "System.Data",
            "System.Data.DataSetExtensions",
            "System.Xml",
            "System.Xml.Linq",
            "System.Net.Http",
            typeof(ScriptExecutor).Assembly.Location,
            typeof(IScriptEnvironment).Assembly.Location
        };

        public static readonly string[] DefaultNamespaces =
        {
            "System",
            "System.Collections.Generic",
            "System.Linq",
            "System.Text",
            "System.Threading.Tasks",
            "System.IO",
            "System.Net.Http"
        };

        private const string ScriptLibrariesInjected = "ScriptLibrariesInjected";

        public IFileSystem FileSystem { get; private set; }

        public IFilePreProcessor FilePreProcessor { get; private set; }

        public IScriptEngine ScriptEngine { get; private set; }

        public ILog Logger { get; private set; }

        public AssemblyReferences References { get; private set; }

        public ICollection<string> Namespaces { get; private set; }

        public ScriptPackSession ScriptPackSession { get; protected set; }

        public IScriptLibraryComposer ScriptLibraryComposer { get; protected set; }

        public ScriptExecutor(
            IFileSystem fileSystem, IFilePreProcessor filePreProcessor, IScriptEngine scriptEngine, ILog logger)
            : this(fileSystem, filePreProcessor, scriptEngine, logger, new NullScriptLibraryComposer())
        {
        }

        public ScriptExecutor(
            IFileSystem fileSystem,
            IFilePreProcessor filePreProcessor,
            IScriptEngine scriptEngine,
            ILog logger,
            IScriptLibraryComposer composer)
        {
            #region CodeContracts 
            Contract.Requires(fileSystem != null); // Suggested By ReviewBot 
            Contract.Ensures(this.FileSystem != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("fileSystem", fileSystem);
            Guard.AgainstNullArgumentProperty("fileSystem", "BinFolder", fileSystem.BinFolder);
            Guard.AgainstNullArgumentProperty("fileSystem", "DllCacheFolder", fileSystem.DllCacheFolder);
            Guard.AgainstNullArgument("filePreProcessor", filePreProcessor);
            Guard.AgainstNullArgument("scriptEngine", scriptEngine);
            Guard.AgainstNullArgument("logger", logger);
            Guard.AgainstNullArgument("composer", composer);

            References = new AssemblyReferences(DefaultReferences);
            Namespaces = new Collection<string>();
            ImportNamespaces(DefaultNamespaces);
            FileSystem = fileSystem;
            FilePreProcessor = filePreProcessor;
            ScriptEngine = scriptEngine;
            Logger = logger;
            ScriptLibraryComposer = composer;
        }

        public void ImportNamespaces(params string[] namespaces)
        {
            #region CodeContracts 
            Contract.Assume(namespaces != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("namespaces", namespaces);

            foreach (var @namespace in namespaces)
            {
                Namespaces.Add(@namespace);
            }
        }

        public void AddReferences(params Assembly[] assemblies)
        {
            #region CodeContracts 
            Contract.Assume(this.References != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("assemblies", assemblies);

            References = References.Union(assemblies);
        }

        public void RemoveReferences(params Assembly[] assemblies)
        {
            #region CodeContracts 
            Contract.Assume(this.References != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("assemblies", assemblies);

            References = References.Except(assemblies);
        }

        public void AddReferences(params string[] paths)
        {
            #region CodeContracts 
            Contract.Assume(this.References != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("paths", paths);

            References = References.Union(paths);
        }

        public void RemoveReferences(params string[] paths)
        {
            #region CodeContracts 
            Contract.Assume(this.References != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("paths", paths);

            References = References.Except(paths);
        }

        public void RemoveNamespaces(params string[] namespaces)
        {
            #region CodeContracts 
            Contract.Assume(namespaces != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("namespaces", namespaces);

            foreach (var @namespace in namespaces)
            {
                Namespaces.Remove(@namespace);
            }
        }

        public virtual void Initialize(
            IEnumerable<string> paths, IEnumerable<IScriptPack> scriptPacks, params string[] scriptArgs)
        {
            #region CodeContracts 
            Contract.Ensures(System.Linq.Enumerable.ToArray(paths) != null); // Suggested By ReviewBot 
            Contract.Ensures(System.Linq.Enumerable.Count(paths) == System.Linq.Enumerable.ToArray(paths).Length); // Suggested By ReviewBot 
            Contract.Ensures(0 <= System.Linq.Enumerable.ToArray(paths).Length); // Suggested By ReviewBot 
            Contract.Ensures(0 <= System.Linq.Enumerable.Count(paths)); // Suggested By ReviewBot 
            Contract.Assume(paths != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            AddReferences(paths.ToArray());
            var bin = Path.Combine(FileSystem.CurrentDirectory, FileSystem.BinFolder);
            var cache = Path.Combine(FileSystem.CurrentDirectory, FileSystem.DllCacheFolder);

            ScriptEngine.BaseDirectory = bin;
            ScriptEngine.CacheDirectory = cache;

            Logger.Debug("Initializing script packs");
            var scriptPackSession = new ScriptPackSession(scriptPacks, scriptArgs);
            ScriptPackSession = scriptPackSession;
            scriptPackSession.InitializePacks();
        }

        public virtual void Reset()
        {
            #region CodeContracts 
            Contract.Assume(this.Namespaces != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            References = new AssemblyReferences(DefaultReferences);
            Namespaces.Clear();
            ImportNamespaces(DefaultNamespaces);

            ScriptPackSession.State.Clear();
        }

        public virtual void Terminate()
        {
            #region CodeContracts 
            Contract.Ensures(this.ScriptPackSession != null); // Suggested By ReviewBot 
            Contract.Assume(this.Logger != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Logger.Debug("Terminating packs");
            ScriptPackSession.TerminatePacks();
        }

        public virtual ScriptResult Execute(string script, params string[] scriptArgs)
        {
            #region CodeContracts 
            Contract.Ensures(0 <= script.Length); // Suggested By ReviewBot 
            Contract.Assume(this.FilePreProcessor != null); // Suggested By ReviewBot 
            Contract.Assume((System.IO.Path.IsPathRooted(script) || script != null)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            var path = Path.IsPathRooted(script) ? script : Path.Combine(FileSystem.CurrentDirectory, script);
            var result = FilePreProcessor.ProcessFile(path);
            References = References.Union(result.References);
            var namespaces = Namespaces.Union(result.Namespaces);
            ScriptEngine.FileName = Path.GetFileName(path);

            Logger.Debug("Starting execution in engine");

            InjectScriptLibraries(Path.GetDirectoryName(path), result, ScriptPackSession.State);
            return ScriptEngine.Execute(result.Code, scriptArgs, References, namespaces, ScriptPackSession);
        }

        public virtual ScriptResult ExecuteScript(string script, params string[] scriptArgs)
        {
            #region CodeContracts 
            Contract.Assume(this.FilePreProcessor != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            var result = FilePreProcessor.ProcessScript(script);
            References = References.Union(result.References);
            var namespaces = Namespaces.Union(result.Namespaces);

            Logger.Debug("Starting execution in engine");

            InjectScriptLibraries(FileSystem.CurrentDirectory, result, ScriptPackSession.State);
            return ScriptEngine.Execute(result.Code, scriptArgs, References, namespaces, ScriptPackSession);
        }

        protected internal virtual void InjectScriptLibraries(
            string workingDirectory,
            FilePreProcessorResult result,
            IDictionary<string, object> state
        )
        {
            #region CodeContracts 
            Contract.Requires(state != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("result", result);
            Guard.AgainstNullArgument("state", state);

            if (state.ContainsKey(ScriptLibrariesInjected))
            {
                return;
            }

            var scriptLibrariesPreProcessorResult = LoadScriptLibraries(workingDirectory);

            if (scriptLibrariesPreProcessorResult != null)
            {
                result.Code = scriptLibrariesPreProcessorResult.Code + Environment.NewLine + result.Code;
                result.References.AddRange(scriptLibrariesPreProcessorResult.References);
                result.Namespaces.AddRange(scriptLibrariesPreProcessorResult.Namespaces);
            }
            state.Add(ScriptLibrariesInjected, null);
        }

        protected internal virtual FilePreProcessorResult LoadScriptLibraries(string workingDirectory)
        {
            #region CodeContracts 
            Contract.Requires(this.ScriptLibraryComposer != null); // Suggested By ReviewBot 
            Contract.Requires((string.IsNullOrWhiteSpace(this.ScriptLibraryComposer.ScriptLibrariesFile) || workingDirectory != null)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            if (string.IsNullOrWhiteSpace(ScriptLibraryComposer.ScriptLibrariesFile))
            {
                return null;
            }

            var scriptLibrariesPath = Path.Combine(workingDirectory, FileSystem.PackagesFolder,
                ScriptLibraryComposer.ScriptLibrariesFile);

            if (FileSystem.FileExists(scriptLibrariesPath))
            {
                Logger.DebugFormat("Found Script Library at {0}", scriptLibrariesPath);
                return FilePreProcessor.ProcessFile(scriptLibrariesPath);
            }

            return null;
        }
    }
}
