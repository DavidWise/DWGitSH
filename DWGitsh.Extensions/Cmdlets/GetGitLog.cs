using System.Management.Automation;
using DWGitsh.Extensions.Cmdlets.Common;
using DWGitsh.Extensions.Commands.Git.Log;
using DWGitsh.Extensions.Models;
using DWGitsh.Extensions.Utility;

namespace DWGitsh.Extensions.Cmdlets
{
    [Cmdlet(VerbsCommon.Get,"GitLog")]
    [OutputType(typeof(GitLogModel))]
    public class GetGitLog : GitCmdletBase<GitLogModel>
    {
        [Alias("Since")]
        [Parameter]
        public string After { get; set; }

        [Alias("Until")]
        [Parameter]
        public string Before { get; set; }


        public GetGitLog() : base() { }


        protected override GitLogModel BuildResponse()
        {
            if (string.IsNullOrEmpty(this.RepositoryDirectories.RepositoryFolder)) return null;

            var cmd = new GitLogCommand(this);
            var result = cmd.GetCommandResults();

            result.Branch = GitUtils.Current.GetBranchName(this.RepositoryDirectories);
            result.RootFolder = this.RepositoryDirectories.RootFolder;

            return result;
        }
    }
}
