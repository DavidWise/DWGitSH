using DWGitsh.Extensions.Tests.Helpers;
using DWGitsh.Extensions.Utility.Colors;
using NSubstitute;
using NUnit.Framework;
using StaticAbstraction;
using StaticAbstraction.IO;
using StaticAbstraction.IO.Mocks;
using System;

namespace DWGitsh.Extensions.Tests.Utility.Colors
{
    [TestFixture]
    public class ColorGroupReaderTests
    {
        protected IStaticAbstraction dm = null;
        protected string folder = "C:\\Path\\To\\Files\\";
        protected IFileInfo defaultFileInfo = null;
        protected IFileInfo customFileInfo = null;
        private static string _defaultColorFileText = null;
        private static string[] _defaultColorFileTextLines = null;
        private static string _customColorFileText = null;
        private static string[] _customColorFileTextLines = null;

        [SetUp]
        public void TestSetup()
        {
            dm = Substitute.For<IStaticAbstraction>();

            defaultFileInfo = new MockFileInfo
            {
                Exists = true,
                Name = "default.csv",
                FullName = folder + "default.csv",
                LastWriteTime = DateTime.Now
            };
            customFileInfo = new MockFileInfo
            {
                Exists = true,
                Name = "customColors.csv",
                FullName = folder + "customColors.csv",
                LastWriteTime = DateTime.Now
            };

            if (_defaultColorFileText == null)
            {
                _defaultColorFileText = EmbeddedResourceLoader.ReadAllText("Colors.defaultColors.csv");
                _defaultColorFileTextLines = _defaultColorFileText?.Split('\n');

                _customColorFileText = EmbeddedResourceLoader.ReadAllText("Colors.customColors.csv");
                _customColorFileTextLines = _customColorFileText?.Split('\n');
            }

            MockCallsForFileInfo(defaultFileInfo, _defaultColorFileTextLines);
            MockCallsForFileInfo(customFileInfo, _customColorFileTextLines);
        }

        protected void MockCallsForFileInfo(IFileInfo info, string[] csvLines)
        {
            dm.Path.Combine(Arg.Any<string>(), info.Name).Returns(info.FullName);

            dm.File.Exists(info.FullName).Returns(true);
            dm.NewFileInfo(info.FullName).Returns(info);
            dm.File.ReadAllLines(info.FullName).Returns(csvLines);
        }

        [Test]
        public void ValidPathDefaultsOnly()
        {
            var cgr = new ColorGroupReader(dm, defaultFileInfo.Name, "Nothing.csv");

            var defaultColor = cgr.GetColor("default");
            Assert.IsNotNull(defaultColor);
            Assert.IsTrue(defaultColor.Foreground == ConsoleColor.Gray);
            Assert.IsNull(defaultColor.Background);

            // verify the last value in the test data
            var sysFolder = cgr.GetColor("sysFolder");
            Assert.IsNotNull(sysFolder);
            Assert.IsTrue(sysFolder.Foreground == ConsoleColor.White);
            Assert.IsTrue(sysFolder.Background == ConsoleColor.DarkRed);
        }

        [Test]
        public void ValidPathDefaultsWithOverride()
        {
            var cgr = new ColorGroupReader(dm, defaultFileInfo.Name, "Nothing.csv");

            var color = cgr.GetColor("sysFolder", null, ConsoleColor.DarkMagenta);
            Assert.IsNotNull(color);
            Assert.IsTrue(color.Foreground == ConsoleColor.White);
            Assert.IsTrue(color.Background == ConsoleColor.DarkMagenta);

            color = cgr.GetColor("sysFolder", ConsoleColor.Green, ConsoleColor.DarkMagenta);
            Assert.IsNotNull(color);
            Assert.IsTrue(color.Foreground == ConsoleColor.Green);
            Assert.IsTrue(color.Background == ConsoleColor.DarkMagenta);

        }


        [TestCase("")]
        [TestCase("    ")]
        [TestCase("  \t  ")]
        [TestCase("\t")]
        [TestCase(null)]
        public void ValidPathWhitespaceGroupNameUsesDefault(string groupName)
        {
            var cgr = new ColorGroupReader(dm, defaultFileInfo.Name, "Nothing.csv");

            var defaultColor = cgr.GetColor("default");
            var color = cgr.GetColor(groupName);
            Assert.IsNotNull(color);
            Assert.IsTrue(color.Foreground == defaultColor.Foreground);
            Assert.IsTrue(color.Background == defaultColor.Background);
        }

        [Test]
        public void VerifyDefaultColor()
        {
            var cgr = new ColorGroupReader(dm, defaultFileInfo.Name, "Nothing.csv");

            var logicalDefaultColor = cgr.GetDefaultColor();
            Assert.IsNotNull(logicalDefaultColor);

            var defaultColor = cgr.GetColor("default");
            Assert.IsTrue(logicalDefaultColor.Foreground == defaultColor.Foreground);
            Assert.IsTrue(logicalDefaultColor.Background == defaultColor.Background);
        }


        [Test]
        public void VerifyBadGroupNameUsesDefaultColor()
        {
            var cgr = new ColorGroupReader(dm, defaultFileInfo.Name, "Nothing.csv");

            var defaultColor = cgr.GetDefaultColor();
            Assert.IsNotNull(defaultColor);

            var color = cgr.GetColor("ABCZXY");
            Assert.IsNotNull(color);
            Assert.IsTrue(defaultColor.Foreground == color.Foreground);
            Assert.IsTrue(defaultColor.Background == color.Background);
        }

        [Test]
        public void CustomColorsNormalPath()
        {
            var cgr = new ColorGroupReader(dm, defaultFileInfo.Name, customFileInfo.Name);

            // verify the defaults have changed
            var defaultColor = cgr.GetColor("default");
            Assert.IsNotNull(defaultColor);
            Assert.IsTrue(defaultColor.Foreground == ConsoleColor.Green);
            Assert.IsTrue(defaultColor.Background == ConsoleColor.DarkMagenta);

            // verify a value from the default colors
            var oldColor = cgr.GetColor("basePath");
            Assert.IsNotNull(oldColor);
            Assert.IsTrue(oldColor.Foreground == ConsoleColor.Cyan);
            Assert.IsTrue(oldColor.Background == ConsoleColor.DarkBlue);

            // verify an identical in the test data
            var newGroup = cgr.GetColor("newgroup");
            Assert.IsNotNull(newGroup);
            Assert.IsTrue(newGroup.Foreground == ConsoleColor.Red);
            Assert.IsTrue(newGroup.Background == ConsoleColor.DarkRed);
        }
    }
}
