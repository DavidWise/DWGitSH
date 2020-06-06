using DWGitsh.Extensions.Commands.Git.ChangeDirectory;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions;
using DWGitsh.Extensions.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using NSubstitute;
using System.Linq;

namespace DWGitsh.Extensions.Tests.Commands.Git.ChangeDirectory.Actions
{
    [TestFixture]
    public class ActionSetAliasTests : ChangeDirectoryActionsTestBase
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
            var testCmd = new ActionSetAlias(_repoPaths, options, _hitManager);

            Assert.AreEqual(options.NameOrAlias, testCmd.TargetName);

            testCmd.Process(_info);

            _hitManager.Received(0).GetHitList();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("\t")]
        [TestCase(" \t ")]
        public void SetAlias_invalid_alias_no_action_taken(string value)
        {
            var options = new GetGitChangeDirectoryCommandOptions { NameOrAlias = "ShouldntMatter", Alias = value };
            var testCmd = new ActionSetAlias(_repoPaths, options, _hitManager);

            Assert.AreEqual(options.NameOrAlias, testCmd.TargetName);

            testCmd.Process(_info);

            _hitManager.Received(0).GetHitList();
        }


        [Test]
        public void Existing_entry_set_new_alias()
        {
            var hitData = GcdTestHelper.BuildFakeHitData(true);
            var expectedData = hitData.First();

            var options = new GetGitChangeDirectoryCommandOptions { NameOrAlias = expectedData.Alias, Alias = "Surge" };

            var testCmd = new ActionSetAlias(_repoPaths, options, _hitManager);
            Assert.AreEqual(options.NameOrAlias, testCmd.TargetName);
            _hitManager.GetHitList().Returns(hitData);

            testCmd.Process(_info);

            _hitManager.Received(1).GetHitList();
            _hitManager.Received(1).SetAlias(expectedData.Directory, options.Alias);
        }


        [Test]
        public void Existing_entry_no_alias_set_new_alias()
        {
            var hitData = GcdTestHelper.BuildFakeHitData(false);
            var expectedData = hitData.First();

            var findName = GcdTestHelper.GetFolderNameFromPath(expectedData.Directory);

            var options = new GetGitChangeDirectoryCommandOptions { NameOrAlias = findName, Alias = "Surge" };

            var testCmd = new ActionSetAlias(_repoPaths, options, _hitManager);
            Assert.AreEqual(options.NameOrAlias, testCmd.TargetName);
            _hitManager.GetHitList().Returns(hitData);

            testCmd.Process(_info);

            _hitManager.Received(1).GetHitList();
            _hitManager.Received(1).SetAlias(expectedData.Directory, options.Alias);
        }

    }
}
