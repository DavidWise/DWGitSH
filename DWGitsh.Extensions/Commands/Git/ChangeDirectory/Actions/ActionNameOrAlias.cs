using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Models;
using DWGitsh.Extensions.Utility;
using System.Collections.Generic;
using System.Linq;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions
{
    internal class ActionNameOrAlias : GcdActionBase
    {
        public IRepositoryPaths RepositoryDirectories { get; protected set; }
        public ActionNameOrAlias(IRepositoryPaths repoPaths, IGitChangeDirectoryOptions options, IHitDataManager hitManager) : base("NameOrAlias", options, hitManager)
        {
            this.RepositoryDirectories = repoPaths;
        }

        protected override bool ShouldProcessCommand()
        {
            return true;
        }

        protected override bool TakeAction(GitChangeDirectoryInfo info)
        {
            var targetName = _options.NameOrAlias;
            var listData = GetHitData();

            if (string.IsNullOrEmpty(targetName)) return HandleNoTarget(info, listData);

            if (targetName == "/" || targetName == "\\") return HandleRootTarget(info);

            if (targetName == "-") return HandlePreviousTarget(info, listData);

            var matches = ResolveMatches(targetName, listData);

            if (matches != null && matches.Count() == 1) return HandleSingleTarget(info, matches.Single());

            return HandleMultipleTarget(info, matches ?? listData);
        }

        protected bool HandleNoTarget(GitChangeDirectoryInfo info, IEnumerable<HitDataViewModel> targets)
        {
            info.ListData = targets;
            info.PromptForListSelector = !_options.List;
            info.Options.List = true;

            return true;
        }

        protected bool HandleRootTarget(GitChangeDirectoryInfo info)
        {
            info.TargetDirectory = this.RepositoryDirectories.RootFolder;

            return true;
        }

        protected bool HandlePreviousTarget(GitChangeDirectoryInfo info, IEnumerable<HitDataViewModel> targets)
        {
            var target = targets.OrderByDescending(x => x.DateLastHit)
                .FirstOrDefault(x => x.Directory.IsSameFolder(this.RepositoryDirectories.RootFolder) == false);

            if (target != null)
            {
                info.TargetDirectory = target.Directory;
            }

            return true;
        }

        protected bool HandleSingleTarget(GitChangeDirectoryInfo info, HitDataViewModel target)
        {
            info.TargetDirectory = target.Directory;
            return true;
        }

        protected bool HandleMultipleTarget(GitChangeDirectoryInfo info, IEnumerable<HitDataViewModel> targets)
        {
            info.ListData = targets;
            info.Options.List = true;
            info.PromptForListSelector = true;

            return true;
        }
    }
}
