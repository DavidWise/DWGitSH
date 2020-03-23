using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DWGitsh.Extensions.Models;
using Newtonsoft.Json;
using StaticAbstraction;
using StaticAbstraction.IO;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data
{
    internal class HitDataManager
    {
        public IRepositoryPaths RepositoryDirectories { get; protected set; }
        protected IDirectoryInfo _dataFolder;
        protected IStaticAbstraction _diskManager;
        protected string _hitDataPath;
        public IDirectoryInfo DataFolder
        {
            get
            {
                if (_dataFolder == null) _dataFolder = FindDataFolder();
                return _dataFolder;
            }
        }


        public HitDataManager(IStaticAbstraction diskManager, IRepositoryPaths repoDirs)
        {
            this._diskManager = diskManager ?? new StAbWrapper();
            this.RepositoryDirectories = repoDirs;
        }


        protected IDirectoryInfo FindDataFolder()
        {
            var basePath = _diskManager.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var dataFolder = _diskManager.Path.Combine(basePath, "DWGitsh");

            if (!_diskManager.Directory.Exists(dataFolder)) _diskManager.Directory.CreateDirectory(dataFolder);

            var result  = _diskManager.NewDirectoryInfo(dataFolder);

            _hitDataPath = _diskManager.Path.Combine(dataFolder, "hitData.json");

            return result;
        }


        public virtual CommandData ReadHitData()
        {
            if (_hitDataPath == null) FindDataFolder();

            CommandData result = null;

            if (_diskManager.File.Exists(_hitDataPath))
            {
                var rawData = _diskManager.File.ReadAllText(_hitDataPath);
                result = JsonConvert.DeserializeObject<CommandData>(rawData);
            }
            else
                result = new CommandData();

            return result;
        }

        public virtual void LogCurrentDirectory()
        {
            if (this.RepositoryDirectories.RepositoryFolder == null) return;

            var data = ReadHitData();
            var updated = false;

            foreach(var item in data.Repositories)
            {
                if (string.Compare(item.Directory, this.RepositoryDirectories.RepositoryFolder, true) == 0)
                {
                    item.HitCount++;
                    item.DateLastHit = DateTime.Now;
                    updated = true;
                }
            }

            if (!updated)
            {
                var newHitData = new HitData { Directory = this.RepositoryDirectories.RepositoryFolder, HitCount = 1, DateLastHit = DateTime.Now };
                data.Repositories.Add(newHitData);
            }

            WriteHitData(data);
        }

        protected void WriteHitData(CommandData data)
        {
            var json = JsonConvert.SerializeObject(data);
            _diskManager.File.WriteAllText(_hitDataPath, json);
        }
    }
}
