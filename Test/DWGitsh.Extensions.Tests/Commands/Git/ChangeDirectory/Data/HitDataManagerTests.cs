using System;
using NUnit.Framework;
using NSubstitute;
using StaticAbstraction;
using DWGitsh.Extensions.Models;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using StaticAbstraction.IO.Mocks;
using StaticAbstraction.IO;
using System.Linq;
using DWGitsh.Extensions.Config;

namespace DWGitsh.Extensions.Tests.Commands.Git.ChangeDirectory.Data
{
    [TestFixture]
    public class HitDataManagerTests
    {
        private IStaticAbstraction _diskManager;
        private IFile _fileManager;
        private IRepositoryPaths _repoPaths;
        private IHitDataRepo _hitRepo;
        private IDWGitshConfig _config;
        private HitDataManager _manager;
        private string _localDataFolder = "C:\\Local\\Data\\Folder\\";
        private string _localAppDataFolder;

        [SetUp]
        public void Setup()
        {
            _localAppDataFolder = $"{_localDataFolder}DWGitsh";

            _fileManager = Substitute.For<IFile>();
            _diskManager = Substitute.For<IStaticAbstraction>();
            _diskManager.File = _fileManager;
            _diskManager.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Returns(_localDataFolder);
            _diskManager.Path.Combine(_localDataFolder, "DWGitsh").Returns(_localAppDataFolder);
            _diskManager.Directory.Exists(_localAppDataFolder).Returns(true);

            _diskManager.NewDirectoryInfo(_localAppDataFolder).Returns(new MockDirectoryInfo { FullName = _localAppDataFolder, Name = "DWGitsh" });

            _config = Substitute.For<IDWGitshConfig>();
            _config.AppDataFolder.Returns(_localAppDataFolder);

            _repoPaths = Substitute.For<IRepositoryPaths>();
            _repoPaths.RootFolder.Returns("C:\\Junk\\Folder\\");
            _repoPaths.RepositoryFolder.Returns("C:\\Junk\\Folder\\.git\\");

            _hitRepo = Substitute.For<IHitDataRepo>();
            _manager = new HitDataManager(_config, _diskManager, _repoPaths, null, _hitRepo);

            _hitRepo.Load().Returns(new CommandData());
        }


        [Test]
        public void LogCurrentDirectory_existingDir_incrementsHitCount()
        {
            var expectedName = "Hello";
            var expectedFolder = "C:\\Junk\\Folder\\";

            var expected = HitDataTestHelper.BuildHelperData(expectedName, expectedFolder);

            _hitRepo.Load().Returns(expected);
            _repoPaths.RepositoryFolder.Returns(expectedFolder);

            _manager.LogCurrentDirectory();

            _hitRepo.Received(1).Save(Arg.Is<CommandData>(s => s.Repositories[0].HitCount == 2));
        }


        [Test]
        public void LogCurrentDirectory_newDir_nopreviousdata()
        {
            var expectedFolder = "C:\\Junk\\Folder\\";

            _repoPaths.RootFolder.Returns(expectedFolder);
            _repoPaths.RepositoryFolder.Returns(expectedFolder);

            _manager.LogCurrentDirectory();

            _hitRepo.Received(1).Save(Arg.Is<CommandData>(s =>
                LogCurrentDirectory_newDir_nopreviousdata_validator(s, expectedFolder)
            ));
        }

        private bool LogCurrentDirectory_newDir_nopreviousdata_validator(CommandData result, string expectedFolder)
        {
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Repositories);
            Assert.AreEqual(1, result.Repositories.Count);
            Assert.AreEqual(expectedFolder, result.Repositories[0].Directory);
            Assert.AreEqual(1, result.Repositories[0].HitCount);

            return true;
        }

        [Test]
        public void GetLastUsedFolder_noData()
        {
            var result = _manager.GetLastUsedFolder();

            Assert.IsNull(result);
        }

        [Test]
        public void GetLastUsedFolder_withData()
        {
            var data = HitDataTestHelper.CommandData_MultipleValues;

            var expected = data.Repositories.OrderByDescending(x => x.DateLastHit).First();

            _hitRepo.Load().Returns(data);

            var result = _manager.GetLastUsedFolder();

            Assert.AreEqual(expected.Directory, result);
        }
    }
}
