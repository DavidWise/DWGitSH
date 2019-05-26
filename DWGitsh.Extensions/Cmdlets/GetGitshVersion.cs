using DWGitsh.Extensions.Models;
using DWPowerShell.Utility.Cmdlet;
using System.Management.Automation;
using System.Reflection;

namespace DWGitsh.Extensions.Cmdlets
{
    [Cmdlet(VerbsCommon.Get,"GitshVersion")]
    [OutputType(typeof(VersionInfo))]
    public class GetGitshVersion : DWPSCmdletBase
    {
        public static readonly VersionInfo Version;

        static GetGitshVersion()
        {
            Version = BuildVersionInfo();
        }

        public GetGitshVersion() : base() { }

        protected override void ProcessRecord()
        {
            WriteObject(Version);
        }

        protected static VersionInfo BuildVersionInfo()
        {
            var asm = Assembly.GetExecutingAssembly();
            var asmName = asm.GetName();
            var ver = asmName.Version;
            var version = new VersionInfo
            {
                Name = asmName.Name,
                Major = ver.Major,
                Minor = ver.Minor,
                Revision = ver.Revision,
                Build = ver.Build,
                Text = ver.ToString(4),
                BuildNote = "" // meant to convey any special notes about the build - not sure yet where to draw this from automatically
            };


            if (!string.IsNullOrWhiteSpace(version.BuildNote))
                version.TextFull = version.Text + " " + version.BuildNote;
            else
                version.TextFull = version.Text;

            return version;
        }
    }
}
