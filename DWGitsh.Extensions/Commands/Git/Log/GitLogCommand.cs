using System;
using DWGitsh.Extensions.Cmdlets;
using DWGitsh.Extensions.Cmdlets.Common;
using DWGitsh.Extensions.Models;

namespace DWGitsh.Extensions.Commands.Git.Log
{
    // good reference for "git log" options:
    // file:///C:/Program%20Files/Git/mingw64/share/doc/git-doc/git-log.html
    // https://git-scm.com/docs/git-log

    public class GitLogCommand : GitCommandBase<GitLogModel, GitLogParser>
    {
        public string EndOfLineToken { get; private set; }
        public string ValueSplitToken { get; private set; }

        public string After { get; set; }
        public string Before { get; set; }

        // for now, caching is disabled since the log filters can change the results so much
        public GitLogCommand(GetGitLog cmdlet) : base(cmdlet.RepositoryDirectories, false)
        {
            this.CmdletWriter = cmdlet.CmdletWriter;
            this.EndOfLineToken = "]|[";
            this.ValueSplitToken = "|,|";
            this.Before = MakeFilterDate(cmdlet.Before);
            this.After = MakeFilterDate(cmdlet.After);

            var after = string.IsNullOrEmpty(this.After) ? "" : $" --since=\"{this.After}\"";

            var before = string.IsNullOrEmpty(this.Before) ? "" : $" --before=\"{this.Before}\"";

            // format specs: https://git-scm.com/docs/pretty-formats
            var formatText = string.Format(" --pretty=format:\"%H{1}%ai{1}%ae{1}%cn{1}%ci{1}%s{1}%d{0}\"", EndOfLineToken, ValueSplitToken);
            var tokenOrder = new GitLogEntryLabels[]
            {
                GitLogEntryLabels.Hash,
                GitLogEntryLabels.AuthorDate,
                GitLogEntryLabels.AuthorEmail,
                GitLogEntryLabels.AuthorName,
                GitLogEntryLabels.CommitDate,
                GitLogEntryLabels.Subject,
                GitLogEntryLabels.Refs
            };

            Name = "Log";
            CacheName = "log";
            Command = $"git log {after}{before}{formatText}";
            CacheLinkFiles= new string[] {"index","FETCH_HEAD", "HEAD", "COMMIT_EDITMSG", "refs/remotes/origin/{branchName}"};
            Parser = new GitLogParser(this, tokenOrder);

            CommandExecFolder = RepositoryDirectories.CurrentPath;
            CmdletWriter.WriteVerbose($"Command: {Command}");
        }


        protected string MakeFilterDate(string dateVal)
        {
            if (string.IsNullOrWhiteSpace(dateVal)) return null;
            try
            {
                var testDate = DateTime.Parse(dateVal);
                return testDate.ToString("o");
            }
            catch { /* do nothing */ }

            // if we are here we have to assume it is a git filter format (i.e. "7 days ago" etc)
            return dateVal.Trim();
        }
    }
}