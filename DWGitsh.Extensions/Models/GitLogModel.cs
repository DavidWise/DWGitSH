using System;

namespace DWGitsh.Extensions.Models
{
    public enum GitLogEntryLabels
    {
        Unknown,
        Hash,
        AuthorEmail,
        AuthorName,
        CommitDate,
        AuthorDate,
        Subject,
        Refs
    }

    public class GitLogModel
    {
        public string RootFolder { get; set; }
        public string Branch { get; set; }
        public GitLogEntry[] Entries;
    }

    public class GitLogEntry
    {
        public string Hash { get; set; }
        public string ChangedByEmail { get; set; }
        public string ChangedByName { get; set; }
        public string Subject { get; set; }
        public DateTime AuthorDate { get; set; }
        public DateTime CommitDate { get; set; }
        public string ReferenceBranch { get; set; }
        public string[] Tags { get; set; }
    }
}
