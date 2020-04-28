using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Models;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions
{
    internal class ActionLog : GcdActionBase
    {
        internal bool IsUnderGitRepo { get; private set; }

        public ActionLog(IRepositoryPaths repoPaths, GetGitChangeDirectoryCommandOptions options, IHitDataManager hitManager) 
            : base("Log", options, hitManager)
        {
            this.IsUnderGitRepo = !string.IsNullOrEmpty(repoPaths?.RepositoryFolder);
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
