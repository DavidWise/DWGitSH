using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NSubstitute;
using StaticAbstraction;
using DWGitsh.Extensions.Models;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using StaticAbstraction.IO.Mocks;

namespace DWGitsh.Extensions.Tests.Commands.Git.ChangeDirectory.Data
{
    [TestFixture]
    public class HitDataManagerTests
    {
        private IStaticAbstraction _diskManager;
        private IRepositoryPaths _repoPaths;

        private HitDataManager _manager;
        private string _localDataFolder = "C:\\Local\\Data\\Folder\\";
        private string _localAppDataFolder = "C:\\Local\\Data\\Folder\\DWGitsh";
        private string _localAppDataHitsFile = "C:\\Local\\Data\\Folder\\DWGitsh\\hitData.json";

        [SetUp]
        public void Setup()
        {
            _diskManager = Substitute.For<IStaticAbstraction>();
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
    }
}
