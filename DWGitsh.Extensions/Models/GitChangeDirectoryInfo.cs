using DWGitsh.Extensions.Commands.Git.ChangeDirectory;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using System.Collections.Generic;

namespace DWGitsh.Extensions.Models
{
    public class GitChangeDirectoryInfo
    {
        public string TargetDirectory { get; set; }

        public IEnumerable<HitDataViewModel> ListData { get; set; }

        public IGitChangeDirectoryOptions Options { get; set; }

        public bool PromptForListSelector { get; set; }

        public List<string> Messages { get; set; }

        public GitChangeDirectoryInfo()
        {
            this.Messages = new List<string>();
        }
    }
}
