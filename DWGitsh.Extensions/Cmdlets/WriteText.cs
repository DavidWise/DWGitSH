using DWGitsh.Extensions.Commands.PowerShell;
using DWGitsh.Extensions.Utility.ConsoleIO;
using DWPowerShell.Utility.Cmdlet;
using System;
using System.Management.Automation;


namespace DWGitsh.Extensions.Cmdlets
{
    public interface IWriteText
    {
        string Text { get; set; }
        string[] TextBlocks { get; set; }
        string ColorGroup { get; set; }
        string[] ColorGroups { get; set; }
        string TextSplit { get; set; }
        SwitchParameter NoNewLine { get; set; }
        SwitchParameter NoColorReset { get; set; }
        SwitchParameter ForceColorReset { get; set; }
        ConsoleColor? ForegroundColor { get; set; }
        ConsoleColor? BackgroundColor { get; set; }
    }

    public interface IWriteTextCmdlet : IWriteText, IDWPSCmdlet
    {
    }

    [Cmdlet("Write", "Text")]
    [OutputType(typeof(void))]
    public class WriteText : DWPSCmdletBase, IWriteTextCmdlet
    {
        [Parameter(Position = 0)]
        public string Text { get; set; }
        [Parameter]
        public string[] TextBlocks { get; set; }

        [Parameter(Position = 1)]
        public string ColorGroup { get; set; }

        [Parameter]
        public string[] ColorGroups { get; set; }

        [Parameter]
        public string TextSplit { get; set; }

        [Parameter]
        public SwitchParameter NoNewLine { get; set; }

        [Parameter]
        public SwitchParameter NoColorReset { get; set; }

        [Parameter]
        public SwitchParameter ForceColorReset { get; set; }

        [Parameter]
        public ConsoleColor? ForegroundColor { get; set; }

        [Parameter]
        public ConsoleColor? BackgroundColor { get; set; }

        protected IConsoleWriter _writer;

        public WriteText() : this(new ConsoleWriter()) { }
        public WriteText(IConsoleWriter writer) : base() {
            _writer = writer;
        }

        protected override void ProcessRecord()
        {
            var command = new WriteTextCommand(_writer)
            {
                NoNewLine = this.NoNewLine.IsPresent,
                NoColorReset = this.NoColorReset.IsPresent,
                ForceColorReset = this.ForceColorReset.IsPresent
            };

            command.Execute(this);
        }
    }
}
