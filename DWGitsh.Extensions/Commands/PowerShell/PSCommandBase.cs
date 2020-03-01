using DWGitsh.Extensions.Config;
using DWGitsh.Extensions.Models;
using DWGitsh.Extensions.Utility;
using StaticAbstraction;

namespace DWGitsh.Extensions.Commands.PowerShell
{
    public abstract class PowerShellCommandBase<TResult>
    {
        public IRepositoryPaths RepositoryDirectories { get; protected set; }

        protected IStaticAbstraction _diskManager { get; private set; }
        protected IDWGitshConfig _config { get; private set; }


        internal PowerShellCommandBase(IDWGitshCommonArgs commonArgs)
        {
            this._diskManager = commonArgs.DiskManager;
            this._config = commonArgs.Config;
            this.RepositoryDirectories = commonArgs.RepoPaths;
        }

        public abstract TResult Process();
    }
}
