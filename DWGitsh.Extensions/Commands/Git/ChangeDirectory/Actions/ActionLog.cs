﻿using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Models;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions
{
    internal class ActionLog : GcdActionBase
    {

        public ActionLog(IRepositoryPaths repoPaths, IGitChangeDirectoryOptions options, IHitDataManager hitManager) 
            : base("Log", repoPaths, options, hitManager)
        {
        }

        protected override bool ShouldProcessCommand()
        {
            if (IsUnderGitRepo && (_options.Log || _options.LogOnly)) return true;
            return false;
        }

        protected override bool TakeAction(GitChangeDirectoryInfo info)
        {
            _hitManager.LogCurrentDirectory();

            if (_options.LogOnly) _options.DoneProcessing = true;
            return true;
        }
    }
}
