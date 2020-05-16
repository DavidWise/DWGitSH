using StaticAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWGitsh.Extensions.Config
{
    public interface IDWGitshConfig
    {
        string AppDataFolder { get; }
    }

    public class DWGitshConfig : IDWGitshConfig
    {
        public string AppDataFolder { get; protected set; }

        protected IStaticAbstraction _diskManager;

        public DWGitshConfig(IStaticAbstraction diskManager)
        {
            _diskManager = diskManager;
            this.AppDataFolder = FindAppDataFolder(_diskManager);
        }

        private static string FindAppDataFolder(IStaticAbstraction diskManager)
        {
            var basePath = diskManager.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var dataFolder = diskManager.Path.Combine(basePath, "DWGitsh");

            if (!diskManager.Directory.Exists(dataFolder)) diskManager.Directory.CreateDirectory(dataFolder);

            return dataFolder;
        }

    }
}
