using DWGitsh.Extensions.Models;
using DWGitsh.Extensions.Utility;
using DWPowerShell.Utility.Cmdlet;
using System;
using System.Management.Automation;

namespace DWGitsh.Extensions.Cmdlets.Common
{
    public abstract class GitCmdletBase<TReturn> : DWPSCmdletBase, ICmdletWriterContainer where TReturn:class, new()
    {
        private RepoPaths _repositoryDirectories;

        [Parameter]
        public SwitchParameter NoCache { get; set; }

        public ICmdletWriter CmdletWriter { get; set; }


        public GitCmdletBase() : base()
        {
            this.CmdletWriter = new GitCmdletWriter(this);
        }


        protected override void ProcessRecord()
        {
            try
            {
                var result = BuildResponse();
                WriteObject(result);
            }
            catch (Exception ex)
            {
                var procname = this.MyInvocation?.InvocationName ?? "GitCmdletbase";
                WriteError(new ErrorRecord(ex, $"Error processing command '{procname}'", ErrorCategory.InvalidOperation, null));
            }
        }


        public RepoPaths RepositoryDirectories
        {
            get
            {
                if (_repositoryDirectories == null)
                {
                    _repositoryDirectories = GitUtils.Current.GetRepoPaths(this.ScriptPath, NoCache.IsPresent);
                }

                return _repositoryDirectories;
            }
        }

        protected abstract TReturn BuildResponse();
    }
}
