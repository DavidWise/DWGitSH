using DWGitsh.Extensions.Commands.Git.ChangeDirectory;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions;
using DWGitsh.Extensions.Models;
using NSubstitute;
using NUnit.Framework;

namespace DWGitsh.Extensions.Tests.Commands.Git.ChangeDirectory.Actions
{
    public class ActionLogTests : ChangeDirectoryActionsTestBase
    {
        [SetUp]
        public void Setup()
        {
            base.SetupBase();
        }

        [Test]
        public void IsUnderGitRepo_valid_git()
        {
            _options.Log = true;

            var test = new ActionLog(_repoPaths, _options, _hitManager);

            Assert.IsTrue(test.IsUnderGitRepo);
        }

        [Test]
        public void IsUnderGitRepo_no_git()
        {
            _options.Log = true;

            var test = new ActionLog(_repoPathsNoGit, _options, _hitManager);

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
            _options.Log = true;

            var test = new ActionLog(_repoPathsNoGit, _options, _hitManager);
            test.Process(_info);

            _hitManager.Received(0).LogCurrentDirectory();
            
        }

        [Test]
        public void IsUnderGitRepo_logged()
        {
            _options.Log = true;
            var info = new GitChangeDirectoryInfo();

            var test = new ActionLog(_repoPaths, _options, _hitManager);
            test.Process(info);

            _hitManager.Received(1).LogCurrentDirectory();
            Assert.IsFalse(_options.DoneProcessing);

        }

        [Test]
        public void IsUnderGitRepo__logOnly_logged()
        {
            _options.LogOnly = true;
            var info = new GitChangeDirectoryInfo();

            var test = new ActionLog(_repoPaths, _options, _hitManager);
            test.Process(info);

            _hitManager.Received(1).LogCurrentDirectory();
            Assert.IsTrue(_options.DoneProcessing);

        }
    }
}
