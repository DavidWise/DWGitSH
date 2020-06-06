using DWGitsh.Extensions.Commands.Git.ChangeDirectory;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Actions;
using NUnit.Framework;
using NSubstitute;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using System.Collections.Generic;

namespace DWGitsh.Extensions.Tests.Commands.Git.ChangeDirectory.Actions
{
    public class ActionDefaultTests : ChangeDirectoryActionsTestBase
    {
        [SetUp]
        public void Setup()
        {
            base.SetupBase();
        }


        private void Anything_selected_does_not_trigger_action_base_test(GetGitChangeDirectoryCommandOptions options)
        {
            var testCmd = new ActionDefault(_repoPaths, options, _hitManager);

            var result = testCmd.Process(_info);

            Assert.False(result);
            _hitManager.Received(0).GetHitList();
        }


        [Test]
        public void Anything_selected_does_not_trigger_action_Log()
        {
            var options = new GetGitChangeDirectoryCommandOptions { Log = true };
            Anything_selected_does_not_trigger_action_base_test(options);
        }


        [Test]
        public void Anything_selected_does_not_trigger_action_Last()
        {
            var options = new GetGitChangeDirectoryCommandOptions { Last = true };
            Anything_selected_does_not_trigger_action_base_test(options);
        }


        [Test]
        public void Anything_selected_does_not_trigger_action_LogOnly()
        {
            var options = new GetGitChangeDirectoryCommandOptions { LogOnly= true };
            Anything_selected_does_not_trigger_action_base_test(options);
        }

        [Test]
        public void Anything_selected_does_not_trigger_action_List()
        {
            var options = new GetGitChangeDirectoryCommandOptions { List = true };
            Anything_selected_does_not_trigger_action_base_test(options);
        }
        [Test]
        public void Anything_selected_does_not_trigger_action_Name()
        {
            var options = new GetGitChangeDirectoryCommandOptions { NameOrAlias = "Plantagenet" };
            Anything_selected_does_not_trigger_action_base_test(options);
        }
        [Test]
        public void Anything_selected_does_not_trigger_action_Alias()
        {
            var options = new GetGitChangeDirectoryCommandOptions { Alias = "Otoole" };
            Anything_selected_does_not_trigger_action_base_test(options);
        }

        [Test]
        public void Not_set_returns_list_and_prompt()
        {
            _hitManager.GetHitList().Returns(new List<HitData>());
            var options = new GetGitChangeDirectoryCommandOptions {  };
            var testCmd = new ActionDefault(_repoPaths, options, _hitManager);

            var result = testCmd.Process(_info);
            Assert.True(result);

            _hitManager.Received(1).GetHitList();
            Assert.True(_info.PromptForListSelector);
            Assert.True(_info.Options.List);
            CollectionAssert.IsEmpty(_info.ListData);
        }
    }
}
