using DWGitsh.Extensions.Commands.Git.ChangeDirectory;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Config;
using DWGitsh.Extensions.Models;
using DWGitsh.Extensions.Tests.Helpers;
using DWGitsh.Extensions.Utility;
using NSubstitute;
using NUnit.Framework;
using StaticAbstraction;
using System.Collections.Generic;

namespace DWGitsh.Extensions.Tests.Commands.Git
{
    [TestFixture]
    public class GetGitChangeDirectoryCommandTests
    {
        protected IHitDataManager _hitManager;
        protected GetGitChangeDirectoryCommand _command;
        protected IDWGitshCommonArgs _args;
        protected IDWGitshConfig _config;
        protected IGitChangeDirectoryOptions _options;
        protected IStaticAbstraction _diskManager;
        protected IRepositoryPaths _repoPaths;

        [SetUp]
        public void Setup()
        {
            _hitManager = Substitute.For<IHitDataManager>();
            _options = Substitute.For<IGitChangeDirectoryOptions>();
            _diskManager = Substitute.For<IStaticAbstraction>();
            _repoPaths = Substitute.For<IRepositoryPaths>();
            _config = Substitute.For<IDWGitshConfig>();

            _args = Substitute.For<IDWGitshCommonArgs>();

            _args.DiskManager.Returns(_diskManager);
            _args.RepoPaths.Returns(_repoPaths);
            _args.Config.Returns(_config);
        }

        private void CreateCommand()
        {
            _command = new GetGitChangeDirectoryCommand(_args, _options, _hitManager);
        }

        [Test]
        public void no_args_does_default()
        {
            _hitManager.GetHitList().Returns(new List<HitData>());
            CreateCommand();

            var result = _command.Process();

            Assert.NotNull(result);
            _hitManager.Received(1).GetHitList();
            Assert.True(result.PromptForListSelector);
            Assert.True(result.Options.List);
            Assert.NotNull(result.ListData);
        }

        [Test]
        public void Valid_arg_log_only()
        {
            var paths = RepositoryPathsHelpers.WithRepo(null, "Web\\Application");
            _args.RepoPaths.Returns(paths);
            _options.LogOnly.Returns(true);

            CreateCommand();

            var result = _command.Process();

            Assert.NotNull(result);
            _hitManager.Received(1).LogCurrentDirectory();
            _hitManager.Received(0).GetHitList();
        }

        [Test]
        public void Valid_arg_with_alias()
        {
            var paths = RepositoryPathsHelpers.WithRepo(null, "Web\\Application");
            _args.RepoPaths.Returns(paths);
            _options.NameOrAlias.Returns("Avacado");
            _hitManager.GetHitList().Returns(new List<HitData>());

            CreateCommand();

            var result = _command.Process();
            Assert.NotNull(result);
            _hitManager.Received(1).GetHitList();
            _hitManager.Received(0).LogCurrentDirectory();

            CollectionAssert.IsEmpty(result.ListData);
            Assert.True(result.Options.List);
            Assert.True(result.PromptForListSelector);
        }
    }
}
