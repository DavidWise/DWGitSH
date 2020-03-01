using DWGitsh.Extensions.Cmdlets;
using DWGitsh.Extensions.Models;
using StaticAbstraction;
using StaticAbstraction.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory
{
    public class GetGitChangeDirectoryCommand : GitCommandBase<GitChangeDirectory, GetGitChangeDirectoryParser>
    {
        protected GetGitChangeDirectoryCommandOptions Options { get; set; }

        public bool ExitWithoutOutput { get; protected set; }

        public IDirectoryInfo DataFolder { get; set; }


        public GetGitChangeDirectoryCommand(RepoPaths repoDirs, GetGitChangeDirectoryCommandOptions options) : base(repoDirs, false)
        {
            this.Options = options;

            this.ExitWithoutOutput = (this.RepositoryDirectories.RepositoryFolder == null || this.Options.LogOnly);
            this.Parser = new GetGitChangeDirectoryParser(this);

            FindDataFolder();
        }

        protected void FindDataFolder()
        {
            if (this.DataFolder != null) return;
            // LOH - need to get and create folders under appdata
        }

        public void Process()
        {
            if (Options.Log || Options.LogOnly) LogCurrentDirectory();

            if (this.ExitWithoutOutput) return;


        }

        protected void LogCurrentDirectory()
        {
            if (this.RepositoryDirectories.RepositoryFolder == null) return;

        }
    }



    public class GetGitChangeDirectoryCommandOptions
    {
        public bool Log { get; set;  }
        public bool LogOnly { get; set;  }
        public bool Last { get; set;  }
    }
}
