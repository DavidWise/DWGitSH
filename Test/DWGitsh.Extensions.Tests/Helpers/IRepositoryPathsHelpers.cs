using DWGitsh.Extensions.Models;
using DWGitsh.Extensions.Utility.Cache;
using NSubstitute;

namespace DWGitsh.Extensions.Tests.Helpers
{
    public static class RepositoryPathsHelpers
    {
        public static string PATH_BASE_FOLDER_NO_REPO = "D:\\Bud\\Abbott\\";
        public static string PATH_BASE_FOLDER_WITH_REPO = "D:\\Keaton\\Lloyd\\Chaplin\\";

        private static char[] _seps = new char[] { '\\', '/' };

        private static string PathEval(string value, string defaultValue)
        {
            if (!string.IsNullOrEmpty(value)) return value;
            return defaultValue;
        }

        private static string PathJoin(string basePath, params string[] segments)
        {
            var sep = "\\";
            var sepPos = basePath == null ? -1 : basePath.IndexOfAny(_seps);
            if (sepPos >= 0) sep = basePath.Substring(sepPos, 1);

            var result = PathJoinSingle(sep, basePath, null);

            foreach(var segment in segments)
            {
                result = PathJoinSingle(sep, result, segment);
            }

            return result; 
        }

        private static string TrimEnds(this string value, params char[] trimCharacters)
        {
            // this is not efficient but intended only for unit tests so not a priority
            var result = value;
            if (string.IsNullOrEmpty(value)) return value;

            var evalTrim = trimCharacters != null && trimCharacters.Length > 0;
            var compareLength = value.Length;
            var done = false;

            while (!done)
            {
                result = result.Trim();
                if (evalTrim) result = result.Trim(trimCharacters);
                done = result.Length == compareLength;
                compareLength = result.Length;
            }

            return result;
        }

        private static string PathJoinSingle(string sep, string basePath, string segment)
        {
            if (string.IsNullOrWhiteSpace(basePath)) return null;

            string result = basePath.TrimEnd(_seps);

            var seg = segment.TrimEnds(_seps);

            if (!string.IsNullOrWhiteSpace(seg))
            {
                result += sep + seg;
            }

            return result;
        }

        public static IRepositoryPaths NoRepo(string targetPath = null)
        {
            var cache = Substitute.For<ICommandCache>();
            var result = Substitute.For<IRepositoryPaths>();
            result.Cache.Returns(cache);
            result.CurrentPath.Returns(PathEval(targetPath, PATH_BASE_FOLDER_NO_REPO));
            return result;
        }

        public static IRepositoryPaths WithRepo(string targetPath = null, string relativePath = null)
        {
            var result = NoRepo(targetPath);

            var target = PathEval(targetPath,  PATH_BASE_FOLDER_WITH_REPO);
            result.CurrentPath.Returns(target);
            result.RepositoryFolder.Returns(PathJoin(target, ".git"));
            result.RootFolder.Returns(target);

            if (!string.IsNullOrEmpty(relativePath))
            {
                var fullRelPath = PathJoin(target, relativePath);
                result.CurrentPath.Returns(fullRelPath);

                CalculateRelativePaths(result);
            }

            result.ClearReceivedCalls();
            return result;
        }

        private static void CalculateRelativePaths(IRepositoryPaths paths)
        {
            var curPath = paths?.CurrentPath;
            var root = paths?.RootFolder.TrimEnd(_seps);

            var relPath = BuildRelativePath(root, curPath);

            if (!string.IsNullOrEmpty(relPath))
            {
                paths.RelativePathToRoot.Returns(relPath);

                var relRepoPath = PathJoin(relPath, ".git");
                paths.RelativePathToRepository.Returns(relRepoPath);
            }
        }

        private static string BuildRelativePath(string basePath, string fullPath) {
            // relative paths are only calculated under a repo
            var curPath = fullPath?.TrimEnd(_seps);
            var root = basePath?.TrimEnd(_seps);
            var relPath = string.Empty;
            if (string.IsNullOrEmpty(curPath) || string.IsNullOrEmpty(root)) return relPath;

            while(curPath.Length > root.Length)
            {
                var lastFolderPos = curPath.LastIndexOfAny(_seps);
                if (lastFolderPos <= 0)
                    curPath = root;
                else
                {
                    curPath = curPath.Substring(0, lastFolderPos);
                    relPath += "..\\";
                }
            }

            if (relPath == string.Empty) relPath = ".\\";

            return relPath;

        }
    }
}
