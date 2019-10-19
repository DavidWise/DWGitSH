using DWGitsh.Extensions.Models;

namespace DWGitsh.Extensions.Commands.Git.Status
{
    public class GitStatusCommand : GitCommandBase<GitStatus, GitStatusParser>
    {
        public GitStatusCommand(RepoPaths repoDirs, bool useCache = true) : base(repoDirs, useCache)
        {
            Name = "Status";
            CacheName = "status";
            //Command = "git status --v";//  --ahead-behind";
            Command = "git status";
            CacheLinkFiles = new string[] {"index","FETCH_HEAD", "HEAD", "COMMIT_EDITMSG", "refs/remotes/origin/{branchName}"};
            Parser = new GitStatusParser(this);

            CommandExecFolder = repoDirs.CurrentPath;
        }

        // Numerous commands affect the repo differently and the list below is *come* of the digging I've done to track it down.  The reason this
        // is needed is so that the "git status" command (which is expensive to call) knows when it needs to expire the cache

        // Command              files affected
        // Git add . -->        index
        // git commit -m ""     Index, COMMIT_EDITMSG, refs/head/[branch name], logs/HEAD, objects/*
        // git push             refs/remotes/origin/[branch name], logs/remotes/refs/origin/[branch name]
        // git reset --hard
        // git checkout
        // git rebase
        // git fetch

        protected override string BuildCacheName()
        {
            var relDir = RepositoryDirectories.RootFolder;
            return this.CacheName += $"_{relDir}";
        }
    }
}