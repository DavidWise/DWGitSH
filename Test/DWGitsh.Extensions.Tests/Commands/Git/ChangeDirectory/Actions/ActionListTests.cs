﻿using DWGitsh.Extensions.Commands.Git.ChangeDirectory;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions;
using NUnit.Framework;
using NSubstitute;

namespace DWGitsh.Extensions.Tests.Commands.Git.ChangeDirectory.Actions
{
    public class ActionListTests : ChangeDirectoryActionsTestBase
    {
        [SetUp]
        public void Setup()
        {
            base.SetupBase();
        }


        [Test]
        public void Valid_returns_all_dirs()
        {
            var options = new GetGitChangeDirectoryCommandOptions { List = true };
            var testCmd = new ActionList(_repoPaths, options, _hitManager);

            var result = testCmd.Process(_info);
            Assert.True(result);

            _hitManager.Received(1).GetHitList();
            Assert.NotNull(_info.ListData);
            Assert.True(GcdTestHelper.AreSame(_AllHitData, _info.ListData));

        }

        [Test]
        public void Not_set_does_nothing()
        {
            var options = new GetGitChangeDirectoryCommandOptions {  };
            var testCmd = new ActionList(_repoPaths, options, _hitManager);

            var result = testCmd.Process(_info);
            Assert.False(result);

            _hitManager.Received(0).GetHitList();
        }

    }
}
