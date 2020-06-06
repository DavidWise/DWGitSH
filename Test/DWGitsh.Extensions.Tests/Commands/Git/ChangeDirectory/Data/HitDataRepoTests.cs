using NUnit.Framework;
using NSubstitute;
using StaticAbstraction;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using StaticAbstraction.IO;

namespace DWGitsh.Extensions.Tests.Commands.Git.ChangeDirectory.Data
{
    public class HitDataRepoTests
    {
        private string _dataPath = "C:\\junkData\\Local\\User\\someFolder";
        private string _localAppDataHitsFile;
        private IStaticAbstraction _diskManager;
        private IFile _fileManager;

        private IHitDataRepo _repo;

        [SetUp]
        public void Setup()
        {
            _localAppDataHitsFile = $"{_dataPath}\\hitData.json";

            _fileManager = Substitute.For<IFile>();
            _diskManager = Substitute.For<IStaticAbstraction>();
            _diskManager.File = _fileManager;

            _diskManager.Path.Combine(_dataPath, "hitData.json").Returns(_localAppDataHitsFile);

            _repo = new HitDataRepo(_dataPath, _diskManager);
        }

        [Test]
        public void ReadHitData_no_existing_file()
        {
            _diskManager.File.Exists(_localAppDataHitsFile).Returns(false);

            var data = _repo.Load();

            Assert.NotNull(data);
            Assert.NotNull(data.Repositories);
            _diskManager.File.Received(1).Exists(_localAppDataHitsFile);
            _diskManager.File.Received(0).ReadAllText(_localAppDataHitsFile);
        }

        [Test]
        public void ReadHitData_existing_file()
        {
            var expectedName = "Hello";
            var expectedFolder = "There";

            var expected = HitDataTestHelper.BuildHelperData(expectedName, expectedFolder);
            MockUpReadHitData(expected);

            var data = _repo.Load();

            Assert.NotNull(data);
            Assert.NotNull(data.Repositories);
            Assert.AreEqual(1, data.Repositories.Count);
            Assert.AreEqual(expectedName, data.Repositories[0].Name);
            Assert.AreEqual(expectedFolder, data.Repositories[0].Directory);
        }


        private CommandData MockUpReadHitData(CommandData returnData)
        {
            string data = null;
            if (returnData != null)
                data = HitDataTestHelper.ConvertToJson(returnData);

            _fileManager.Exists(_localAppDataHitsFile).Returns(data != null);
            _fileManager.ReadAllText(_localAppDataHitsFile).Returns(data);

            return returnData;
        }
    }
}
