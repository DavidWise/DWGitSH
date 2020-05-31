using DWGitsh.Extensions.Cmdlets.Common;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory;
using DWGitsh.Extensions.Commands.Git.Config;
using DWGitsh.Extensions.Models;
using DWGitsh.Extensions.Utility;
using System.Management.Automation;

namespace DWGitsh.Extensions.Cmdlets
{
    [Cmdlet(VerbsCommon.Get,"GitChangeDirectory")]
    [OutputType(typeof(GitChangeDirectoryInfo))]
    public class GetGitChangeDirectory : GitCmdletBase<GitChangeDirectoryInfo>
    {
        [Parameter(Position = 0)]
        public string NameOrAlias { get; set; }

        [Parameter]
        public SwitchParameter Log;
        [Parameter]
        public SwitchParameter LogOnly;
        [Parameter]
        public SwitchParameter Last;
        [Parameter]
        public SwitchParameter List;

        [Parameter]
        public string Alias;
        [Parameter]
        public SwitchParameter RemoveAlias;

        public GetGitChangeDirectory() : base() { }

        protected override GitChangeDirectoryInfo BuildResponse()
        {
            var options = new GetGitChangeDirectoryCommandOptions(this);

            var commonArgs = new DWGitshCommonArgs(null, null, this.RepositoryDirectories);

            var cmd = new GetGitChangeDirectoryCommand(commonArgs, options);

            var cmdResult = cmd.Process();

            return cmdResult;
        }
    }
}
