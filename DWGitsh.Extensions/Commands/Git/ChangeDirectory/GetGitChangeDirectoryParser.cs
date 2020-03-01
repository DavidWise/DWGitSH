using DWGitsh.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWGitsh.Extensions.Commands.Git.ChangeDirectory
{
    public class GetGitChangeDirectoryParser : GitCommandParserBase<GitChangeDirectory>
    {
        public GetGitChangeDirectoryParser(IGitCommand command) : base(command)
        {

        }

        public override GitChangeDirectory Parse(string text)
        {
            return new GitChangeDirectory();
        }
    }
}
