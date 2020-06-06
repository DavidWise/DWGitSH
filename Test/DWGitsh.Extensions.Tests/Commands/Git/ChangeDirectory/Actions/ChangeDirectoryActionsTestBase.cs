using DWGitsh.Extensions.Commands.Git.ChangeDirectory;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Models;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace DWGitsh.Extensions.Tests.Commands.Git.ChangeDirectory.Actions
{
    public class ChangeDirectoryActionsTestBase
    {
        protected static string _nonGitFolder = "D:\\Some\\Nongit\\Folder";
        protected static string _rootFolder = "D:\\Junk\\Folder\\Root";
        protected static string _gitFolder = "D:\\Junk\\Folder\\Root\\.git";

        protected IRepositoryPaths _repoPaths;
        protected IRepositoryPaths _repoPathsNoGit;
        protected IHitDataManager _hitManager;
        protected IHitDataManager _hitManagerEmpty;
        protected GitChangeDirectoryInfo _info;
        protected GetGitChangeDirectoryCommandOptions _options;

        protected List<HitData> _AllHitData;
        protected HitData _lastHitFolder;



        protected void SetupBase()
        {
            _AllHitData = GcdTestHelper.BuildFakeHitData();

            _info = new GitChangeDirectoryInfo();
            _options = new GetGitChangeDirectoryCommandOptions();
            _info.Options = new GetGitChangeDirectoryCommandOptions();

            _repoPaths = Substitute.For<IRepositoryPaths>();
            _repoPaths.RepositoryFolder.Returns(_gitFolder);
            _repoPaths.RootFolder.Returns(_rootFolder);

            _repoPathsNoGit = Substitute.For<IRepositoryPaths>();
            _repoPathsNoGit.CurrentPath.Returns(_nonGitFolder);

            _hitManager = Substitute.For<IHitDataManager>();
            _hitManager.GetHitList().Returns(_AllHitData);
            _lastHitFolder = _AllHitData.OrderByDescending(x => x.DateLastHit).First();
            _hitManager.GetLastUsedFolder().Returns(_lastHitFolder.Directory);

            _hitManagerEmpty = Substitute.For<IHitDataManager>();
            _hitManagerEmpty.GetHitList().Returns(new List<HitData>());
            _hitManagerEmpty.GetLastUsedFolder().Returns((string)null);
        }

    }
}
