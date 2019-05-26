using DWGitsh.Extensions.Cmdlets;
using DWGitsh.Extensions.Commands.PowerShell;
using DWGitsh.Extensions.Utility.ConsoleIO;
using NUnit.Framework;
using NSubstitute;
using StaticAbstraction;
using System.Management.Automation;
using System.Linq;
using DWPowerShell.Utility.ConsoleIO;

namespace DWGitsh.Extensions.Tests.Commands
{
    [TestFixture()]
    public class WriteTextCommandTests
    {
        protected IConsole _console = null;
        protected IConsoleWriter _writer = null;
        protected IWriteTextCmdlet _cmdlet = null;

        protected SwitchParameter _noNewLine;
        protected SwitchParameter _noColorReset;
        protected SwitchParameter _forceColorReset;

        [SetUp]
        public void SetupTests()
        {
            _console = Substitute.For<IConsole>();
            _writer = Substitute.For<IConsoleWriter>();

            _noNewLine = new SwitchParameter(false);
            _noColorReset = new SwitchParameter(false);
            _forceColorReset = new SwitchParameter(false);
           

            _cmdlet = Substitute.For<IWriteTextCmdlet>();
            _cmdlet.NoNewLine.Returns(_noNewLine);
            _cmdlet.NoColorReset.Returns(_noColorReset);
            _cmdlet.ForceColorReset.Returns(_forceColorReset);
        }

        [Test]
        public void GetTextSimple()
        {
            var msg = "Hello There";
            _cmdlet.ColorGroup = ",filler,";
            _cmdlet.Text = msg;

            var writer = new ConsoleWriter(_console);
            var cmd = new WriteTextCommand(writer);
            cmd.Execute(_cmdlet);
            _console.Received(1).WriteLine(msg);
        }

        [Test]
        public void GetTextSimpleArray()
        {
            var msg = "Hello There";
            _cmdlet.ColorGroup = ",filler,";
            _cmdlet.TextBlocks = new string[] {msg};

            var writer = new ConsoleWriter(_console);
            var cmd = new WriteTextCommand(writer);
            cmd.Execute(_cmdlet);
            _console.Received(1).WriteLine(msg);
        }

        [Test]
        public void GetTextComplexArray()
        {
            var msg = "Hello There";
            var msg2 = "YouTest";
            _cmdlet.ColorGroup = ",filler,";
            _cmdlet.TextBlocks = new string[] { msg, msg2 };

            var writer = new ConsoleWriter(_console);
            var cmd = new WriteTextCommand(writer);

            var vals = cmd.GetTextValues(_cmdlet);
            Assert.IsNotNull(vals);
            Assert.AreEqual(vals.Length, 2);
            Assert.AreEqual(vals[0], msg);
            Assert.AreEqual(vals[1], msg2);

            cmd.Execute(_cmdlet);
            _console.Received(1).Write(msg);
            _console.Received(1).WriteLine(msg2);
        }

        [Test]
        public void GetTextWithSplit()
        {
            var msg = "Hello|There||All";
            _cmdlet.Text = msg;
            _cmdlet.TextSplit = "|";

            var writer = new ConsoleWriter(_console);

            var cmd = new WriteTextCommand(writer);
            var vals = cmd.GetTextValues(_cmdlet);
            Assert.IsNotNull(vals);
            Assert.AreEqual(vals.Length, 4);
            Assert.AreEqual(vals[0], "Hello");
            Assert.AreEqual(vals[1], "There");
            Assert.AreEqual(vals[2], "");
            Assert.AreEqual(vals[3], "All");

            cmd.Execute(_cmdlet);
            _console.Received(3).Write(Arg.Any<string>());
            _console.Received(1).WriteLine("All");
        }

        [Test]
        public void GetTextWithColors()
        {
            var line1 = "Line One";
            var line2 = "Line Too";
            var line3 = "Line Tree";
            _cmdlet.TextBlocks = new string[] {line1, line2, line3 };
            _cmdlet.ColorGroups = new string[] { "branch", "userId", "fullPath" };

            var cmd = new WriteTextCommand(_writer);
            var vals = cmd.GetColorGroups(_cmdlet);
            Assert.IsNotNull(vals);
            Assert.AreEqual(vals.Length, 3);
            Assert.AreEqual(vals, _cmdlet.ColorGroups);

            var branchColor = cmd.Colors.GetColor("branch");
            var userIdColor = cmd.Colors.GetColor("userId");
            var fullPathColor = cmd.Colors.GetColor("fullPath");

            cmd.Execute(_cmdlet);
            _writer.Received(1).Write(line1, branchColor);
            _writer.Received(1).Write(line2, userIdColor);
            _writer.Received(1).WriteLine(line3, fullPathColor);
        }

        [Test]
        public void GetTextWithColorsViaSplit()
        {
            var line1 = "Line One";
            var line2 = "Line Too";
            var line3 = "Line Tree";
            _cmdlet.Text = $"{line1}|{line2}|{line3}";
            _cmdlet.ColorGroup = "branch,userId,fullPath";
            _cmdlet.TextSplit = "|";

            var cmd = new WriteTextCommand(_writer);
            var vals = cmd.GetColorGroups(_cmdlet);
            Assert.IsNotNull(vals);
            Assert.AreEqual(vals.Length, 3);

            var branchColor = cmd.Colors.GetColor("branch");
            var userIdColor = cmd.Colors.GetColor("userId");
            var fullPathColor = cmd.Colors.GetColor("fullPath");

            cmd.Execute(_cmdlet);
            _writer.Received(1).Write(line1, branchColor);
            _writer.Received(1).Write(line2, userIdColor);
            _writer.Received(1).WriteLine(line3, fullPathColor);
        }
    }
}
