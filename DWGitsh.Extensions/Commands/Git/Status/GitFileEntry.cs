using DWGitsh.Extensions.Utility;
using StaticAbstraction;

namespace DWGitsh.Extensions.Commands.Git.Status
{
    public enum GitFileState
    {
        Unchanged,
        Modified,
        Untracked,
        Renamed,
        Copied,
        Deleted
    }

    public class GitFileEntry
    {
        public GitFileState State { get; set; }
        public string FullPath { get; set; }
        public string SourcePath { get; set; }
        public string Name { get; set; }
        public bool Staged { get; set; }

        protected IStaticAbstraction _diskManager = null;


        public GitFileEntry(string currentPath, string relativePath, string targetPath = null) : this(new StAbWrapper(), currentPath, relativePath, targetPath)
        {
        }

        public GitFileEntry(IStaticAbstraction diskManager, string currentPath, string relativePath, string targetPath = null)
        {
            _diskManager = diskManager;

            string finalRelPath = null;
            string originalRelPath = null;

            if (targetPath == null)
            {
                finalRelPath = relativePath.Replace("/", "\\");
            }
            else
            {
                originalRelPath = relativePath.Replace("/", "\\");
                finalRelPath = targetPath?.Replace("/", "\\");
            }

            var valCurPath = GitUtils.Current.FixInvalidFileNameCharsInPath(currentPath);
            var valRelPath = GitUtils.Current.FixInvalidFileNameCharsInPath(finalRelPath);

            var dest = diskManager.Path.Combine(valCurPath, valRelPath);


            var info = _diskManager.NewFileInfo(dest);
            this.FullPath = info.FullName;
            this.Name = info.Name;

            if (originalRelPath != null)
            {
                var sourceFullPath = diskManager.Path.Combine(currentPath, originalRelPath);
                var oInfo = _diskManager.NewFileInfo(sourceFullPath);
                this.SourcePath = oInfo.FullName;
            }
        }

        public override string ToString()
        {
            return $"{this.State}: {this.Name}";
        }
    }
}
