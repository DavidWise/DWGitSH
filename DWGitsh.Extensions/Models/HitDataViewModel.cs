using DWGitsh.Extensions.Commands.Git.ChangeDirectory.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWGitsh.Extensions.Models
{
    public class HitDataViewModel
    {
        public string Name { get; set; }
        public string Directory { get; set; }
        public string Alias { get; set; }
        public int HitCount { get; set; }
        public DateTime DateLastHit { get; set; }
    }

    internal static class HitDataViewModelMapper
    {
        public static HitDataViewModel ToViewModel(this HitData data)
        {
            HitDataViewModel result = null;

            if (data != null)
            {
                result = new HitDataViewModel
                {
                    Alias = data.Alias,
                    Name = data.Name,
                    Directory = data.Directory,
                    HitCount = data.HitCount,
                    DateLastHit = data.DateLastHit
                };
            }

            return result;
        }

        public static IEnumerable<HitDataViewModel> ToViewModel(this IEnumerable<HitData> data)
        {
            IEnumerable<HitDataViewModel> result = null;

            if (data != null)
            {
                result = data.Select(x => x.ToViewModel());
            }

            return result;
        }
    }
}
