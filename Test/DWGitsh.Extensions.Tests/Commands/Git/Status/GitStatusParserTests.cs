using DWGitsh.Extensions.Commands.Git;
using DWGitsh.Extensions.Commands.Git.Status;
using DWGitsh.Extensions.Models;
using DWGitsh.Extensions.Tests.Helpers;
using DWGitsh.Extensions.Utility.ConsoleIO;
using NSubstitute;
using NUnit.Framework;
using StaticAbstraction;
using System.Linq;

namespace DWGitsh.Extensions.Tests.Commands.Git.Status
{
    [TestFixture()]
    public class GitStatusParserTests
    {
        protected IConsole _console = null;
        protected IConsoleWriter _writer = null;
        protected IGitCommand _cmdlet = null;


        [SetUp]
        public void SetupTests()
        {
            _console = Substitute.For<IConsole>();
            _writer = Substitute.For<IConsoleWriter>();

            var repoDirs = Substitute.For<IRepositoryPaths>();
            repoDirs.RootFolder.Returns("C:\\Junk\\Folder\\");
            repoDirs.RepositoryFolder.Returns("C:\\Junk\\Folder\\.git\\");
            _cmdlet = Substitute.For<IGitCommand>();

            _cmdlet.RepositoryDirectories.Returns(repoDirs);

        }


        [Test]
        public void StatusContainsCopiedFile()
        {
            var data = TestHelper.GetTestData("Status/Res/MultipleFilesWithCopy.txt");

            var parser = new GitStatusParser(_cmdlet);
            var result = parser.Parse(data);
            Assert.IsNotNull(result);

            var copied = result.FileChanges.Where(x => x.State == GitFileState.Copied).ToList();
            Assert.IsNotNull(copied);
            Assert.IsTrue(copied.Count == 1);
            Assert.AreEqual(copied[0].Name, "react-dom.bundle.js");
        }

        [Test]
        public void StatusForDetachedHead()
        {
            var data = TestHelper.GetTestData("Status/Res/GitStatusDetachedHead.txt");

            var parser = new GitStatusParser(_cmdlet);
            var result = parser.Parse(data);
            Assert.IsNotNull(result);

            Assert.IsTrue(result.Detached);
            Assert.AreEqual("HEAD", result.DetachedMarker);
            Assert.AreEqual("1.3680.0", result.DetachedAt);
        }
    }
}
