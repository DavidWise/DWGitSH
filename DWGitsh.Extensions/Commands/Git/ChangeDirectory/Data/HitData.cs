using System;
using System.Collections.Generic;
using System.Linq;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data
{

    [Serializable]
    public class HitData
    {
        public string Name { get; set; }
        public string Directory { get; set; }
        public string Alias { get; set; }
        public string LastBranch { get; set; }
        public int HitCount { get; set; }
        public DateTime DateLastHit { get; set; }
    }
}
