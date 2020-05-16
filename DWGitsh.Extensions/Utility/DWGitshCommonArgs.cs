using DWGitsh.Extensions.Config;
using DWGitsh.Extensions.Models;
using DWPowerShell.Utility.Cache;
using StaticAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWGitsh.Extensions.Utility
{
    public interface IDWGitshCommonArgs
    {
        IDWGitshConfig Config { get;  }
        IStaticAbstraction DiskManager { get;  }
        IRepositoryPaths RepoPaths { get;  }
        IGitUtils Utils { get; }
        ICacheContainer Cache { get; }
    }

    public class DWGitshCommonArgs : IDWGitshCommonArgs
    {
        public IStaticAbstraction DiskManager { get; set; }
        public IRepositoryPaths RepoPaths { get; set; }
        public IDWGitshConfig Config { get; set; }
        public IGitUtils Utils { get; set; }
        public ICacheContainer Cache { get; set; }

        public DWGitshCommonArgs() : this(null, null, null, null, null) { }
        public DWGitshCommonArgs(IStaticAbstraction diskManager) : this(diskManager, null, null, null, null) { }
        public DWGitshCommonArgs(IStaticAbstraction diskManager, IDWGitshConfig config) : this(diskManager, config, null, null, null) { }
        public DWGitshCommonArgs(IStaticAbstraction diskManager, IDWGitshConfig config, IRepositoryPaths repoPaths) : this(diskManager, config, repoPaths, null, null) { }
        public DWGitshCommonArgs(IStaticAbstraction diskManager, IDWGitshConfig config, IRepositoryPaths repoPaths, IGitUtils utils) : this(diskManager, config, repoPaths, utils, null) { }

        public DWGitshCommonArgs(IStaticAbstraction diskManager, IDWGitshConfig config, IRepositoryPaths repoPaths, IGitUtils utils, ICacheContainer cache)
        {
            this.DiskManager = diskManager ?? new StAbWrapper();

            this.Config = config ?? new DWGitshConfig(DiskManager);

            this.Cache = cache ?? new CacheContainer(DiskManager);

            this.Utils = utils ?? new GitUtils(DiskManager, Cache);

            this.RepoPaths = repoPaths;
        }
    }
}
