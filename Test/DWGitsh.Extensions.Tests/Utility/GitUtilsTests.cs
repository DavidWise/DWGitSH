using DWGitsh.Extensions.Models;
using DWGitsh.Extensions.Utility;
using DWPowerShell.Utility;
using DWPowerShell.Utility.Cache;
using NSubstitute;
using NUnit.Framework;
using StaticAbstraction;
using StaticAbstraction.IO;
using System;

namespace DWGitsh.Extensions.Tests.Utility
{
    [TestFixture]
    public class GitUtilsTests
    {
        protected IStaticAbstraction _diskManager;
        protected IPath _pathManager;
        protected ICacheContainer _cacheManager;
        protected GitUtils _gitUtils;

        [SetUp]
        public void Setup()
        {
            _diskManager = Substitute.For<IStaticAbstraction>();
            _pathManager = Substitute.For<IPath>();
            _pathManager.Combine(Arg.Any<string>(), Arg.Any<string>()).Returns(x=>
            {
                var data = x.Arg<string[]>();
                return data[0].TrimEnd('\\') + "\\" + data[1].TrimStart('\\');
            });
            _diskManager.Path.Returns(_pathManager);
            _cacheManager = Substitute.For<ICacheContainer>();
            _gitUtils = new GitUtils(_diskManager, _cacheManager);
        }

        protected IDirectoryInfo BuildDirectoryInfo(string path, bool exists, IDirectoryInfo parent)
        {
            var dirInfo = Substitute.For<IDirectoryInfo>();
            dirInfo.Parent.Returns((IDirectoryInfo)parent);
            dirInfo.Exists.Returns(exists);
            dirInfo.FullName.Returns(path);
            var name = path.Trim('\\');
            var pos = name.LastIndexOf("\\");
            if (pos > 0)
            {
                name = path.Substring(pos+1);
            }
            dirInfo.Name.Returns(name);

            return dirInfo;
        }

        protected IDirectoryInfo BuildDirectoryInfo(string path)
        {
            var segments = path.Split('\\');
            IDirectoryInfo parentPath = null;
            IDirectoryInfo curpath = null;
            var buildPath = "";

            for (int i = 0; i < segments.Length; i++)
            {
                buildPath += segments[i];
                curpath = BuildDirectoryInfo(buildPath, true, parentPath);
                parentPath = curpath;
                buildPath += "\\";
            }

            return curpath;
        }


        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(" \r\t   ")]
        [TestCase("\r\t")]
        public void GetPathToRepository_nullOrWhiteSpace(string path)
        {
            Assert.IsNull(_gitUtils.GetPathToRepository(path));
        }

