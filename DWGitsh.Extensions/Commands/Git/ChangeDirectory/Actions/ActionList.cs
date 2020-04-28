using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions
{
    internal class ActionList : GcdActionBase
    {
        public ActionList(GetGitChangeDirectoryCommandOptions options, HitDataManager hitManager)
            : base("List", options, hitManager)
        {

        }

        protected override bool ShouldProcessCommand()
        {
            return _options.List;
        }

        protected override bool TakeAction(GitChangeDirectoryInfo info)
        {
            var data = GetHitData();

            info.ListData = data;
            return true;
        }
    }
}
