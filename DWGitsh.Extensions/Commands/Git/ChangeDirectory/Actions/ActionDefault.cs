using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Models;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions
{
    internal class ActionDefault : GcdActionBase
    {
        public ActionDefault(IRepositoryPaths repoPaths, IGitChangeDirectoryOptions options, IHitDataManager hitManager)
            : base("Default", repoPaths, options, hitManager)
        {

        }

        protected override bool ShouldProcessCommand()
        {
            var anythingSelected = _options.Last || _options.List || _options.Log || _options.LogOnly || _options.RemoveAlias ||
                    !string.IsNullOrEmpty(_options.NameOrAlias) || !string.IsNullOrEmpty(_options.Alias);

            return !anythingSelected;
        }

        protected override bool TakeAction(GitChangeDirectoryInfo info)
        {
            var data = GetHitData();

            info.ListData = data;
            info.Options.List = true;
            info.PromptForListSelector = true;

            return true;
        }
    }
}
