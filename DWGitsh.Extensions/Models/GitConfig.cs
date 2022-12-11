using System.Collections.Generic;

namespace DWGitsh.Extensions.Models
{
    public class GitConfig
    {
        public GitConfigUser User { get; set; }
        public string RepoOwner { get; set; }
        public string RepoName { get; set; }
        public string RepoUrl { get; set; }
        public bool IsWindowsTerminal { get; set; }

        public Dictionary<string, string> Values { get; set; }

        public override string ToString()
        {
            if (User == null) return string.Empty;

            return $"{{user={User.ToString()}}}";
        }
    }
}