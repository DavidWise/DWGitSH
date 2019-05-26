using DWPowerShell.Utility.ConsoleIO;
using StaticAbstraction;
using System;

namespace DWGitsh.Extensions.Utility.ConsoleIO
{
    public enum ConsoleTextPosition
    {
        Left,
        Center,
        Right
    }

    public interface IConsoleWriter
    {
        void WriteHeader(string msg, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null, ConsoleTextPosition position = ConsoleTextPosition.Left);
        void BlankoutHeader(ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null);
        void ResetConsole();
        void Write(string message);
        void Write(string message, ColorPair colors);
        void WriteLine(string message, ColorPair colors);
    }

    public class ConsoleWriter : IConsoleWriter
    {
        private readonly ConsoleState _originalState;
        protected IConsole _console;

        public ConsoleWriter(IConsole console)
        {
            _console = console;
            _originalState = new ConsoleState(_console);
        }

        public ConsoleWriter()
        {
            _console = new StAbConsole();
            _originalState = new ConsoleState(_console);
        }

        public void WriteHeader(string msg, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null, ConsoleTextPosition position = ConsoleTextPosition.Left)
        {
            var color = new ColorPair(_console, foregroundColor, backgroundColor);

            _console.CursorLeft = 0; //pos;
            _console.CursorTop = _originalState.WindowTop;

            var line = BuildLine(msg, position);

            this.Write(line, color);
            _originalState.ResetCursor();
        }

        protected string BuildLine(string message, ConsoleTextPosition position)
        {
            // TODO: Handle when command is longer than the available width

            var msg = message;
            var width = _console.WindowWidth;
            var buffer = new string(' ', width);
            if (msg.Length >= width)
            {
                msg = message.Substring(0, width - 3) + "...";
            }

            var line = "";
            switch (position)
            {
                case ConsoleTextPosition.Left:
                    line = msg + buffer;
                    break;
                case ConsoleTextPosition.Right:
                    line = buffer.Substring(msg.Length) + msg;
                    break;
                default:
                    var padding = (width - msg.Length) / 2;
                    var pad = new string(' ', (int) padding);
                    line = $"{pad}{msg}{pad}";
                    break;
            }

            if (line.Length > width) line = line.Substring(0, width);
            if (line.Length < width)
            {
                var padding = new string(' ', width - line.Length);
                line += padding;
            }

            return line;
        }

        public void BlankoutHeader(ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
        {
            BlankOutLine(_originalState.WindowTop, ' ', foregroundColor, backgroundColor);
        }

        protected void BlankOutLine(int y, char blankOutChar = ' ', ConsoleColor? foregroundColor = null,
            ConsoleColor? backgroundColor = null)
        {
            _console.CursorTop = y;
            _console.CursorLeft = 0;
            var msg = new string(blankOutChar, _console.WindowWidth);

            var color = new ColorPair(_console, foregroundColor, backgroundColor);

            this.Write(msg, color);
            _originalState.ResetCursor();
        }


        public void ResetConsole()
        {
            _originalState?.ResetColors();
            BlankoutHeader();
            _originalState?.ResetState();
        }

        public void Write(string message)
        {
            _console.Write(message);
        }

        public void Write(string message, ColorPair colors)
        {
            colors?.ApplyColors();
            _console.Write(message);
        }

        public void WriteLine(string message, ColorPair colors)
        {
            colors?.ApplyColors();
            _console.WriteLine(message);
        }
    }
}
