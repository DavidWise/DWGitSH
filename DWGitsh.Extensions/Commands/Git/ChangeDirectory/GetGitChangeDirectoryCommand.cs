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

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory
{
    public class GetGitChangeDirectoryCommand : GitCommandBase<GitChangeDirectory, GetGitChangeDirectoryParser>
    {
        protected GetGitChangeDirectoryCommandOptions Options { get; set; }

        public bool ExitWithoutOutput { get; protected set; }

        private HitDataManager _hitManager;

        public GetGitChangeDirectoryCommand(RepoPaths repoDirs, GetGitChangeDirectoryCommandOptions options) : base(repoDirs, false)
        {
            this.Options = options;

            this.ExitWithoutOutput = (this.RepositoryDirectories.RepositoryFolder == null || this.Options.LogOnly);
            this.Parser = new GetGitChangeDirectoryParser(this);

            _hitManager = new HitDataManager(AppDataFolder, _diskManager, repoDirs);
        }



        public GitChangeDirectoryInfo Process()
        {
            var result = new GitChangeDirectoryInfo
            { 
                Options = this.Options
            };

            Action_LastDirectory(result);
            Action_Log(result);

            if (this.ExitWithoutOutput) return result;

            Action_NameOrAlias(result);

            Action_List(result); 

            return result;
        }

        #region Command Actions

        protected void Action_LastDirectory(GitChangeDirectoryInfo info)
        {
            if (Options.Last) info.TargetDirectory = _hitManager.GetLastUsedFolder();
        }


        protected void Action_Log(GitChangeDirectoryInfo info)
        {
            if (Options.Log || Options.LogOnly) _hitManager.LogCurrentDirectory();
        }

        protected void Action_NameOrAlias(GitChangeDirectoryInfo info)
        {
            var targetName = this.Options.NameOrAlias?.Trim();
            if (string.IsNullOrEmpty(targetName)) return;

            if (targetName == "/" || targetName == "\\")
            {
                info.TargetDirectory = this.RepositoryDirectories.RootFolder;
                return;
            }

            // TODO: Handle partial matches, aliases, etc..
        }


        protected void Action_List(GitChangeDirectoryInfo info)
        {
            IEnumerable<HitDataViewModel> data = null;

            if (!Options.List) 
                data = new List<HitDataViewModel>().AsEnumerable();
            else 
                data = _hitManager.GetHitList()
                    .OrderByDescending(x => x.HitCount)
                    .ThenBy(x => x.Directory)
                    .ToViewModel();

            info.ListData = data;
        }

        #endregion
    }


    public class GetGitChangeDirectoryCommandOptions
    {
        public string NameOrAlias { get; set;  }
        public bool Log { get; set;  }
        public bool LogOnly { get; set;  }
        public bool Last { get; set;  }
        public bool List { get; set;  }
    }
}
