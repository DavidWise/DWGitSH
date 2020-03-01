using DWGitsh.Extensions.Cmdlets.Common;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory;
using DWGitsh.Extensions.Commands.Git.Config;
using DWGitsh.Extensions.Models;
using System.Management.Automation;

namespace DWGitsh.Extensions.Cmdlets
{
    [Cmdlet(VerbsCommon.Get,"GitChangeDirectory")]
    [OutputType(typeof(GitChangeDirectoryResult))]
    public class GetGitChangeDirectory : GitCmdletBase<GitChangeDirectoryResult>
    {
        [Parameter]
        public SwitchParameter Log;
        [Parameter]
        public SwitchParameter LogOnly;
        [Parameter]
        public SwitchParameter Last;

        public GetGitChangeDirectory() : base() { }

        protected override GitChangeDirectoryResult BuildResponse()
        {
            GitChangeDirectoryResult result = null;

            var options = new GetGitChangeDirectoryCommandOptions
            {
                Log = this.Log.IsPresent || this.LogOnly.IsPresent,
                LogOnly = this.LogOnly.IsPresent,
                Last = this.Last.IsPresent
            };

            var cmd = new GetGitChangeDirectoryCommand(this.RepositoryDirectories, options);

            // no output is produced for commands that trigger exitnow
            if (cmd.ExitWithoutOutput) return result;

            return new GitChangeDirectoryResult("Hi from GetGitChangeDirectory");
        }
    }

    public class GitChangeDirectoryResult
    {
        private string _message;
        public GitChangeDirectoryResult()
        {

        }
        public GitChangeDirectoryResult(string msg)
        {
            _message = msg;
        }
        public override string ToString()
        {
            return _message;
        }
    }
}
