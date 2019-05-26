using DWGitsh.Extensions.Cmdlets.Common;
using DWGitsh.Extensions.Models;
using DWGitsh.Extensions.Utility;
using DWGitsh.Extensions.Utility.Colors;
using DWGitsh.Extensions.Utility.ConsoleIO;
using DWPowerShell.Utility;
using System;
using System.Collections.Generic;
using StaticAbstraction.IO;
using DWPowerShell.Utility.Abstraction.Process;
using StaticAbstraction;

namespace DWGitsh.Extensions.Commands.Git
{
    public interface IGitCommand : ICmdletWriterContainer
    {
        string Name { get; set; }
        string CacheName { get; set; }
        string[] CacheLinkFiles { get; set; }
        uint CacheMaxAgeMinutes { get; set; }
        string Command { get; set; }
        string CommandOutput { get; }
        string CommandExecFolder { get; set; }
        bool UseCache { get; set; }
        IRepositoryPaths RepositoryDirectories { get;  }
    }

    public interface IGitCommand<TResult>: IGitCommand where TResult : class, new()
    {
        TResult GetCommandResults(bool skipCache = false);
        IGitCommandResultsParser<TResult> Parser { get;  }
    }


    public abstract class GitCommandBase<TResult, TParser> : IGitCommand<TResult> 
        where TResult:class, new() 
        where TParser: IGitCommandResultsParser<TResult>
    {
        protected IStaticAbstraction _diskManager = null;
        protected IProcessManager _processManager = null;
        protected ColorGroupReader _colorGroups = new ColorGroupReader();

        public string Name { get; set; }
        public string CacheName { get; set; }
        public string[] CacheLinkFiles { get; set; }
        public uint CacheMaxAgeMinutes { get; set; }
        public string Command { get; set; }
        public bool UseCache { get; set; }
        public string CommandExecFolder { get; set; }
        public string CommandOutput { get; protected set; }

        public IGitCommandResultsParser<TResult> Parser { get; protected set; }
        public IRepositoryPaths RepositoryDirectories { get; protected set; }
        public ICmdletWriter CmdletWriter { get; set; }

        protected GitCommandBase(RepoPaths repoDirs)
        {
            Init(null, null, repoDirs, !repoDirs.IgnoreCache);
        }


        protected GitCommandBase(RepoPaths repoDirs, bool useCache)
        {
            Init(null, null, repoDirs, useCache);
        }

        protected GitCommandBase(IStaticAbstraction diskManager, RepoPaths repoDirs, bool useCache = true)
        {
            Init(diskManager, null, repoDirs, useCache);
        }
        protected GitCommandBase(IStaticAbstraction diskManager, IProcessManager processManager, RepoPaths repoDirs, bool useCache = true)
        {
            Init(diskManager, processManager, repoDirs, useCache);
        }

        protected void Init(IStaticAbstraction diskManager, IProcessManager processManager, RepoPaths repoDirs, bool useCache)
        {
            this._diskManager = diskManager ?? new StAbWrapper();
            this._processManager = processManager ?? new ProcessManager();
            this.RepositoryDirectories = repoDirs;
            this.UseCache = useCache;

            _colorGroups = new ColorGroupReader(_diskManager);
        }

        protected virtual string BuildCacheName()
        {
            var cacheName = this.CacheName;
            if (this.CommandExecFolder != null) cacheName += $"_{DWPSUtils.MakeFileSystemSafe(this.CommandExecFolder)}";

            return cacheName;
        }

        public virtual TResult GetCommandResults(bool skipCache = false)
        {
            if (RepositoryDirectories == null) return null;
            if (string.IsNullOrWhiteSpace(RepositoryDirectories.RootFolder)) return null;
            if (string.IsNullOrWhiteSpace(this.Command)) return null;

            var cacheName = BuildCacheName();

            var useCache = this.UseCache;
            if (skipCache || RepositoryDirectories.IgnoreCache || string.IsNullOrWhiteSpace(this.CacheName)) useCache = false;

            TResult resultOut = default(TResult);

            if (useCache) resultOut = RepositoryDirectories.Cache.Get<TResult>(cacheName);
            this.CommandOutput = null;

            if (resultOut == default(TResult))
            {
                var execFolder = this.CommandExecFolder ?? RepositoryDirectories.RootFolder;
                var writer = new ConsoleWriter();
                var barColor = _colorGroups.GetColor("commandExecBar");
                writer.WriteHeader(this.Command, barColor.Foreground, barColor.Background, ConsoleTextPosition.Center);
                var result = DWPSUtils.ExecuteCommandSync(_diskManager, _processManager, this.Command, execFolder);
                writer.ResetConsole();

                resultOut = this.Parser.Parse( result.Output);

                AddItemToCache(RepositoryDirectories, cacheName, resultOut);
                this.CommandOutput = result.Output;
            }

            return resultOut;
        }

        protected List<string> ResolveLinkedFiles(Dictionary<string, string> tokens)
        {
            var result = new List<string>();

            if (this.CacheLinkFiles != null && this.CacheLinkFiles.Length > 0)
            {
                foreach (var linkFile in this.CacheLinkFiles)
                {
                    var cacheLinkFile = linkFile.Replace("/", "\\");

                    foreach (var token in tokens)
                    {
                        if (cacheLinkFile.IndexOf(token.Key, StringComparison.InvariantCulture) >= 0)
                            cacheLinkFile = cacheLinkFile.Replace(token.Key, token.Value);
                        var linkFilePath = DWPSUtils.IsFullPath(cacheLinkFile)
                            ? cacheLinkFile
                            : _diskManager.Path.Combine(this.RepositoryDirectories.RepositoryFolder, cacheLinkFile);
                        result.Add(linkFilePath);
                    }
                }
            }

            return result;
        }

        protected void AddItemToCache(IRepositoryPaths repoDirs, string cacheName, TResult value)
        {
            if (repoDirs.IgnoreCache) return;

            if (this.CacheLinkFiles != null && this.CacheLinkFiles.Length>0)
            {
                var tokens = new Dictionary<string, string>();

                var curBranch = GitUtils.Current.GetBranchName(repoDirs);
                tokens.Add("{branchName}", curBranch);
                var linkedFiles = ResolveLinkedFiles(tokens);

                repoDirs.Cache.Add(cacheName, value, linkedFiles.ToArray());
            }
            else if (this.CacheMaxAgeMinutes > 0)
            {
                var expSpan = new TimeSpan(0, Convert.ToInt32(this.CacheMaxAgeMinutes), 0);
                repoDirs.Cache.Add(cacheName, value, expSpan);
            }
            else
            {
                // Should this add a non-expiring item to the cache???
                repoDirs.Cache.Add(cacheName, value);
            }
        }
    }  
}