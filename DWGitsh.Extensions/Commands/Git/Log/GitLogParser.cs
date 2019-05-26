using DWGitsh.Extensions.Models;
using DWPowerShell.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWGitsh.Extensions.Commands.Git.Log
{
    public class GitLogParser : GitCommandParserBase<GitLogModel>
    {
        public string ValueSplitToken { get; protected set; }
        public string EndOfLineToken { get; protected set; }

        private Dictionary<GitLogEntryLabels, int> _tokenPositions = new Dictionary<GitLogEntryLabels, int>();
        
        public GitLogParser(GitLogCommand command, GitLogEntryLabels[] tokenOrder) : base(command)
        {
            this.ValueSplitToken = command.ValueSplitToken;
            this.EndOfLineToken = command.EndOfLineToken;

            for (int i = 0; i < tokenOrder.Length; i++)
            {
                _tokenPositions.Add(tokenOrder[i], i);
            }
        }

        public override GitLogModel Parse(string text)
        {
            var result = new GitLogModel();
            if (!string.IsNullOrWhiteSpace(text))
            {
                var items = new List<GitLogEntry>();
                var lines = text.Split(new string[] {EndOfLineToken}, StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var line in lines)
                {
                    var tokens = line.Split(new string[] {ValueSplitToken}, StringSplitOptions.None).TrimAll();
                    var item = new GitLogEntry();

                    item.Hash = MapTokenToString(GitLogEntryLabels.Hash, tokens);
                    item.AuthorDate = MapTokenToDateTime(GitLogEntryLabels.AuthorDate, tokens);
                    item.ChangedByEmail = MapTokenToString(GitLogEntryLabels.AuthorEmail, tokens);
                    item.ChangedByName = MapTokenToString(GitLogEntryLabels.AuthorName, tokens);
                    item.CommitDate = MapTokenToDateTime(GitLogEntryLabels.CommitDate, tokens);
                    item.Subject = MapTokenToString(GitLogEntryLabels.Subject, tokens);

                    var refs = MapTokenToStringArray(GitLogEntryLabels.Refs, tokens);
                    item.Tags = BuildTags(refs);
                    item.ReferenceBranch = GetReferenceBranch(refs);

                    items.Add(item);
                }

                result.Entries = items.OrderByDescending(x => x.CommitDate).ToArray();
            }

            return result;
        }

        protected string[] BuildTags(string[] refs)
        {
            var result = new List<string>();
            foreach (var tagRef in refs)
            {
                if (tagRef != null && tagRef.StartsWith("tag: ", StringComparison.CurrentCulture))
                {
                    var val = tagRef.Substring(4).Trim();
                    result.Add(val);
                }
            }

            return result.ToArray();
        }

        protected string GetReferenceBranch(string[] refs)
        {
            string result = null;

            foreach (var commitRef in refs)
            {
                if (!string.IsNullOrEmpty(commitRef))
                {
                    if (commitRef.StartsWith("origin/", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var candidateBranch = commitRef.Substring(7);
                        var ignoreBranches = new string[] {"HEAD", "develop", "master"};
                        var isIgnored = ignoreBranches.Any(x => string.Compare(x, candidateBranch, StringComparison.InvariantCultureIgnoreCase) == 0);

                        if (result == null && !isIgnored) result = candidateBranch;
                    }
                }
            }

            return result;
        }


        protected string MapTokenToString(GitLogEntryLabels token, string[] values)
        {
            string result = null;
            if (_tokenPositions.ContainsKey(token))
            {
                var pos = _tokenPositions[token];

                if (values.Length > pos) result = values[pos];
            }

            return result;
        }

        protected string[] MapTokenToStringArray(GitLogEntryLabels token, string[] values)
        {
            var result = new List<string>();
            var val = MapTokenToString(token, values);
            if (!string.IsNullOrEmpty(val))
            {
                var vals = val.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var refVal in vals)
                {
                    var newVal = refVal.Trim('(', ')', ' ', '\r', '\n');
                    if (!string.IsNullOrEmpty(newVal)) result.Add(newVal);
                }
            }

            return result.ToArray();
        }

        protected DateTime MapTokenToDateTime(GitLogEntryLabels token, string[] values)
        {
            var result = MapTokenToString(token, values);
            return result == null ? DateTime.MinValue : DateTime.Parse(result);
        }
    }
}
