using DWGitsh.Extensions.Cmdlets.Common;
using DWGitsh.Extensions.Commands.Git.Config;
using DWGitsh.Extensions.Models;
using System.Management.Automation;

namespace DWGitsh.Extensions.Cmdlets
{
    [Cmdlet(VerbsCommon.Get,"GitConfig")]
    [OutputType(typeof(GitConfig))]
    public class GetGitConfig : GitCmdletBase<GitConfig>
    {

        public GetGitConfig() : base() { }

        protected override GitConfig BuildResponse()
        {
            if (string.IsNullOrEmpty(this.RepositoryDirectories.RepositoryFolder)) return null;

            var configCmd = new GitConfigCommand(this.RepositoryDirectories, !this.NoCache.IsPresent);
            var config = configCmd.GetCommandResults();

            return config;
        }
    }
}
