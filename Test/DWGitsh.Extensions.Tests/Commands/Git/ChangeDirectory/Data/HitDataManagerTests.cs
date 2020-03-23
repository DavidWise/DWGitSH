using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NSubstitute;
using StaticAbstraction;
using DWGitsh.Extensions.Models;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using StaticAbstraction.IO.Mocks;
using StaticAbstraction.IO;
using Newtonsoft.Json;
using System.Linq;

namespace DWGitsh.Extensions.Tests.Commands.Git.ChangeDirectory.Data
{
    [TestFixture]
    public class HitDataManagerTests
    {
        private IStaticAbstraction _diskManager;
        private IFile _fileManager;
        private IRepositoryPaths _repoPaths;

        private HitDataManager _manager;
        private string _localDataFolder = "C:\\Local\\Data\\Folder\\";
        private string _localAppDataFolder = "C:\\Local\\Data\\Folder\\DWGitsh";
        private string _localAppDataHitsFile = "C:\\Local\\Data\\Folder\\DWGitsh\\hitData.json";

        [SetUp]
        public void Setup()
        {
            _fileManager = Substitute.For<IFile>();
            _diskManager = Substitute.For<IStaticAbstraction>();
            _diskManager.File = _fileManager;
            _diskManager.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Returns(_localDataFolder);
            _diskManager.Path.Combine(_localDataFolder, "DWGitsh").Returns(_localAppDataFolder);
            _diskManager.Path.Combine(_localAppDataFolder, "hitData.json").Returns(_localAppDataHitsFile);
            _diskManager.Directory.Exists(_localAppDataFolder).Returns(true);

            _diskManager.NewDirectoryInfo(_localAppDataFolder).Returns(new MockDirectoryInfo { FullName = _localAppDataFolder, Name = "DWGitsh" });

            _repoPaths = Substitute.For<IRepositoryPaths>();
            _repoPaths.RootFolder.Returns("C:\\Junk\\Folder\\");
            _repoPaths.RepositoryFolder.Returns("C:\\Junk\\Folder\\.git\\");

            _manager = new HitDataManager(_diskManager, _repoPaths);
        }

        [Test]
        public void DataDirectory_finder()
        {
            Assert.AreEqual(_localAppDataFolder, _manager.DataFolder.FullName);
        }

        [Test]
        public void ReadHitData_no_existing_file()
        {
            var data = _manager.ReadHitData();

            _diskManager.Received(1).File.Exists(_localAppDataHitsFile);
            Assert.NotNull(data);
            Assert.NotNull(data.Repositories);
        }

        [Test]
        public void ReadHitData_existing_file()
        {
            var expectedName = "Hello";
            var expectedFolder = "There";

            var expected = MockUpReadHitData(expectedName, expectedFolder);

            var data = _manager.ReadHitData();

            _fileManager.Received(1).Exists(_localAppDataHitsFile);
            _fileManager.Received(1).ReadAllText(_localAppDataHitsFile);
            Assert.NotNull(data);
            Assert.NotNull(data.Repositories);
            Assert.AreEqual(1, data.Repositories.Count);
            Assert.AreEqual(expectedName, data.Repositories[0].Name);
            Assert.AreEqual(expectedFolder, data.Repositories[0].Directory);
        }


        [Test]
        public void LogCurrentDirectory_existingDir_incrementsHitCount()
        {
            var expectedName = "Hello";
            var expectedFolder = "C:\\Junk\\Folder\\.git\\";

            var expected = MockUpReadHitData(expectedName, expectedFolder);

            _repoPaths.RepositoryFolder.Returns(expectedFolder);

            _manager.LogCurrentDirectory();

            _fileManager.Received(1).WriteAllText(_localAppDataHitsFile, 
                Arg.Is<string>(s => JsonConvert.DeserializeObject<CommandData>(s).Repositories[0].HitCount == 2));

        }


        [Test]
        public void LogCurrentDirectory_newDir_nopreviousdata()
        {
            var expectedName = "Hello";
            var expectedFolder = "C:\\Junk\\Folder\\.git\\";

            _repoPaths.RepositoryFolder.Returns(expectedFolder);

            _manager.LogCurrentDirectory();

            _fileManager.Received(1).WriteAllText(_localAppDataHitsFile,
                Arg.Is<string>(s => LogCurrentDirectory_newDir_nopreviousdata_validator(expectedFolder,s)));
        }

        private bool LogCurrentDirectory_newDir_nopreviousdata_validator(string folderName, string s)
        {
            var passedTest = false;

            var data = JsonConvert.DeserializeObject<CommandData>(s);
            Assert.NotNull(data);
            Assert.NotNull(data.Repositories);
            Assert.AreEqual(1, data.Repositories.Count);
            Assert.AreEqual(folderName, data.Repositories[0].Directory);
            Assert.AreEqual(1, data.Repositories[0].HitCount);
            passedTest = true;

            return passedTest;
        }




        #region helper methods

        private CommandData MockUpReadHitData()
        {
            return MockUpReadHitData("SampleName", _localAppDataFolder, 1);
        }
        private CommandData MockUpReadHitData(string name, string repoDir, int hitCount = 1)
        {
            var data = BuildHelperData(name, repoDir, hitCount);
            return MockUpReadHitData(data);
        }

        private CommandData MockUpReadHitData(CommandData returnData)
        {
            string data = null;
            if (returnData != null)
                data = returnData.ToJson();

            _fileManager.Exists(_localAppDataHitsFile).Returns(data != null);
            _fileManager.ReadAllText(_localAppDataHitsFile).Returns(data);

            return returnData;
        }

        private CommandData BuildHelperData(string name, string repoDir, int hitCount = 1)
        {
            var dateLastHit = DateTime.Now.AddMinutes(-5);
            return new CommandData
            {
                Repositories = new List<HitData> {
                    new HitData { Directory = repoDir, Name = name, HitCount = hitCount, DateLastHit = dateLastHit }
                }
            };
        }


        #endregion
    }

    static class CommandDataTestExtensions
    {
        public static string ToJson(this CommandData data)
        {
            if (data == null) return string.Empty;
            return JsonConvert.SerializeObject(data);
        }
    }
}
