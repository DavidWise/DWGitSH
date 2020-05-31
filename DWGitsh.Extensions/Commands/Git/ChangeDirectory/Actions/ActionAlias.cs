using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Models;
using System.Linq;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions
{
    internal abstract class ActionAlias : GcdActionBase
    {
        public string TargetName { get; set; }

        protected ActionAlias(string actionName, IGitChangeDirectoryOptions options, IHitDataManager hitManager) : base(actionName, options, hitManager)
        {
            TargetName = options.NameOrAlias;
        }

        protected override bool TakeAction(GitChangeDirectoryInfo info)
        {
            var processed = false;

            if (!string.IsNullOrEmpty(TargetName))
            {
                var listData = GetHitData();

                var matches = ResolveMatches(TargetName, listData);

                if (matches == null || !matches.Any())
                {
                    info.Messages.Add($"Unable to {ActionName} alias - found no match for '{TargetName}'");
                }
                else if (matches.Count() > 1)
                {
                    info.Messages.Add($"Unable to {ActionName} alias - found more than one match for '{TargetName}'");
                }
                else
                {
                    var newAlias = GetAlias();
                    _hitManager.SetAlias(matches.Single().Directory, newAlias);
                    processed = true;
                }
            }

            return processed;
        }

        protected virtual string GetAlias()
        {
            return _options.Alias;
        }
    }

    internal class ActionSetAlias : ActionAlias
    {
        public ActionSetAlias(IGitChangeDirectoryOptions options, IHitDataManager hitManager) : base("Set", options, hitManager) { }

        protected override bool ShouldProcessCommand()
        {
            if (string.IsNullOrWhiteSpace(_options.NameOrAlias) || string.IsNullOrWhiteSpace(_options.Alias)) return false;
            return true;
        }
    }

    internal class ActionRemoveAlias : ActionAlias
    {
        public ActionRemoveAlias(IGitChangeDirectoryOptions options, IHitDataManager hitManager) : base("Remove", options, hitManager) { }

        protected override bool ShouldProcessCommand()
        {
            if (string.IsNullOrEmpty(_options.NameOrAlias) || !_options.RemoveAlias) return false;
            return true;
        }

        protected override string GetAlias()
        {
            return null;
        }
    }
}
