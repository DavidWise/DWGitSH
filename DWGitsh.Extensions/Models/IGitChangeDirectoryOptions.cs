namespace DWGitsh.Extensions.Models
{
    public interface IGitChangeDirectoryOptions
    {
        string Alias { get; set; }
        bool DoneProcessing { get; set; }
        bool Last { get; set; }
        bool List { get; set; }
        bool Log { get; set; }
        bool LogOnly { get; set; }
        string NameOrAlias { get; set; }
        bool RemoveAlias { get; set; }
    }
}