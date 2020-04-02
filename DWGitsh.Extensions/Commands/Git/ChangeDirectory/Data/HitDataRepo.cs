using Newtonsoft.Json;
using StaticAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data
{
    internal interface IHitDataRepo
    {
        CommandData Load();
        void Save(CommandData data);
    }

    internal class HitDataRepo : IHitDataRepo
    {
        protected IStaticAbstraction _diskManager;
        protected string _hitDataPath;

        public HitDataRepo(string dataFolder, IStaticAbstraction diskManager)
        {
            _diskManager = diskManager;
            _hitDataPath = _diskManager.Path.Combine(dataFolder, "hitData.json");
        }

        public virtual CommandData Load()
        {
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

        public virtual void Save(CommandData data)
        {
            var json = JsonConvert.SerializeObject(data);
            _diskManager.File.WriteAllText(_hitDataPath, json);
        }
    }
}
