using DWPowerShell.Utility;
using DWPowerShell.Utility.Cmdlet;
using StaticAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace DWGitsh.Extensions.Cmdlets
{
    [Cmdlet(VerbsCommon.Get,"GitDir")]
    [OutputType(typeof(string))]
    public class GetGitDir : DWPSCmdletBase, IDynamicParameters
    {
        private static string _infoDir = null;
        private static readonly Dictionary<string, string> _gitDirs = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        public static IStaticAbstraction _diskManager = new StAbWrapper();

        protected RuntimeDefinedParameter RepoName { get; set; }

        static GetGitDir()
        {
            LoadGitDirs(_diskManager);
        }


        public GetGitDir() : base()
        {
        }

        public object GetDynamicParameters()
        {
            // initial logic came from https://foxdeploy.com/2017/01/13/adding-tab-completion-to-your-powershell-functions/

            if (_gitDirs.Count == 0) return null;
            var names = _gitDirs.OrderBy(x=> x.Key).Select(x => x.Key).ToArray();
            var paramName = "RepoName";

            var rtDict = new RuntimeDefinedParameterDictionary();
            var attributes = new System.Collections.ObjectModel.Collection<Attribute>();
            var parameterAttribute = new ParameterAttribute
            {
                Mandatory = false,
                Position = 0
            };
            attributes.Add(parameterAttribute);
            var validateSetAttribute = new ValidateSetAttribute(names);
            attributes.Add(validateSetAttribute);

            RepoName = new RuntimeDefinedParameter(paramName, typeof(string), attributes);
            rtDict.Add(paramName, RepoName);
            

            return rtDict;
        }

        protected override void ProcessRecord()
        {
            string result = null;
            if (RepoName.IsSet)
            {
                var key = RepoName.Value as string;
                if (_gitDirs.ContainsKey(key))
                {
                    var targetPath = _gitDirs[key];

                    _diskManager.Directory.SetCurrentDirectory(targetPath);
                }
                else
                {
                    result = $"'{key}' is not a known git repository";
                }
            }

            if (result != null) WriteObject(result);
        }


        private static void LoadGitDirs(IStaticAbstraction diskManager)
        {
            _infoDir = diskManager.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "gitsh", "localRepos");

            if (!diskManager.Directory.Exists(_infoDir)) return;
            var repoNames = diskManager.Directory.GetFiles(_infoDir, "*.repo");
            foreach (var repoName in repoNames)
            {
                var repoPath = diskManager.File.ReadAllText(repoName);
                var nameInfo = diskManager.NewFileInfo(repoName);
                var name = nameInfo.Name.Replace(nameInfo.Extension, "");
                _gitDirs.Add(name, repoPath);
            }
        }

        public static void TagRepoDir(string name, string path)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(path)) return;
            if (_gitDirs.ContainsKey(name) &&
                string.Compare(_gitDirs[name], path, StringComparison.InvariantCultureIgnoreCase) == 0) return;

            if (!_diskManager.Directory.Exists(_infoDir)) _diskManager.Directory.CreateDirectory(_infoDir);

            var fileName = _diskManager.Path.Combine(_infoDir, DWPSUtils.MakeFileSystemSafe(name + ".repo"));

            if (_diskManager.File.Exists(fileName))
            {
                var touchInfo = _diskManager.NewFileInfo(fileName);
                touchInfo.LastAccessTimeUtc = DateTime.UtcNow;
                return;
            }

            _diskManager.File.WriteAllText(fileName, path);

            _gitDirs.Add(name, path);
        }
    }

}
