using DWGitsh.Extensions.Commands.Git.ChangeDirectory;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using System.Collections.Generic;

namespace DWGitsh.Extensions.Models
{
    public class GitChangeDirectoryInfo
    {
        public string TargetDirectory { get; set; }

        public IEnumerable<HitDataViewModel> ListData { get; set; }

        public GetGitChangeDirectoryCommandOptions Options { get; set; }
    }
}
