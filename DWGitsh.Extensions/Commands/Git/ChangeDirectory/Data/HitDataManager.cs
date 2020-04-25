using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DWGitsh.Extensions.Models;
using DWGitsh.Extensions.Utility;
using Newtonsoft.Json;
using StaticAbstraction;
using StaticAbstraction.IO;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data
{
    internal class HitDataManager
    {
        public IRepositoryPaths RepositoryDirectories { get; protected set; }

        protected readonly IStaticAbstraction _diskManager;

        protected string _hitDataPath;
        protected IGitUtils _utils;
        protected IHitDataRepo _hitDataRepo;

        public HitDataManager(string dataFolder, IStaticAbstraction diskManager, IRepositoryPaths repoDirs, IGitUtils utils = null, IHitDataRepo hitRepo = null)
        {
            _utils = utils ?? GitUtils.Current;
            this._diskManager = diskManager ?? new StAbWrapper();
            this.RepositoryDirectories = repoDirs;

            _hitDataRepo = hitRepo ?? new HitDataRepo(dataFolder, _diskManager);
        }


        public virtual void LogCurrentDirectory()
        {
            if (this.RepositoryDirectories.RepositoryFolder == null) return;

            var data = _hitDataRepo.Load();
            var updated = false;
            var compareFolder = _utils.TrimTrailingSlash(this.RepositoryDirectories.RootFolder);

            var branch = _utils.GetBranchName(this.RepositoryDirectories);

            foreach (var item in data.Repositories)
            {
                var itemFolder = _utils.TrimTrailingSlash(item.Directory);
                if (string.Compare(itemFolder, compareFolder, true) == 0)
                {
                    item.HitCount++;
                    item.DateLastHit = DateTime.Now;
                    if (!string.IsNullOrEmpty(branch)) item.LastBranch = branch;
                    updated = true;
                }
            }

            if (!updated)
            {
                var newHitData = new HitData { Directory = this.RepositoryDirectories.RootFolder, HitCount = 1, DateLastHit = DateTime.Now };
                data.Repositories.Add(newHitData);
            }

            _hitDataRepo.Save(data);
        }

        public virtual string GetLastUsedFolder()
        {
            string result = null;

            var data = _hitDataRepo.Load();

            if (data != null && data.Repositories != null)
            {
                var mostRecent = data.Repositories.OrderByDescending(x => x.DateLastHit).FirstOrDefault();
                if (mostRecent != null) result = mostRecent.Directory;
            }

            return result;
        }

        public virtual List<HitData> GetHitList()
        {
            var data = _hitDataRepo.Load();
            return data.Repositories;
        }
    }
}
