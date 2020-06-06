using DWGitsh.Extensions.Commands.Git.ChangeDirectory;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions;
using NUnit.Framework;
using NSubstitute;
using System.Linq;

namespace DWGitsh.Extensions.Tests.Commands.Git.ChangeDirectory.Actions
{
    [TestFixture]
    public class ActionRemoveAliasTests : ChangeDirectoryActionsTestBase
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
        public void RemoveAlias_invalid_name_no_action_taken(string value)
        {
            var options = new GetGitChangeDirectoryCommandOptions { NameOrAlias = value };
            var testCmd = new ActionRemoveAlias(_repoPaths, options, _hitManager);

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
            var testCmd = new ActionRemoveAlias(_repoPaths, options, _hitManager);

            Assert.AreEqual(options.NameOrAlias, testCmd.TargetName);

            testCmd.Process(_info);

            _hitManager.Received(0).GetHitList();
        }


        [Test]
        public void Existing_entry_remove_no_match()
        {
            var hitData = GcdTestHelper.BuildFakeHitData(false);
            var expectedData = hitData.First();
            _hitManager.GetHitList().Returns(hitData);

            var options = new GetGitChangeDirectoryCommandOptions { NameOrAlias = "Gibberish", RemoveAlias = true};

            var testCmd = new ActionRemoveAlias(_repoPaths, options, _hitManager);


            testCmd.Process(_info);

            _hitManager.Received(1).GetHitList();
            _hitManager.Received(0).SetAlias(Arg.Any<string>(), Arg.Any<string>());
        }


        [Test]
        public void Existing_entry_remove_alias()
        {
            var hitData = GcdTestHelper.BuildFakeHitData(true);
            _hitManager.GetHitList().Returns(hitData);
            var expectedData = hitData.First();

            var findName = GcdTestHelper.GetFolderNameFromPath(expectedData.Directory);

            var options = new GetGitChangeDirectoryCommandOptions { NameOrAlias = findName, RemoveAlias = true };

            var testCmd = new ActionRemoveAlias(_repoPaths, options, _hitManager);
            Assert.AreEqual(options.NameOrAlias, testCmd.TargetName);

            testCmd.Process(_info);

            _hitManager.Received(1).GetHitList();
            _hitManager.Received(1).SetAlias(expectedData.Directory, null);
        }

    }
}
