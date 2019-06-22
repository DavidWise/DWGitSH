using System;
using System.Collections.Generic;
using System.Linq;
using DWGitsh.Extensions.Models;

namespace DWGitsh.Extensions.Commands.Git.Config
{
    public class GitConfigParser : GitCommandParserBase<GitConfig>
    {
        public GitConfigParser(IGitCommand command) : base(command)
        {
            this.Command = command;
        }

        public override GitConfig Parse(string text)
        {
            return ParseRawConfig(this, text);
        }

        protected static GitConfig ParseRawConfig(GitConfigParser parser, string text) { 
        if (string.IsNullOrWhiteSpace(text)) return null;

            var result = new GitConfig();

            var allKeys = ParseRawGitConfigData(text);

            result.Values = allKeys.OrderBy(x=> x.Key).ToDictionary(x=> x.Key, x=> x.Value, StringComparer.InvariantCultureIgnoreCase);


            if (allKeys.ContainsKey("user.name") || allKeys.ContainsKey("user.email"))
            {
                result.User = new GitConfigUser();
                if (allKeys.ContainsKey("user.name")) result.User.Name = allKeys["user.name"];
                if (allKeys.ContainsKey("user.email")) result.User.Email = allKeys["user.email"];

                if (!string.IsNullOrWhiteSpace(result.User.Email))
                {
                    var tName = result.User.Email;
                    var splitPos = tName.IndexOf("@");
                    if (splitPos > 0)
                        result.User.Mailbox = tName.Substring(0, splitPos);
                }
            }

            if (allKeys.ContainsKey("remote.origin.url"))
            {
                result.RepoUrl = allKeys["remote.origin.url"]?.Replace(".git", "");
                var url = new Uri(allKeys["remote.origin.url"]);
                var vals = url.PathAndQuery.Trim('/').Split('/');
                if (vals.Length > 0) result.RepoOwner = vals[0];
                if (vals.Length > 1) result.RepoName = vals[1].Replace(".git", "");
            }

            return result;
        }

        protected static Dictionary<string, string> ParseRawGitConfigData(string cfg)
        {
            var allKeys = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            if (string.IsNullOrWhiteSpace(cfg)) return allKeys;
            var lines = cfg.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            var parsed = lines.Where(x => x.IndexOf('=') > 0).ToArray();

            foreach (var line in parsed)
            {
                var pos = line.IndexOf('=');
                var key = line.Substring(0, pos);
                var val = line.Substring(pos + 1);
                if (!allKeys.ContainsKey(key)) allKeys.Add(key, val);
            }

            return allKeys;
        }
    }
}
