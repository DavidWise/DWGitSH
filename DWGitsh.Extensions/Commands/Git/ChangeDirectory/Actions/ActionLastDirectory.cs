using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions
{
    internal class ActionLastDirectory : GcdActionBase
    {
        public ActionLastDirectory(GetGitChangeDirectoryCommandOptions options, HitDataManager hitManager) 
            : base("LastDirectory", options, hitManager)
        {

        }

        protected override bool ShouldProcessCommand()
        {
            return _options.Last;
        }

        protected override bool TakeAction(GitChangeDirectoryInfo info)
        {
            info.TargetDirectory = _hitManager.GetLastUsedFolder();
            return true;
        }
    }
}
