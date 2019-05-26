using System.Collections.Generic;
using DWGitsh.Extensions.Commands.Git.Status;

namespace DWGitsh.Extensions.Models
{
    public class GitStatus
    {
        public string Branch { get; set; }
        public int Behind { get; set; }
        public int Ahead { get; set; }
        public int Staged { get; set; }
        public int Unstaged { get; set; }
        public bool Detached { get; set; }
        public string DetachedMarker { get; set; }
        public string DetachedAt { get; set; }

        public List<GitFileEntry> FileChanges { get; set; }
    }
}
