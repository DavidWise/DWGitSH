using DWGitsh.Extensions.Cmdlets;
using DWGitsh.Extensions.Models;
using StaticAbstraction;
using StaticAbstraction.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Utility;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory
{
    public class GetGitChangeDirectoryCommand : GitCommandBase<GitChangeDirectory, GetGitChangeDirectoryParser>
    {
        protected GetGitChangeDirectoryCommandOptions Options { get; set; }

        public bool ExitWithoutOutput { get; protected set; }

        protected bool IsUnderGitRepo =>  (this.RepositoryDirectories != null && this.RepositoryDirectories.RepositoryFolder != null);


        private HitDataManager _hitManager;

        public GetGitChangeDirectoryCommand(RepoPaths repoDirs, GetGitChangeDirectoryCommandOptions options) : base(repoDirs, false)
        {
            this.Options = options;

            this.ExitWithoutOutput = this.Options.LogOnly;
            this.Parser = new GetGitChangeDirectoryParser(this);

            _hitManager = new HitDataManager(AppDataFolder, _diskManager, repoDirs);
        }



        public GitChangeDirectoryInfo Process()
        {
            var result = new GitChangeDirectoryInfo
            { 
                Options = this.Options
            };


            var actions = BuildActions();

            foreach (var action in actions)
                action.Process(result);


            return result;
        }


        protected List<IGcdAction> BuildActions()
        {
            var result = new List<IGcdAction>
            {
                new ActionLog(this.RepositoryDirectories, this.Options, _hitManager),
                new ActionLastDirectory(this.Options, _hitManager),
                new ActionList(this.Options, _hitManager),
                new ActionNameOrAlias(this.RepositoryDirectories, this.Options, _hitManager),
                new ActionSetAlias(this.Options, _hitManager),
                new ActionRemoveAlias(this.Options, _hitManager)
            };

            return result;
        }

        // This is called from the helper powershell script
        public static IEnumerable<HitDataViewModel> ResolveMatches(string value, IEnumerable<HitDataViewModel> data)
        {
            return GcdActionBase.ResolveMatches(value, data);
        }
    }
}
