using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Models;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions
{
    internal class ActionLastDirectory : GcdActionBase
    {
        public ActionLastDirectory(IRepositoryPaths repoPaths, IGitChangeDirectoryOptions options, IHitDataManager hitManager) 
            : base("LastDirectory", repoPaths, options, hitManager)
        {

        }

        protected override bool ShouldProcessCommand()
        {
            return _options.Last;
        }

        protected override bool TakeAction(GitChangeDirectoryInfo info)
        {
            info.TargetDirectory = _hitManager.GetLastUsedFolder();
            return true;
        }
    }
}
