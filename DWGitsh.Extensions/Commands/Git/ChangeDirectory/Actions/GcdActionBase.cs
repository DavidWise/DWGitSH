using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions
{
    public interface IGcdAction
    {
        bool Process(GitChangeDirectoryInfo info);
    }

    internal abstract class GcdActionBase : IGcdAction
    {

        public string ActionName { get; private set; }

        protected IHitDataManager _hitManager;
        protected IGitChangeDirectoryOptions _options;
        protected IRepositoryPaths _repoPaths;

        protected GcdActionBase(string actionName, IRepositoryPaths repoPaths, IGitChangeDirectoryOptions options, IHitDataManager hitManager)
        {
            _hitManager = hitManager;
            _options = options;
            _repoPaths = repoPaths;
            this.ActionName = actionName;
        }

        internal bool IsUnderGitRepo
        {
            get
            {
                if (!string.IsNullOrEmpty(_repoPaths?.RepositoryFolder)) return true;
                return false;
            }
        }

        public bool Process(GitChangeDirectoryInfo info)
        {
            if (!_options.DoneProcessing && ShouldProcessCommand())
                return TakeAction(info);
            else
                return false;
        }

        protected abstract bool TakeAction(GitChangeDirectoryInfo info);

        protected abstract bool ShouldProcessCommand();

        protected IEnumerable<HitDataViewModel> GetHitData()
        {
            var data = _hitManager.GetHitList()
                .OrderByDescending(x => x.HitCount)
                .ThenBy(x => x.Directory)
                .ToViewModel();

            if (data == null) data = new List<HitDataViewModel>().AsEnumerable();

            return data.ToArray();
        }


        public static IEnumerable<HitDataViewModel> ResolveMatches(string value, IEnumerable<HitDataViewModel> data)
        {
            var matches = FindMatches(value, data);

            // TODO: - need a way to tell the script that no matches were found and to use the full list
            //if (matches == null) matches = data;

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


    }
}
