using DWGitsh.Extensions.Models;

namespace DWGitsh.Extensions.Commands.Git.Config
{
    public class GitConfigCommand : GitCommandBase<GitConfig, GitConfigParser>
    {
        public GitConfigCommand(RepoPaths repoDirs, bool useCache = true): base(repoDirs, useCache)
        {
            Name = "Config";
            CacheName = "config";
            Command = "git --no-pager config --list";
            CacheMaxAgeMinutes = 10;
            Parser = new GitConfigParser(this);
        }
    }
}