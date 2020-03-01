using DWGitsh.Extensions.Utility.Cache;

namespace DWGitsh.Extensions.Models
{
    public interface IRepositoryPaths
    {
        string RootFolder { get; set; }
        string RepositoryFolder { get; set; }
        string CurrentPath { get; set; }
        string RelativePathToRoot { get; set; }
        string RelativePathToRepository { get; set; }
        ICommandCache Cache { get; set; }
        bool IgnoreCache { get; set; }
    }

    public class RepoPaths : IRepositoryPaths
    {
        public string RootFolder { get; set; }
        public string RepositoryFolder { get; set; }
        public string CurrentPath { get; set; }
        public string RelativePathToRoot { get; set; }
        public string RelativePathToRepository { get; set; }
        public ICommandCache Cache { get; set; }
        public bool IgnoreCache { get; set; }
    }
}
