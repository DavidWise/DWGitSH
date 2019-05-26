using System;
using System.Collections.Generic;
using DWGitsh.Extensions.Models;

namespace DWGitsh.Extensions.Commands.Git.Status
{
    public class GitStatusParser : GitCommandParserBase<GitStatus>
    {
        public GitStatusParser(IGitCommand command) : base(command)
        {
        }

        public override GitStatus Parse(string text)
        {
            return ParseRawStatus(this, text);
        }

        protected static GitStatus ParseRawStatus(GitStatusParser parser, string status)
        {
            if (string.IsNullOrWhiteSpace(status)) return null;

            var lines = status.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var result = new GitStatus();

            int[] aheadBehind = new int[0];

            var statusOverride = GitFileState.Unchanged;
            var staged = false;
            var cmp = StringComparison.InvariantCultureIgnoreCase;

            var stageCount = 0;
            var unstageCount = 0;

            foreach (var line in lines)
            {
                if (line.IndexOf("Untracked files:", cmp) >= 0) statusOverride = GitFileState.Untracked;
                if (line.StartsWith("Changes to be committed", cmp)) staged = true;
                if (line.StartsWith("Changes not staged for commit:", cmp)) staged = false;
                if (line.Contains(" detached at "))
                {
                    var detachArgs = line.Split(new string[] {" detached at "}, StringSplitOptions.None);
                    result.Detached = true;
                    if (detachArgs.Length > 0) result.DetachedMarker = detachArgs[0].Trim();
                    if (detachArgs.Length > 1) result.DetachedAt = detachArgs[1].Trim();
                }
                
                if (line.StartsWith("On branch "))
                    result.Branch = line.Substring(10);
                else if (line.StartsWith("Your branch is "))
                    aheadBehind = ParseAheadBehindLine(line);
                else
                {
                    if (line.StartsWith("\t"))
                    {
                        var file = ParserFileLine(parser.Command.RepositoryDirectories, line);
                        if (file != null)
                        {
                            if (statusOverride == GitFileState.Untracked) file.State = statusOverride;
                            file.Staged = staged;
                            if (result.FileChanges == null) result.FileChanges = new List<GitFileEntry>();
                            result.FileChanges.Add(file);

                            if (staged)
                                stageCount++;
                            else
                                unstageCount++;
                        }
                    }
                }
            }

            if (aheadBehind.Length > 0) result.Ahead = aheadBehind[0];
            if (aheadBehind.Length > 1) result.Behind = aheadBehind[1];
            result.Staged = stageCount;
            result.Unstaged = unstageCount;

            return result;
        }

        protected static int[] ParseAheadBehindLine(string line)
        {
            // Your branch is behind 'origin/develop' by 1 commit, and can be fast-forwarded.
            // Your branch is ahead of 'origin/master' by 1 commit.
            // ***there is a third line for both ahead/behind that I dont have the exact text for and thus cant parse (yet)

            var result = new int[] { 0, 0 };

            var newLine = line.Replace("Your branch is ", "").Replace(" and can be fast-forwarded.", "").Replace("commit", "");

            var splitPos = newLine.IndexOf(" by ", StringComparison.InvariantCultureIgnoreCase);
            if (splitPos < 0 || splitPos + 4 > newLine.Length) return result;

            splitPos += 4;
            var endPos = newLine.IndexOf(" ", splitPos, StringComparison.InvariantCultureIgnoreCase);
            if (endPos < 0) return result;

            var cnt = Convert.ToInt32("0" + newLine.Substring(splitPos, endPos - splitPos));

            if (newLine.StartsWith("ahead")) result[0] = cnt;
            if (newLine.StartsWith("behind")) result[1] = cnt;

            return result;
        }

        protected static GitFileEntry ParserFileLine(IRepositoryPaths repoDirs, string line)
        {
            var info = line.Trim();
            if (string.IsNullOrWhiteSpace(info)) return null;

            string relPath = null;
            string toPath = null;
            var defaultState = GitFileState.Unchanged;

            var splitPos = info.IndexOf(": ");
            if (splitPos > 0)
            {
                var changeType = info.Substring(0, splitPos);
                relPath = info.Substring(splitPos + 1).Trim();

                if (changeType == "modified") defaultState = GitFileState.Modified;
                if (changeType == "deleted") defaultState = GitFileState.Deleted;
                if (changeType == "renamed") defaultState = GitFileState.Renamed;
                if (changeType == "copied") defaultState = GitFileState.Copied;

                var toPos = relPath.IndexOf("->");
                if (toPos > 0)
                {
                    toPath = relPath.Substring(toPos + 2).Trim();
                    relPath = relPath.Substring(0, toPos).Trim();
                }

            }
            else
                relPath = info;

            var result = new GitFileEntry(repoDirs.CurrentPath, relPath, toPath)
            {
                State = defaultState
            };

            return result;
        }
    }
}