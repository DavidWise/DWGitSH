using DWGitsh.Extensions.Commands.Git.ChangeDirectory;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions;
using DWGitsh.Extensions.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using NSubstitute;
using System.Linq;
using DWGitsh.Extensions.Utility;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;

namespace DWGitsh.Extensions.Tests.Commands.Git.ChangeDirectory.Actions
{
    public class ActionNameOrAliasTests : ChangeDirectoryActionsTestBase
    {
        [SetUp]
        public void Setup()
        {
            base.SetupBase();
        }


        [TestCase("", true)]
        [TestCase("", false)]
        [TestCase(null, true)]
        [TestCase(null, false)]
        public void Path_HandleNoTarget(string value, bool useEmptyData)
        {
            var expected = _AllHitData;
            var hitManager = _hitManager;
            if (useEmptyData)
            {
                expected = new List<HitData>();
                hitManager = _hitManagerEmpty;
            }

            var options = new GetGitChangeDirectoryCommandOptions { NameOrAlias = value };
            var testCmd = new ActionNameOrAlias(_repoPaths, options, hitManager);
            _info.Options = new GetGitChangeDirectoryCommandOptions { NameOrAlias = options.NameOrAlias };

            var origList = options.List;
            var result = testCmd.Process(_info);
            Assert.True(result);

            hitManager.Received(1).GetHitList();

            Assert.True(_info.PromptForListSelector == !origList);
            Assert.True(_info.Options.List);
            Assert.True(GcdTestHelper.AreSame(expected, _info.ListData));
        }



        [TestCase("/")]
        [TestCase("\\")]
        public void Path_HandleRootTarget(string value)
        {
            var options = new GetGitChangeDirectoryCommandOptions { NameOrAlias = value };
            var testCmd = new ActionNameOrAlias(_repoPaths, options, _hitManager);

            var result = testCmd.Process(_info);
            Assert.True(result);

            _hitManager.Received(1).GetHitList();

            Assert.NotNull(_info.TargetDirectory);
            Assert.AreEqual(_repoPaths.RootFolder, _info.TargetDirectory);
        }

        [TestCase("/")]
        [TestCase("\\")]
        public void Path_HandleRootTarget_no_data(string value)
        {
            var options = new GetGitChangeDirectoryCommandOptions { NameOrAlias = value };
            var testCmd = new ActionNameOrAlias(_repoPaths, options, _hitManagerEmpty);

            var result = testCmd.Process(_info);
            Assert.True(result);

            _hitManagerEmpty.Received(1).GetHitList();

            Assert.NotNull(_info.TargetDirectory);
            Assert.AreEqual(_repoPaths.RootFolder, _info.TargetDirectory);
        }

        [Test]
        public void Path_HandlePreviousTarget()
        {
            var options = new GetGitChangeDirectoryCommandOptions { NameOrAlias = "-" };
            var testCmd = new ActionNameOrAlias(_repoPaths, options, _hitManager);

            var expected = _AllHitData.OrderByDescending(x => x.DateLastHit)
                .FirstOrDefault(x => x.Directory.IsSameFolder(_repoPaths.RootFolder) == false);

            var result = testCmd.Process(_info);
            Assert.True(result);

            _hitManager.Received(1).GetHitList();

            Assert.NotNull(_info.TargetDirectory);
            Assert.AreEqual(expected.Directory, _info.TargetDirectory);
        }

        [Test]
        public void Path_HandlePreviousTarget_no_data()
        {
            var options = new GetGitChangeDirectoryCommandOptions { NameOrAlias = "-" };
            var testCmd = new ActionNameOrAlias(_repoPaths, options, _hitManagerEmpty);

            var result = testCmd.Process(_info);
            Assert.True(result);

            _hitManagerEmpty.Received(1).GetHitList();

            Assert.Null(_info.TargetDirectory);
        }

        [Test]
        public void Path_HandleSingleTarget()
        {
            var expected = _AllHitData.First();
            var expectedFolderName = GcdTestHelper.GetFolderNameFromPath(expected.Directory);
            var options = new GetGitChangeDirectoryCommandOptions { NameOrAlias = expectedFolderName };
            var testCmd = new ActionNameOrAlias(_repoPaths, options, _hitManager);

            var result = testCmd.Process(_info);
            Assert.True(result);

            _hitManager.Received(1).GetHitList();
            Assert.AreEqual(expected.Directory, _info.TargetDirectory);
        }


        [Test]
        public void Path_HandleMultiTarget()
        {
            var options = new GetGitChangeDirectoryCommandOptions { NameOrAlias = "SomeRandomName" };
            var testCmd = new ActionNameOrAlias(_repoPaths, options, _hitManager);

            var result = testCmd.Process(_info);
            Assert.True(result);

            _hitManager.Received(1).GetHitList();
            Assert.True(GcdTestHelper.AreSame(_AllHitData, _info.ListData));
            Assert.Null(_info.TargetDirectory);
            Assert.True(_info.Options.List);
            Assert.True(_info.PromptForListSelector);
        }

        [Test]
        public void Path_HandleMultiTarget_no_data()
        {
            var options = new GetGitChangeDirectoryCommandOptions { NameOrAlias = "SomeRandomName" };
            var testCmd = new ActionNameOrAlias(_repoPaths, options, _hitManagerEmpty);

            var result = testCmd.Process(_info);
            Assert.True(result);

            _hitManagerEmpty.Received(1).GetHitList();
            CollectionAssert.IsEmpty(_info.ListData);
            Assert.Null(_info.TargetDirectory);
            Assert.True(_info.Options.List);
            Assert.True(_info.PromptForListSelector);
        }

    }
}
