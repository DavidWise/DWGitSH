using System.Collections.Generic;
using DWGitsh.Extensions.Commands.Git.Status;

namespace DWGitsh.Extensions.Models
{
    public class GitDetailModel
    {
        public bool HasGit
        {
            get { return !string.IsNullOrWhiteSpace(Root); }
        }
        public string Branch { get; set; }
        public string Root { get; set; }
        public string RelativePathToRoot { get; set; }
        public int Ahead { get; set; }
        public int Behind { get; set; }
        public int Staged { get; set; }
        public int Unstaged { get; set; }
        public GitConfigUser User { get; set; }

        public bool IsDetached { get; set; }
        public string DetachedAt { get; set; }

        public List<GitFileEntry> FileChanges { get; set; }

    }
}