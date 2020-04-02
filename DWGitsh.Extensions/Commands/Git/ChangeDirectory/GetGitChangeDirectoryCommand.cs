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

            if (Options.Last) result.LastPath = _hitManager.GetLastUsedFolder();

            if (Options.Log || Options.LogOnly) _hitManager.LogCurrentDirectory();

            if (this.ExitWithoutOutput) return result;

            return result;
        }
    }


    public class GetGitChangeDirectoryCommandOptions
    {
        public bool Log { get; set;  }
        public bool LogOnly { get; set;  }
        public bool Last { get; set;  }
        public bool List { get; set;  }
    }
}
