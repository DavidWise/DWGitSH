using DWGitsh.Extensions.Commands.Git.ChangeDirectory;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Models;
using NSubstitute;
using NUnit.Framework;

namespace DWGitsh.Extensions.Tests.Commands.Git.ChangeDirectory.Actions
{
    public class ActionLogTests
    {
        private static string _nonGitFolder = "D:\\Some\\Nongit\\Folder";
        private static string _rootFolder = "D:\\Junk\\Folder\\Root";
        private static string _gitFolder = "D:\\Junk\\Folder\\Root\\.git";

        private IRepositoryPaths _repoPaths;
        private IRepositoryPaths _repoPathsNoGit;
        private IHitDataManager _hitManager;

        [SetUp]
        public void Setup()
        {
            _repoPaths = Substitute.For<IRepositoryPaths>();
            _repoPaths.RepositoryFolder.Returns(_gitFolder);
            _repoPaths.RootFolder.Returns(_rootFolder);

            _repoPathsNoGit = Substitute.For<IRepositoryPaths>();
            _repoPathsNoGit.CurrentPath.Returns(_nonGitFolder);

            _hitManager = Substitute.For<IHitDataManager>();
        }

        [Test]
        public void IsUnderGitRepo_valid_git()
        {
            var options = new GetGitChangeDirectoryCommandOptions { Log = true };

            var test = new ActionLog(_repoPaths, options, _hitManager);
            Assert.IsTrue(test.IsUnderGitRepo);
        }

        [Test]
        public void IsUnderGitRepo_no_git()
        {
            var options = new GetGitChangeDirectoryCommandOptions { Log = true };

            var test = new ActionLog(_repoPathsNoGit, options, _hitManager);
            Assert.IsFalse(test.IsUnderGitRepo);
        }

        [Test]
        public void IsUnderGitRepo_null()
        {
            var options = new GetGitChangeDirectoryCommandOptions { Log = true };

            var test = new ActionLog(null, options, _hitManager);
            Assert.IsFalse(test.IsUnderGitRepo);
        }

        [Test]
        public void IsUnderGitRepo_no_git_not_logged()
        {
            var options = new GetGitChangeDirectoryCommandOptions { Log = true };
            var info = new GitChangeDirectoryInfo();

            var test = new ActionLog(_repoPathsNoGit, options, _hitManager);
            test.Process(info);
            _hitManager.Received(0).LogCurrentDirectory();
            
        }

        [Test]
        public void IsUnderGitRepo_logged()
        {
            var options = new GetGitChangeDirectoryCommandOptions { Log = true };
            var info = new GitChangeDirectoryInfo();

            var test = new ActionLog(_repoPaths, options, _hitManager);
            test.Process(info);
            _hitManager.Received(1).LogCurrentDirectory();
            Assert.IsFalse(options.DoneProcessing);

        }

        [Test]
        public void IsUnderGitRepo__logOnly_logged()
        {
            var options = new GetGitChangeDirectoryCommandOptions { LogOnly = true};
            var info = new GitChangeDirectoryInfo();

            var test = new ActionLog(_repoPaths, options, _hitManager);
            test.Process(info);
            _hitManager.Received(1).LogCurrentDirectory();
            Assert.IsTrue(options.DoneProcessing);

        }
    }
}
