using DWGitsh.Extensions.Commands.Git.ChangeDirectory;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions;
using DWGitsh.Extensions.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using NSubstitute;

namespace DWGitsh.Extensions.Tests.Commands.Git.ChangeDirectory.Actions
{
    class ActionSetAliasTests : ChangeDirectoryActionsTestBase
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
        public void SetAlias_invalid_name_no_action_taken(string value)
        {
            var options = new GetGitChangeDirectoryCommandOptions { NameOrAlias = value };
            var testCmd = new ActionSetAlias(options, _hitManager);

            Assert.AreEqual(options.NameOrAlias, testCmd.TargetName);

            testCmd.Process(_info);

            _hitManager.Received(0).GetHitList();
        }


        [Test]
        public void FFDFDF()
        {
            var options = new GetGitChangeDirectoryCommandOptions { NameOrAlias = "Fidget" };
            var testCmd = new ActionSetAlias(options, _hitManager);

            Assert.AreEqual(options.NameOrAlias, testCmd.TargetName);

            //TODO: LoH


        }


    }
}
