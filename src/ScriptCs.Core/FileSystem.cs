using System;
using System.Collections.Generic;
using System.IO;
using ScriptCs.Contracts;
using System.Diagnostics.Contracts;

namespace ScriptCs
{
    public class FileSystem : IFileSystem
    {
        public virtual IEnumerable<string> EnumerateFiles(
            string dir, string searchPattern, SearchOption searchOption = SearchOption.AllDirectories)
        {
            #region CodeContracts 
            Contract.Assume(dir != null); // Suggested By ReviewBot 
            Contract.Assume(searchPattern != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return Directory.EnumerateFiles(dir, searchPattern, searchOption);
        }

        public virtual IEnumerable<string> EnumerateDirectories(
            string dir, string searchPattern, SearchOption searchOption = SearchOption.AllDirectories)
        {
            #region CodeContracts 
            Contract.Assume(dir != null); // Suggested By ReviewBot 
            Contract.Assume(searchPattern != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return Directory.EnumerateDirectories(dir, searchPattern, searchOption);
        }

        public virtual IEnumerable<string> EnumerateFilesAndDirectories(
            string dir, string searchPattern, SearchOption searchOption = SearchOption.AllDirectories)
        {
            #region CodeContracts 
            Contract.Assume(dir != null); // Suggested By ReviewBot 
            Contract.Assume(searchPattern != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return Directory.EnumerateFileSystemEntries(dir, searchPattern, searchOption);
        }

        public virtual void Copy(string source, string dest, bool overwrite)
        {
            #region CodeContracts 
            Contract.Ensures(!string.IsNullOrEmpty(source)); // Suggested By ReviewBot 
            Contract.Ensures(!string.IsNullOrEmpty(dest)); // Suggested By ReviewBot 
            Contract.Assume(!string.IsNullOrEmpty(source)); // Suggested By ReviewBot 
            Contract.Assume(!string.IsNullOrEmpty(dest)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            File.Copy(source, dest, overwrite);
        }

        public virtual void CopyDirectory(string source, string dest, bool overwrite)
        {
            #region CodeContracts 
            Contract.Ensures(!string.IsNullOrEmpty(dest)); // Suggested By ReviewBot 
            Contract.Ensures(!string.IsNullOrEmpty(source)); // Suggested By ReviewBot 
            Contract.Assume(!string.IsNullOrEmpty(dest)); // Suggested By ReviewBot 
            Contract.Assume(!string.IsNullOrEmpty(source)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            // NOTE: adding guards since the exceptions thrown by System.IO would be confusing
            Guard.AgainstNullArgument("source", source);
            Guard.AgainstNullArgument("dest", dest);

            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
            }

            foreach (var file in Directory.GetFiles(source))
            {
                File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), overwrite);
            }

            foreach (var directory in Directory.GetDirectories(source))
            {
                CopyDirectory(directory, Path.Combine(dest, Path.GetFileName(directory)), overwrite);
            }
        }

        public virtual bool DirectoryExists(string path)
        {
            #region CodeContracts 
            Contract.Ensures(!string.IsNullOrEmpty(path)); // Suggested By ReviewBot 
            Contract.Assume(!string.IsNullOrEmpty(path)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return Directory.Exists(path);
        }

        public virtual void CreateDirectory(string path, bool hidden)
        {
            #region CodeContracts 
            Contract.Ensures(!string.IsNullOrEmpty(path)); // Suggested By ReviewBot 
            Contract.Ensures(1 <= path.Length); // Suggested By ReviewBot 
            Contract.Ensures(0 <= path.Length); // Suggested By ReviewBot 
            Contract.Assume(!string.IsNullOrEmpty(path)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            var directory = Directory.CreateDirectory(path);

            if (hidden)
            {
                directory.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
        }

        public virtual void DeleteDirectory(string path)
        {
            Directory.Delete(path, true);
        }

        public virtual string ReadFile(string path)
        {
            #region CodeContracts 
            Contract.Ensures(!string.IsNullOrEmpty(path)); // Suggested By ReviewBot 
            Contract.Ensures(1 <= path.Length); // Suggested By ReviewBot 
            Contract.Assume(!string.IsNullOrEmpty(path)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return File.ReadAllText(path);
        }

        public virtual string[] ReadFileLines(string path)
        {
            #region CodeContracts 
            Contract.Ensures(!string.IsNullOrEmpty(path)); // Suggested By ReviewBot 
            Contract.Ensures(Contract.ForAll(0, Contract.Result<System.String[]>().Length, __k__ => Contract.Result<System.String[]>()[__k__] != null)); // Suggested By ReviewBot 
            Contract.Assume(!string.IsNullOrEmpty(path)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return File.ReadAllLines(path);
        }

        public virtual bool IsPathRooted(string path)
        {
            #region CodeContracts 
            Contract.Ensures(Contract.Result<System.Boolean>() == System.IO.Path.IsPathRooted(path)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return Path.IsPathRooted(path);
        }

        public virtual string CurrentDirectory
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(1 <= Contract.Result<System.String>().Length); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return Environment.CurrentDirectory;
            }
            set
            {
                #region CodeContracts 
                Contract.Ensures(1 <= value.Length); // Suggested By ReviewBot 
                Contract.Assume(value != null); // Suggested By ReviewBot 
                Contract.Assume(value.Length > 0); // Suggested By ReviewBot 
                #endregion CodeContracts 

                Environment.CurrentDirectory = value;
            }
        }

        public virtual string NewLine
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(1 <= Contract.Result<System.String>().Length); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return Environment.NewLine;
            }
        }

        public virtual DateTime GetLastWriteTime(string file)
        {
            #region CodeContracts 
            Contract.Ensures(!string.IsNullOrEmpty(file)); // Suggested By ReviewBot 
            Contract.Assume(!string.IsNullOrEmpty(file)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return File.GetLastWriteTime(file);
        }

        public virtual void Move(string source, string dest)
        {
            #region CodeContracts 
            Contract.Ensures(1 <= source.Length); // Suggested By ReviewBot 
            Contract.Ensures(1 <= dest.Length); // Suggested By ReviewBot 
            Contract.Ensures(0 <= source.Length); // Suggested By ReviewBot 
            Contract.Ensures(0 <= dest.Length); // Suggested By ReviewBot 
            Contract.Assume(source != null); // Suggested By ReviewBot 
            Contract.Assume(dest != null); // Suggested By ReviewBot 
            Contract.Assume(source.Length != 0); // Suggested By ReviewBot 
            Contract.Assume(dest.Length != 0); // Suggested By ReviewBot 
            #endregion CodeContracts 

            File.Move(source, dest);
        }

        public virtual void MoveDirectory(string source, string dest)
        {
            #region CodeContracts 
            Contract.Ensures(!string.IsNullOrEmpty(source)); // Suggested By ReviewBot 
            Contract.Ensures(!string.IsNullOrEmpty(dest)); // Suggested By ReviewBot 
            Contract.Assume(!string.IsNullOrEmpty(source)); // Suggested By ReviewBot 
            Contract.Assume(!string.IsNullOrEmpty(dest)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Directory.Move(source, dest);
        }

        public virtual bool FileExists(string path)
        {
            #region CodeContracts 
            Contract.Ensures(Contract.Result<System.Boolean>() == System.IO.File.Exists(path)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return File.Exists(path);
        }

        public virtual void FileDelete(string path)
        {
            #region CodeContracts 
            Contract.Ensures(!string.IsNullOrEmpty(path)); // Suggested By ReviewBot 
            Contract.Assume(!string.IsNullOrEmpty(path)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            File.Delete(path);
        }

        public virtual IEnumerable<string> SplitLines(string value)
        {
            #region CodeContracts 
            Contract.Ensures(0 <= ((System.Array)Contract.Result<System.Collections.Generic.IEnumerable<System.String>>()).Length); // Suggested By ReviewBot 
            Contract.Assume(value != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            Guard.AgainstNullArgument("value", value);

            return value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        }

        public virtual void WriteToFile(string path, string text)
        {
            File.WriteAllText(path, text);
        }

        public virtual Stream CreateFileStream(string filePath, FileMode mode)
        {
            #region CodeContracts 
            Contract.Ensures(!string.IsNullOrEmpty(filePath)); // Suggested By ReviewBot 
            Contract.Assume(!string.IsNullOrEmpty(filePath)); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return new FileStream(filePath, mode);
        }

        public virtual void WriteAllBytes(string filePath, byte[] bytes)
        {
            File.WriteAllBytes(filePath, bytes);
        }

        public virtual string GlobalFolder
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(8 <= Contract.Result<System.String>().Length); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "scriptcs");
            }
        }

        public virtual string GetWorkingDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return CurrentDirectory;
            }

            var realPath = GetFullPath(path);

            if (FileExists(realPath) || DirectoryExists(realPath))
            {
                if ((File.GetAttributes(realPath) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    return realPath;
                }

                return Path.GetDirectoryName(realPath);
            }

            return Path.GetDirectoryName(realPath);
        }

        public virtual string GetFullPath(string path)
        {
            #region CodeContracts 
            Contract.Ensures(System.IO.Path.GetFullPath(path) != null); // Suggested By ReviewBot 
            Contract.Ensures(Contract.Result<System.String>() == System.IO.Path.GetFullPath(path)); // Suggested By ReviewBot 
            Contract.Assume(path != null); // Suggested By ReviewBot 
            #endregion CodeContracts 

            return Path.GetFullPath(path);
        }

        public virtual string HostBin
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(System.AppDomain.CurrentDomain.BaseDirectory != null); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public virtual string BinFolder
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @"scriptcs_bin"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "scriptcs_bin";
            }
        }

        public virtual string DllCacheFolder
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @".scriptcs_cache"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return ".scriptcs_cache";
            }
        }

        public virtual string PackagesFile
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @"scriptcs_packages.config"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "scriptcs_packages.config";
            }
        }

        public virtual string PackagesFolder
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @"scriptcs_packages"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "scriptcs_packages";
            }
        }

        public virtual string NugetFile
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(Contract.Result<System.String>() == @"scriptcs_nuget.config"); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return "scriptcs_nuget.config";
            }
        }

        public virtual string GlobalOptsFile
        {
            get
            {
                #region CodeContracts 
                Contract.Ensures(this.GlobalFolder != null); // Suggested By ReviewBot 
                Contract.Ensures(13 <= Contract.Result<System.String>().Length); // Suggested By ReviewBot 
                Contract.Assume(this.GlobalFolder != null); // Suggested By ReviewBot 
                #endregion CodeContracts 

                return Path.Combine(GlobalFolder, Constants.ConfigFilename);
            }
        }
    }
}
