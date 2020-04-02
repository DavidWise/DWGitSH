using DWGitsh.Extensions.Commands.Git.ChangeDirectory;

namespace DWGitsh.Extensions.Models
{
    public class GitChangeDirectoryInfo
    {
        public string LastPath { get; set; }

        public GetGitChangeDirectoryCommandOptions Options { get; set; }
    }
}
