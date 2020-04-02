using DWGitsh.Extensions.Models;
using DWGitsh.Extensions.Utility.Cache;
using DWPowerShell.Utility;
using DWPowerShell.Utility.Cache;
using StaticAbstraction;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DWGitsh.Extensions.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace DWGitsh.Extensions.Utility
{
    public interface IGitUtils
    {
        string GetPathToRepository(string currentPath);
        string GetBranchName(IRepositoryPaths repoDirs);
        RepoPaths GetRepoPaths(string currentPath, bool noCache);

        string FixInvalidFileNameCharsInPath(string path);

        string TrimTrailingSlash(string path);
    }

    public class GitUtils : IGitUtils
    {
        public static IGitUtils Current { get; private set; }

        protected IStaticAbstraction _diskManager;

        protected ICacheContainer RepoCache;

        protected static char[] _slashChars = new char[] {'\\', '/'};

        static GitUtils()
        {
            Current = new GitUtils();
        }

        private GitUtils()
        {
            _diskManager = new StAbWrapper();
            RepoCache = new CommandCache();
        }

        // needed for unit testing
        internal GitUtils(IStaticAbstraction diskManager, ICacheContainer repoCache) : this()
        {
            if (diskManager!= null) _diskManager = diskManager;
            if (repoCache!=null) RepoCache = repoCache;
            Current = this;
        }


        public string GetPathToRepository(string currentPath)
        {
             if (string.IsNullOrWhiteSpace(currentPath)) return null;
            var curDir = _diskManager.NewDirectoryInfo(currentPath);
            if (curDir == null) return null;

            while (curDir != null)
            {
                var path = _diskManager.Path.Combine(curDir.FullName, ".git");
                if (_diskManager.Directory.Exists(path)) return DWPSUtils.ForceTrailingSlash(curDir.FullName);
                curDir = curDir.Parent;
            }

            return null;
        }


        public string GetBranchName(IRepositoryPaths repoDirs)
        {
            string branch = null;
            var gitPath = repoDirs?.RepositoryFolder;
            if (!string.IsNullOrWhiteSpace(gitPath))
            {
                var path = _diskManager.Path.Combine(gitPath, "HEAD");
                if (_diskManager.File.Exists(path))
                {
                    var item = _diskManager.File.ReadAllText(path);
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        var pos = item.LastIndexOf('/');
                        if (pos > 0) branch = item.Substring(pos + 1).Trim();
                    }
                }
            }

            return branch;
        }


        public RepoPaths GetRepoPaths(string currentPath, bool noCache)
        {
            if (string.IsNullOrWhiteSpace(currentPath)) throw new ArgumentException("Current path cannot be empty");

            var fixedCurPath = DWPSUtils.ForceTrailingSlash(currentPath);
            var refCurPath = "REF" + fixedCurPath;
            RepoPaths result = null;

            if (!noCache) result = RepoCache.Get<RepoPaths>(refCurPath);

            if (result == null)
            {
                result = BuildRepoPaths(currentPath);
                RepoCache.Add(refCurPath, result, new TimeSpan(0, 10, 0), true);
            }

            result.IgnoreCache = noCache;

            return result;
        }

        /// <summary>
        /// replaces characters that git allows (i.e. double quotes) with Windows equivalents where possible.
        /// </summary>
        /// <param name="path">the path to check</param>
        /// <returns>a valid Windows-safe version of the file path</returns>
        public string FixInvalidFileNameCharsInPath(string path)
        {
            var findChars = new string[] {"\""}; // just double quotes for now but I suspect the list will grow over time
            var replaceChars = new string[] {""};
            return DWPSUtils.ReplaceTokens(path, findChars, replaceChars, false);
        }


        internal RepoPaths BuildRepoPaths(string currentPath)
        {
            var fixedCurPath = DWPSUtils.ForceTrailingSlash(currentPath);
            var result = new RepoPaths { CurrentPath = fixedCurPath };
            var repoPath = GetPathToRepository(currentPath);

            if (!String.IsNullOrWhiteSpace(repoPath))
            {
                result.RootFolder = DWPSUtils.ForceTrailingSlash(repoPath);
                result.RepositoryFolder = DWPSUtils.ForceTrailingSlash(_diskManager.Path.Combine(result.RootFolder, ".git"));
                result.RelativePathToRoot = DWPSUtils.BuildRelativePath(currentPath, result.RootFolder);
                result.RelativePathToRepository = DWPSUtils.BuildRelativePath(currentPath, result.RepositoryFolder);
            }

            var refPath = result.RootFolder ?? result.CurrentPath;

            var cache = RepoCache.Get<CommandCache>(refPath);
            if (cache == null)
            {
                cache = new CommandCache();
                RepoCache.Add(refPath, cache);
            }

            result.Cache = cache;

            return result;
        }


        public string TrimTrailingSlash(string path)
        {
            var result = path;

            if (!string.IsNullOrEmpty(path))
                result = result.TrimEnd(_slashChars);

            return result;
        }
    }
}
