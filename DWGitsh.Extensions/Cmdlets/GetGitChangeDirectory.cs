using DWGitsh.Extensions.Cmdlets.Common;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory;
using DWGitsh.Extensions.Commands.Git.Config;
using DWGitsh.Extensions.Models;
using System.Management.Automation;

namespace DWGitsh.Extensions.Cmdlets
{
    [Cmdlet(VerbsCommon.Get,"GitChangeDirectory")]
    [OutputType(typeof(GitChangeDirectoryInfo))]
    public class GetGitChangeDirectory : GitCmdletBase<GitChangeDirectoryInfo>
    {
        [Parameter]
        public string NameOrAlias { get; set; }

        [Parameter]
        public SwitchParameter Log;
        [Parameter]
        public SwitchParameter LogOnly;
        [Parameter]
        public SwitchParameter Last;
        [Parameter]
        public SwitchParameter List;

        public GetGitChangeDirectory() : base() { }

        protected override GitChangeDirectoryInfo BuildResponse()
        {
            var options = new GetGitChangeDirectoryCommandOptions
            {
                Log = this.Log.IsPresent || this.LogOnly.IsPresent,
                LogOnly = this.LogOnly.IsPresent,
                Last = this.Last.IsPresent,
                List = this.List.IsPresent
            };

            var cmd = new GetGitChangeDirectoryCommand(this.RepositoryDirectories, options);
            var cmdResult = cmd.Process();

            // no output is produced for commands that trigger exitnow
            if (cmd.ExitWithoutOutput) return cmdResult;

            // assuming something written to client here

            return cmdResult;
        }
    }
}
