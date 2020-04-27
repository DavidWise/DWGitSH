using DWGitsh.Extensions.Cmdlets;
using DWGitsh.Extensions.Models;
using StaticAbstraction;
using StaticAbstraction.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Utility;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory
{
    public class GetGitChangeDirectoryCommand : GitCommandBase<GitChangeDirectory, GetGitChangeDirectoryParser>
    {
        protected GetGitChangeDirectoryCommandOptions Options { get; set; }

        public bool ExitWithoutOutput { get; protected set; }

        protected bool IsUnderGitRepo =>  (this.RepositoryDirectories != null && this.RepositoryDirectories.RepositoryFolder != null);


        private HitDataManager _hitManager;

        public GetGitChangeDirectoryCommand(RepoPaths repoDirs, GetGitChangeDirectoryCommandOptions options) : base(repoDirs, false)
        {
            this.Options = options;

            this.ExitWithoutOutput = this.Options.LogOnly;
            this.Parser = new GetGitChangeDirectoryParser(this);

            _hitManager = new HitDataManager(AppDataFolder, _diskManager, repoDirs);
        }



        public GitChangeDirectoryInfo Process()
        {
            var result = new GitChangeDirectoryInfo
            { 
                Options = this.Options
            };

            Action_LastDirectory(result);
            Action_Log(result);

            if (this.ExitWithoutOutput) return result;

            Action_List(result); 

            Action_NameOrAlias(result);

            Action_SetAlias(result);
            
            return result;
        }

        #region Command Actions

        protected void Action_LastDirectory(GitChangeDirectoryInfo info)
        {
            if (Options.Last) info.TargetDirectory = _hitManager.GetLastUsedFolder();
        }


        protected void Action_Log(GitChangeDirectoryInfo info)
        {
            if (!IsUnderGitRepo) return;
            if (Options.Log || Options.LogOnly) _hitManager.LogCurrentDirectory();
        }

        protected void Action_NameOrAlias(GitChangeDirectoryInfo info)
        {
            var targetName = this.Options.NameOrAlias?.Trim();
            var listData = GetHitListData();

            if (string.IsNullOrEmpty(targetName))
            {
                info.ListData = listData;
                info.PromptForListSelector = !this.Options.List;
                info.Options.List = true;
                return;
            }

            if (targetName == "/" || targetName == "\\")
            {
                info.TargetDirectory = this.RepositoryDirectories.RootFolder;
                return;
            }

            if (targetName == "-")
            {
                var target = listData.OrderByDescending(x => x.DateLastHit)
                    .FirstOrDefault(x => x.Directory.IsSameFolder(this.RepositoryDirectories.RootFolder) == false);

                if (target != null)
                {
                    info.TargetDirectory = target.Directory;
                    return;
                }
            }

            var matches = ResolveMatches(targetName, listData);
            if (matches.Count() == 1)
            {
                info.TargetDirectory = matches.Single().Directory;
                return;
            }

            info.ListData = matches;
            info.Options.List = true;
            info.PromptForListSelector = true;
        }

        protected void Action_SetAlias(GitChangeDirectoryInfo info)
        {
            if (string.IsNullOrEmpty(this.Options.Alias)) return;
            var targetName = this.Options.NameOrAlias?.Trim();
            if (string.IsNullOrEmpty(targetName)) return;

            var listData = GetHitListData();

            var matches = ResolveMatches(targetName, listData);

            if (matches == null || !matches.Any())
            {
                info.Messages.Add($"Unable to set alias - found no match for '{targetName}'");
                return;
            }

            if (matches.Count() > 1)
            {
                info.Messages.Add($"Unable to set alias - found more than one match for '{targetName}'");
                return;
            }

            _hitManager.SetAlias(matches.Single().Directory, this.Options.Alias);
        }

        public static IEnumerable<HitDataViewModel> ResolveMatches(string value, IEnumerable<HitDataViewModel> data )
        {
            var matches = FindMatches(value, data);

            if (matches == null) matches = data;

            return matches.FixOrdinal();
        }

        protected static IEnumerable<HitDataViewModel> FindMatches(string value, IEnumerable<HitDataViewModel> data)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            // match on ordinal first
            int pos = 0;
            if (Int32.TryParse(value, out pos))
            {
                var match = data.SingleOrDefault(x => x.Ordinal == pos);

                if (match != null)
                {
                    return new List<HitDataViewModel> { match };
                }
            }

            // now match on Alias
            var matches = data.Where(x => string.Compare(value, x.Alias, StringComparison.InvariantCultureIgnoreCase) == 0);
            if (matches != null && matches.Any())
            {
                return matches.ToArray();
            }

            // now try to find a partial match
            var partials = data.Where(x => x.Directory.IndexOf(value, StringComparison.InvariantCultureIgnoreCase) >= 0);
            if (partials != null && partials.Any())
            {
                return partials.ToArray();
            }

            return null;
        }


        protected void Action_List(GitChangeDirectoryInfo info)
        {
            IEnumerable<HitDataViewModel> data = null;

            if (!Options.List)
                data = new List<HitDataViewModel>().AsEnumerable();
            else
                data = GetHitListData();

            info.ListData = data;
        }

        protected IEnumerable<HitDataViewModel> GetHitListData()
        {
            var data = _hitManager.GetHitList()
                .OrderByDescending(x => x.HitCount)
                .ThenBy(x => x.Directory)
                .ToViewModel();

            if (data == null) data = new List<HitDataViewModel>().AsEnumerable();

            return data.ToArray();
        }

        #endregion
    }


    public class GetGitChangeDirectoryCommandOptions
    {
        public string NameOrAlias { get; set;  }
        public bool Log { get; set;  }
        public bool LogOnly { get; set;  }
        public bool Last { get; set;  }
        public bool List { get; set;  }
        public string Alias { get; set; }
    }
}
