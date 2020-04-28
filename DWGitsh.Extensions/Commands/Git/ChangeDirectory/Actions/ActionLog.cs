using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions
{
    internal class ActionLog : GcdActionBase
    {
        protected bool IsUnderGitRepo { get; private set; }

        public ActionLog(IRepositoryPaths repoPaths, GetGitChangeDirectoryCommandOptions options, HitDataManager hitManager) 
            : base("Log", options, hitManager)
        {
            this.IsUnderGitRepo = (repoPaths != null && repoPaths.RepositoryFolder != null);
        }

        protected override bool ShouldProcessCommand()
        {
            if (IsUnderGitRepo && (_options.Log || _options.LogOnly)) return true;
            return false;
        }

        protected override bool TakeAction(GitChangeDirectoryInfo info)
        {
            //if (!IsUnderGitRepo) return;
            _hitManager.LogCurrentDirectory();

            if (_options.LogOnly) _options.DoneProcessing = true;
            return true;
        }
    }
}
