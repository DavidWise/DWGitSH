﻿using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Models;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions
{
    internal class ActionList : GcdActionBase
    {
        public ActionList(IRepositoryPaths repoPaths, IGitChangeDirectoryOptions options, IHitDataManager hitManager)
            : base("List", repoPaths, options, hitManager)
        {

        }

        protected override bool ShouldProcessCommand()
        {
            return _options.List;
        }

        protected override bool TakeAction(GitChangeDirectoryInfo info)
        {
            var data = GetHitData();

            info.ListData = data;
            return true;
        }
    }
}
