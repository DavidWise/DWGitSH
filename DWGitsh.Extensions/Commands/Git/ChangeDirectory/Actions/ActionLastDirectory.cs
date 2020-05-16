using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Models;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions
{
    internal class ActionLastDirectory : GcdActionBase
    {
        public ActionLastDirectory(IGitChangeDirectoryOptions options, IHitDataManager hitManager) 
            : base("LastDirectory", options, hitManager)
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