        [Test]
        public void GetPathToRepository_gitPathUpTree()
        {
            var dirPath = "C:\\junk\\folder\\does\\not\\exist";
            var dirInfo = BuildDirectoryInfo(dirPath);
            var expected = "C:\\junk\\";

            _diskManager.NewDirectoryInfo("somePath").Returns(dirInfo);
            _diskManager.Directory.Exists(expected + ".git").Returns(true);

            var result = _gitUtils.GetPathToRepository("somePath");
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void GetPathToRepository_no_gitPathUpTree()
        {
            var dirPath = "C:\\junk\\folder\\does\\not\\exist";
            var dirInfo = BuildDirectoryInfo(dirPath);

            _diskManager.NewDirectoryInfo("somePath").Returns(dirInfo);

            var result = _gitUtils.GetPathToRepository("somePath");
            Assert.IsNull(result);
        }

        #region GetBranchName Tests

        [Test]
        public void GetBranchName_null()
        {
            Assert.IsNull(_gitUtils.GetBranchName(null));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("  \t ")]
        [TestCase("\r")]
        public void GetBranchName_whitespaceAsPath(string testValue)
        {
            var repoDirs = Substitute.For<IRepositoryPaths>();
            repoDirs.RepositoryFolder.Returns(testValue);
            Assert.IsNull(_gitUtils.GetBranchName(repoDirs));
        }

        [Test]
        public void GetBranchName_bad_flows()
        {
            var repoPath = "C:\\junk\\folder\\.git\\";
            var repoDirs = Substitute.For<IRepositoryPaths>();
            repoDirs.RepositoryFolder.Returns(repoPath);

            _diskManager.File.Exists(Arg.Any<string>()).Returns(false);
            Assert.IsNull(_gitUtils.GetBranchName(repoDirs));

            _diskManager.File.Exists(Arg.Any<string>()).Returns(true);
            _diskManager.File.ReadAllText(Arg.Any<string>()).Returns((string)null);
            Assert.IsNull(_gitUtils.GetBranchName(repoDirs));

            _diskManager.File.ReadAllText(Arg.Any<string>()).Returns("");
            Assert.IsNull(_gitUtils.GetBranchName(repoDirs));

            _diskManager.File.ReadAllText(Arg.Any<string>()).Returns("  ");
            Assert.IsNull(_gitUtils.GetBranchName(repoDirs));

            _diskManager.File.ReadAllText(Arg.Any<string>()).Returns("\r\n");
            Assert.IsNull(_gitUtils.GetBranchName(repoDirs));

            _diskManager.File.ReadAllText(Arg.Any<string>()).Returns("Unexpected Text");
            Assert.IsNull(_gitUtils.GetBranchName(repoDirs));

            _diskManager.File.ReadAllText(Arg.Any<string>()).Returns("Unexpected Text/");
            Assert.AreEqual(string.Empty,_gitUtils.GetBranchName(repoDirs));

            _diskManager.File.ReadAllText(Arg.Any<string>()).Returns("Unexpected/Text/");
            Assert.AreEqual(string.Empty, _gitUtils.GetBranchName(repoDirs));
        }


        [Test]
        public void GetBranchName_valid_path()
        {
            var expected = "branch_name";
            var repoPath = "C:\\junk\\folder\\.git\\";
            var repoDirs = Substitute.For<IRepositoryPaths>();
            repoDirs.RepositoryFolder.Returns(repoPath);
            _diskManager.File.Exists(Arg.Any<string>()).Returns(true);
            _diskManager.File.ReadAllText(Arg.Any<string>()).Returns($"ref: refs/heads/{expected}");

            Assert.AreEqual(expected, _gitUtils.GetBranchName(repoDirs));

            _diskManager.File.ReadAllText(Arg.Any<string>()).Returns($"ref: refs/heads/{expected}   \r\n");
            Assert.AreEqual(expected, _gitUtils.GetBranchName(repoDirs));
        }

        #endregion


        #region  GetRepoPaths Tests

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(" \r\t   ")]
        [TestCase("\r\t")]
        public void GetRepoPaths_badPaths_throw_exception(string testPath)
        {
            Assert.Throws<ArgumentException>(() => { _gitUtils.GetRepoPaths(testPath, true); });
            DWPSUtils._diskManager = _diskManager;
        }

        [Test]
        public void GetRepoPaths_usesCacheIfAvailable()
        {
            var currentPath = "C:\\testZZZ\\badYYY\\folderNNN";
            var cacheFolders = new RepoPaths {CurrentPath = currentPath };
            var cacheKey = "REF" + currentPath + "\\";

            _cacheManager.Get<RepoPaths>(cacheKey).Returns(cacheFolders);

            DWPSUtils._diskManager = _diskManager;

            var result = _gitUtils.GetRepoPaths(currentPath, false);
            _cacheManager.Received(1).Get<RepoPaths>(cacheKey);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.CurrentPath, currentPath);
            Assert.IsFalse(result.IgnoreCache);
        }

        #endregion

        #region FixInvalidFileNameCharsInPath Tests

        [TestCase("", "")]
        [TestCase("   ", "   ")]
        [TestCase(null, "")]
        public void FixInvalidFileNameCharsInPath_NullEmpty(string testName, string expected)
        {
            var result = _gitUtils.FixInvalidFileNameCharsInPath(testName);
            Assert.AreEqual(result, expected);

        }

        [TestCase("C:\\Some\\Valid\\Path", "C:\\Some\\Valid\\Path")]
        [TestCase("C:\\Some-Valid-Path", "C:\\Some-Valid-Path")]
        public void FixInvalidFileNameCharsInPath_validPaths(string testName, string expected)
        {
            var result = _gitUtils.FixInvalidFileNameCharsInPath(testName);
            Assert.AreEqual(result, expected);

        }

        [TestCase("\"How-to-find-\"Unknown\"-elements\"", "How-to-find-Unknown-elements")]
        public void FixInvalidFileNameCharsInPath_BadPaths(string testName, string expected)
        {
            var result = _gitUtils.FixInvalidFileNameCharsInPath(testName);
            Assert.AreEqual(result, expected);

        }

        #endregion
    }
}
