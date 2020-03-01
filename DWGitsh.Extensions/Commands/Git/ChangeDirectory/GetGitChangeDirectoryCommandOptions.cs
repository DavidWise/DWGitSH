using DWGitsh.Extensions.Cmdlets;
using DWGitsh.Extensions.Models;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory
{
    public class GetGitChangeDirectoryCommandOptions : IGitChangeDirectoryOptions
    {
        public string NameOrAlias { get; set; }
        public bool Log { get; set; }
        public bool LogOnly { get; set; }
        public bool Last { get; set; }
        public bool List { get; set; }
        public bool DoneProcessing { get; set; }
        public bool RemoveAlias { get; set; }
        public string Alias { get; set; }

        internal GetGitChangeDirectoryCommandOptions() { }

        public GetGitChangeDirectoryCommandOptions(GetGitChangeDirectory command)
        {
            NameOrAlias = command.NameOrAlias?.Trim();
            Log = command.Log.IsPresent || command.LogOnly.IsPresent;
            LogOnly = command.LogOnly.IsPresent;
            Last = command.Last.IsPresent;
            List = command.List.IsPresent;
            RemoveAlias = command.RemoveAlias.IsPresent;
            Alias = command.Alias?.Trim();
        }

        public GetGitChangeDirectoryCommandOptions(IGitChangeDirectoryOptions source)
        {
            if (source == null) return;

            Alias = source.Alias;
            DoneProcessing = source.DoneProcessing;
            Last = source.Last;
            List = source.List;
            Log = source.Log;
            LogOnly = source.LogOnly;
            NameOrAlias = source.NameOrAlias;
            RemoveAlias = source.RemoveAlias;
        }
    }
}
