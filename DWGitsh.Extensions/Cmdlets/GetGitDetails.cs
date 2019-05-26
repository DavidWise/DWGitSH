using System.Management.Automation;
using DWGitsh.Extensions.Cmdlets.Common;
using DWGitsh.Extensions.Commands.Git.Config;
using DWGitsh.Extensions.Commands.Git.Status;
using DWGitsh.Extensions.Models;
using DWGitsh.Extensions.Utility;

namespace DWGitsh.Extensions.Cmdlets
{
    [Cmdlet(VerbsCommon.Get,"GitDetails")]
    [OutputType(typeof(GitDetailModel))]
    public class GetGitDetails : GitCmdletBase<GitDetailModel>
    {
        public GetGitDetails() : base() { }
        
        protected override GitDetailModel BuildResponse()
        {
            return BuildGitDetails(this.RepositoryDirectories, NoCache.IsPresent);
        }

        protected static GitDetailModel BuildGitDetails(RepoPaths repoPaths, bool noCache)
        {
            var result = new GitDetailModel
            {
                Root = repoPaths.RootFolder,
                RelativePathToRoot = repoPaths.RelativePathToRoot
            };

            if (result.HasGit)
            {
                var configCmd = new GitConfigCommand(repoPaths, !noCache);
                var config = configCmd.GetCommandResults();

                var statusCmd = new GitStatusCommand(repoPaths, !noCache);
                var status = statusCmd.GetCommandResults();

                GetGitDir.TagRepoDir(config.RepoName, repoPaths.RootFolder);

                result.Branch =  GitUtils.Current.GetBranchName(repoPaths);
                result.User = config?.User;
                result.Ahead = (status?.Ahead) ?? 0;
                result.Behind = (status?.Behind) ?? 0;
                result.Staged = status?.Staged ?? 0;
                result.Unstaged = status?.Unstaged ?? 0;
                result.IsDetached = status.Detached;
                result.DetachedAt = status.DetachedAt;

                result.FileChanges = status?.FileChanges;
            }

            return result;
        }
    }
}
