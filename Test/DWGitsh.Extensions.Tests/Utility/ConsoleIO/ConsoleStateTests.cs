using System;
using NUnit.Framework;
using NSubstitute;
using StaticAbstraction;
using DWPowerShell.Utility.ConsoleIO;
using StaticAbstraction.Mocks;

namespace DWGitsh.Extensions.Tests.Utility.ConsoleIO
{
    [TestFixture]
    public class ConsoleStateTests
    {
        [Test]
        public void ConsoleState_Init()
        {
            var console = Substitute.For<IConsole>();

            console.CursorLeft.Returns(5);
            console.CursorTop.Returns(10);
            console.WindowTop.Returns(12);
            console.BackgroundColor.Returns(ConsoleColor.Cyan);
            console.ForegroundColor.Returns(ConsoleColor.Black);


            var result = new ConsoleState(console);

            Assert.IsTrue(result.CursorLeft == 5);
            Assert.IsTrue(result.CursorTop == 10);
            Assert.IsTrue(result.WindowTop == 12);
            Assert.IsTrue(result.BackColor == ConsoleColor.Cyan);
            Assert.IsTrue(result.ForeColor == ConsoleColor.Black);

            console.CursorLeft.Returns(1);
            Assert.IsTrue(result.CursorLeft == 5);
        }

        [Test]
        public void ConsoleState_resetCursor()
        {
            var console = ConsoleTester.BuildConsole();
            var state = new ConsoleState(console);

            console.WriteLine("Hello");
            Assert.IsTrue(console.CursorTop != state.CursorTop);
            Assert.IsTrue(console.CursorLeft != state.CursorLeft);

            state.ResetCursor();
            Assert.IsTrue(console.CursorTop == state.CursorTop);
            Assert.IsTrue(console.CursorLeft == state.CursorLeft);
        }

        [Test]
        public void ConsoleState_resetColors()
        {
            var console = ConsoleTester.BuildConsole();
            var state = new ConsoleState(console);

            var fg = state.ForeColor;
            var bg = state.BackColor;

            console.BackgroundColor = ConsoleColor.Black;
            console.ForegroundColor = ConsoleColor.White;

            Assert.IsTrue(console.BackgroundColor != state.BackColor);
            Assert.IsTrue(console.ForegroundColor != state.ForeColor);

            state.ResetColors();

            Assert.IsTrue(console.BackgroundColor == bg);
            Assert.IsTrue(console.ForegroundColor == fg);
            Assert.IsTrue(state.BackColor == bg);
            Assert.IsTrue(state.ForeColor == fg);
        }
    }

    // creates a mock implementation that can be manipulated since the underlying console values can be changed separately of code
    class ConsoleTester : MockConsole
    {
        public override void Write(string message)
        {
            this.CursorLeft += message.Length;
        }
        public override void WriteLine(string message)
        {
            this.CursorLeft = 2;
            this.CursorTop++;
            this.WindowTop++;
        }

        public static IConsole BuildConsole()
        {
            return new ConsoleTester
            {
                CursorLeft = 15,
                CursorTop = 10,
                WindowTop = 1,
                BackgroundColor = ConsoleColor.DarkMagenta,
                ForegroundColor = ConsoleColor.Yellow
            };
        }
    }
}
