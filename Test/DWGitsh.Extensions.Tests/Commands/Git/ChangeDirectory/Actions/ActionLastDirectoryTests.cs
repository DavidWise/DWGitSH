﻿using DWGitsh.Extensions.Commands.Git.ChangeDirectory;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions;
using NUnit.Framework;
using NSubstitute;

namespace DWGitsh.Extensions.Tests.Commands.Git.ChangeDirectory.Actions
{
    public class ActionLastDirectoryTests : ChangeDirectoryActionsTestBase
    {

        [SetUp]
        public void Setup()
        {
            base.SetupBase();
        }


        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("\t")]
        [TestCase(" \t ")]
        public void No_directory_hit(string value)
        {
            var options = new GetGitChangeDirectoryCommandOptions { NameOrAlias = value, Last = true };
            var testCmd = new ActionLastDirectory(_repoPaths, options, _hitManager);

            testCmd.Process(_info);

            _hitManager.Received(0).GetHitList();
        }

        [Test]
        public void Valid_returns_last_dir()
        {
            var options = new GetGitChangeDirectoryCommandOptions { Last = true };
            var testCmd = new ActionLastDirectory(_repoPaths, options, _hitManager);

            testCmd.Process(_info);

            _hitManager.Received(0).GetHitList();
            _hitManager.Received(1).GetLastUsedFolder();
            Assert.AreEqual(_lastHitFolder.Directory, _info.TargetDirectory);
        }

    }
}
