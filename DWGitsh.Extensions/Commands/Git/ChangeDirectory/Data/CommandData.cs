using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data
{
    [Serializable]
    internal class CommandData
    {
        public List<HitData> Repositories { get; set;  }

        public CommandData()
        {
            this.Repositories = new List<HitData>();
        }
    }
}
