using DWGitsh.Extensions.Commands.Git.ChangeDirectory;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Models;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;

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
        protected GitChangeDirectoryInfo _info;
        protected GetGitChangeDirectoryCommandOptions _options;



        protected void SetupBase()
        {
            _info = new GitChangeDirectoryInfo();
            _options = new GetGitChangeDirectoryCommandOptions();

            _repoPaths = Substitute.For<IRepositoryPaths>();
            _repoPaths.RepositoryFolder.Returns(_gitFolder);
            _repoPaths.RootFolder.Returns(_rootFolder);

            _repoPathsNoGit = Substitute.For<IRepositoryPaths>();
            _repoPathsNoGit.CurrentPath.Returns(_nonGitFolder);

            _hitManager = Substitute.For<IHitDataManager>();
        }

    }
}
