using DWGitsh.Extensions.Utility;
using NSubstitute;
using NUnit.Framework;
using StaticAbstraction;

namespace DWGitsh.Extensions.Tests.Utility
{
    [TestFixture]
    public class ExtensionsTests
    {
        private const string _winTermEnvVar = "WSLENV";

        protected IEnvironment _env;

        [SetUp]
        public void Setup()
        {
            _env = Substitute.For<IEnvironment>();
        }

        [TestCase(null)]
        [TestCase("")]
        public void IsWindowsTerminal_is_false_for_PS_EXE(string value)
        {
            _env.GetEnvironmentVariable(_winTermEnvVar).Returns(value);

            var result = _env.IsWindowsTerminal();

            Assert.False(result);
        }


        [Test]
        public void IsWindowsTerminal_is_true_for_Windows_terminal()
        {
            _env.GetEnvironmentVariable(_winTermEnvVar).Returns("WT_SESSION::WT_PROFILE_ID");

            var result = _env.IsWindowsTerminal();

            Assert.True(result);
        }
    }
}
