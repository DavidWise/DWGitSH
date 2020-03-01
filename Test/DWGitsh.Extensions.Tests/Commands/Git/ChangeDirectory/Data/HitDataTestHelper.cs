using System;
using System.Collections.Generic;
using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using Newtonsoft.Json;

namespace DWGitsh.Extensions.Tests.Commands.Git.ChangeDirectory.Data
{
    class HitDataTestHelper
    {
        public static CommandData BuildHelperData(string name, string repoRootDir, int hitCount = 1)
        {
            var baseData = new CommandData();
            var dateLastHit = DateTime.Now.AddMinutes(-5);

            baseData.Repositories.Add(new HitData { 
                Name = name, 
                Directory = repoRootDir, 
                DateLastHit = dateLastHit, 
                HitCount = hitCount 
            });

            return baseData;
        }

        public static string ConvertToJson(CommandData data)
        {
            if (data == null) return string.Empty;
            return JsonConvert.SerializeObject(data);
        }

        public static CommandData CommandData_SingleValue => new CommandData
        {
            Repositories = new List<HitData>
                {
                    new HitData {Name = "Mordin", Directory = "MajorGeneral", DateLastHit = new DateTime(2017, 11, 25), HitCount = 99}
                }
        };

        public static CommandData CommandData_MultipleValues => new CommandData
        {
            Repositories = new List<HitData>
                {
                    new HitData {Name = "Bilbo", Directory = "One Ring Drive", DateLastHit = new DateTime(2001, 12, 10), HitCount = 1},
                    new HitData {Name = "Frodo", Directory = "Burden Ave", DateLastHit = new DateTime(2019, 08, 13), HitCount = 35},
                    new HitData {Name = "Gandalf", Directory = "Monochrome Blvd", DateLastHit = new DateTime(2013, 2, 10), HitCount = 2}
                }
        };
    }
}
