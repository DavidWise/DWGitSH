using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using DWGitsh.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWGitsh.Extensions.Tests.Commands.Git.ChangeDirectory
{
    public class GcdTestHelper
    {

        public static string GetFolderNameFromPath(string path)
        {
            var inPath = (path ?? "").TrimEnd('\\', '/');
            var splitPos = inPath.LastIndexOfAny("\\/".ToCharArray());

            if (splitPos < 0) return null;
            return inPath.Substring(splitPos + 1);
        }

        public static List<HitData> BuildFakeHitData(bool? includeAlias = null)
        {
            return BuildFakeHitData(int.MaxValue, includeAlias);
        }

        public static List<HitData> BuildFakeHitData(int entries, bool? includeAlias = null)
        {
            var result = new List<HitData>();

            var maxEntries = entries;

            if (maxEntries > 0)
            {
                var rawData = BuildRawHitData();

                if (includeAlias.HasValue)
                    rawData = rawData.Where(x => string.IsNullOrEmpty(x.Alias) != includeAlias.Value);

                result = rawData.Take(maxEntries).ToList();
            }

            return result;
        }

        public static bool AreSame(HitData rawData, HitDataViewModel compare)
        {
            if (rawData == null && compare == null) return true;
            if (rawData == null || compare == null) return false;

            if (rawData.HitCount != compare.HitCount) return false;
            if (rawData.DateLastHit != compare.DateLastHit) return false;
            if (rawData.Directory != compare.Directory) return false;
            if (rawData.Alias != compare.Alias) return false;
            return true;
        }

        public static bool AreSame(IEnumerable<HitData> rawData, IEnumerable<HitDataViewModel> compare)
        {
            if (rawData == null && compare == null) return true;
            if (rawData == null || compare == null) return false;
            if (rawData.Count() != compare.Count()) return false;

            foreach (var item in rawData)
            {
                var match = compare.SingleOrDefault(x => AreSame(item, x));
                if (match == null) return false;
            }
            return true;
        }

        private static IEnumerable<HitData> BuildRawHitData()
        {
            var baseTime = DateTime.Now;
            return new List<HitData>
            {
                new HitData { Alias = "Trending", DateLastHit = baseTime.AddMinutes(-10), HitCount = 23, Directory = "S:\\One\\Bad\\FolderPath", LastBranch = "test-branch" }
                , new HitData { Alias = "GibberMeFlibbet", DateLastHit = baseTime.AddMinutes(-90), HitCount = 3, Directory = "S:\\One\\Other\\Folderama", LastBranch = "branch-of-peace" }
                , new HitData { Alias = "Orange", DateLastHit = baseTime.AddMinutes(-1), HitCount = 12, Directory = "S:\\For\\Other\\Folgers", LastBranch = "twig-really" }
                , new HitData { Alias = "Tangerine", DateLastHit = baseTime.AddDays(-31), HitCount = 19, Directory = "G:\\Path\\To\\Elation", LastBranch = "master" }
                , new HitData { Alias = "Quandary", DateLastHit = baseTime.AddDays(-17), HitCount = 4, Directory = "F:\\To\\Be\\Or\\Not\\To\\Be", LastBranch = "great-dane" }
                , new HitData { Alias = "Mastiff", DateLastHit = baseTime.AddDays(-1), HitCount = 9, Directory = "D:\\Big\\Doggie", LastBranch = "on-a-leash" }
                , new HitData { Alias = "Orion", DateLastHit = baseTime.AddHours(-5), HitCount = 72, Directory = "C:\\Bogus\\Nebula", LastBranch = "cloudy-mass" }
                , new HitData { DateLastHit = baseTime.AddHours(-17), HitCount = 23, Directory = "F:\\No\\Alias\\In\\This\\Folder", LastBranch = "dev" }
                , new HitData { DateLastHit = baseTime.AddHours(-19), HitCount = 23, Directory = "G:\\Alias\\Missing\\Here", LastBranch = "dev-too" }
                , new HitData { DateLastHit = baseTime.AddDays(-500), HitCount = 81, Directory = "M:\\Really\\Old\\Repo", LastBranch = "dev-too" }
            };
        }
    }
}
