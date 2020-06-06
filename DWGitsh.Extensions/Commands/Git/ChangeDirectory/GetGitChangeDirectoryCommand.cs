using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Commands.PowerShell;
using DWGitsh.Extensions.Models;
using DWGitsh.Extensions.Utility;
using System.Collections.Generic;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory
{
    public class GetGitChangeDirectoryCommand : PowerShellCommandBase<GitChangeDirectoryInfo>
    {
        protected IGitChangeDirectoryOptions Options { get; set; }

        protected bool IsUnderGitRepo =>  (this.RepositoryDirectories != null && this.RepositoryDirectories.RepositoryFolder != null);
        
        private IHitDataManager _hitManager;

        public GetGitChangeDirectoryCommand(IDWGitshCommonArgs commonArgs, IGitChangeDirectoryOptions options) : this(commonArgs, options, null) { }

        public GetGitChangeDirectoryCommand(IDWGitshCommonArgs commonArgs, IGitChangeDirectoryOptions options, IHitDataManager hitdataManager) : base(commonArgs)
        {
            this.Options = options;

            _hitManager = hitdataManager ?? new HitDataManager(_config, _diskManager, commonArgs.RepoPaths);
        }



        public override GitChangeDirectoryInfo Process()
        {
            var result = new GitChangeDirectoryInfo
            {
                Options = new GetGitChangeDirectoryCommandOptions(this.Options)
            };


            var actions = BuildActions();

            foreach (var action in actions)
                action.Process(result);


            return result;
        }


        protected List<IGcdAction> BuildActions()
        {
            var result = new List<IGcdAction>();
            var logAction = new ActionLog(this.RepositoryDirectories, this.Options, _hitManager);

            if (Options.LogOnly)
            {
                result.Add(logAction);
            }
            else
            {
                result = new List<IGcdAction>
                {
                    logAction,
                    new ActionDefault(this.RepositoryDirectories, this.Options, _hitManager),
                    new ActionLastDirectory(this.RepositoryDirectories, this.Options, _hitManager),
                    new ActionList(this.RepositoryDirectories, this.Options, _hitManager),
                    new ActionNameOrAlias(this.RepositoryDirectories, this.Options, _hitManager),
                    new ActionSetAlias(this.RepositoryDirectories, this.Options, _hitManager),
                    new ActionRemoveAlias(this.RepositoryDirectories, this.Options, _hitManager)
                };
            }

            return result;
        }

        // This is called from the helper powershell script
        public static IEnumerable<HitDataViewModel> ResolveMatches(string value, IEnumerable<HitDataViewModel> data)
        {
            return GcdActionBase.ResolveMatches(value, data);
        }
    }
}
