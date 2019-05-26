using DWGitsh.Extensions.Cmdlets;
using DWGitsh.Extensions.Utility.Colors;
using DWGitsh.Extensions.Utility.ConsoleIO;
using DWPowerShell.Utility.ConsoleIO;
using System;

namespace DWGitsh.Extensions.Commands.PowerShell
{
    public class WriteTextCommand
    {
        protected static ColorGroupReader _colors = null;
        protected static ColorPair _originalColors = null;

        protected IConsoleWriter _writer = null;

        public bool NoNewLine { get; set; }
        public bool NoColorReset { get; set; }
        public bool ForceColorReset { get; set; }

        public ColorGroupReader Colors
        {
            get { return _colors; }
        }

        public WriteTextCommand() : this(new ConsoleWriter()) { }
        public WriteTextCommand(IConsoleWriter writer)
        {
            if (_colors == null) _colors = new ColorGroupReader("defaultColors.csv", "customColors.csv");
            _writer = writer;
        }


        public void Execute(IWriteTextCmdlet cmdLet)
        {
            if (_originalColors == null) _originalColors = new ColorPair();
            ColorPair group = new ColorPair(cmdLet.ForegroundColor, cmdLet.BackgroundColor);

            var blocks = GetTextValues(cmdLet);
            var colors = GetColorGroups(cmdLet);
            var useNewLine = !this.NoNewLine;

            var lastPos = blocks.Length - 1;
            for (var i = 0; i < blocks.Length; i++)
            {
                var colGroup = i < colors.Length ? colors[i] : null;
                if (colGroup != null)
                    group = _colors.GetColor(colGroup, cmdLet.ForegroundColor, cmdLet.BackgroundColor);

                if (i == lastPos && useNewLine)
                {
                    _writer.WriteLine(blocks[i], group);
                }
                else
                {
                    _writer.Write(blocks[i], group);
                }
            }

            if (this.NoColorReset) group.ApplyColors();
            if (this.ForceColorReset) { _originalColors.ResetColors(); }
        }


        public string[] GetColorGroups(IWriteTextCmdlet cmdLet)
        {
            return GetValues(cmdLet.ColorGroups, cmdLet.ColorGroup, ",");
        }

        public string[] GetTextValues(IWriteTextCmdlet cmdLet)
        {
            return GetValues(cmdLet.TextBlocks, cmdLet.Text, cmdLet.TextSplit);
        }

        protected static string[] GetValues(string[] values, string defaultValue, string splitToken)
        {
            if (values?.Length > 1) return values;
            var value = values?.Length > 0 ? values[0] : defaultValue;

            if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(splitToken))
            {
                return value.Split(new string[] { splitToken }, StringSplitOptions.None);
            }

            return new string[] { value };
        }
    }
}
