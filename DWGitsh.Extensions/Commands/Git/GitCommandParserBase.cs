using DWGitsh.Extensions.Cmdlets.Common;

namespace DWGitsh.Extensions.Commands.Git
{
    public abstract class GitCommandParserBase<T> : ICmdletWriterContainer, IGitCommandResultsParser<T> where T : class, new()
    {
        public IGitCommand Command { get; protected set; }
        public ICmdletWriter CmdletWriter { get; protected set; }


        public GitCommandParserBase(IGitCommand command)
        {
            this.Command = command;
            this.CmdletWriter = command.CmdletWriter;
        }
        

        public abstract T Parse(string text);
    }
}
