namespace DWGitsh.Extensions.Commands.Git
{
    public interface IGitCommandResultsParser<T> where T:class, new()
    {
        IGitCommand Command { get; }
        T Parse(string text);
    }
}
