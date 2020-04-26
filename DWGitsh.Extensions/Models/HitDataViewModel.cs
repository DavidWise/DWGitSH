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
        public uint Ordinal { get; set; }
        public string Name { get; set; }
        public string Directory { get; set; }
        public string Alias { get; set; }
        public string LastBranch { get; set; }
        public int HitCount { get; set; }
        public DateTime DateLastHit { get; set; }

        public bool HasAlias =>  !string.IsNullOrEmpty(this.Alias);
        public bool HasName =>  !string.IsNullOrEmpty(this.Name);
        public bool HasBranch =>  !string.IsNullOrEmpty(this.LastBranch);

    }

    internal static class HitDataViewModelMapper
    {
        public static HitDataViewModel ToViewModel(this HitData data, uint ordinal)
        {
            HitDataViewModel result = null;

            if (data != null)
            {
                System.Diagnostics.Debug.WriteLine($"Setting to {ordinal}");
                result = new HitDataViewModel
                {
                    Ordinal = ordinal,
                    Alias = data.Alias,
                    Name = data.Name,
                    Directory = data.Directory?.TrimEnd('/', '\\'),
                    LastBranch = data.LastBranch,
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
                uint pos = 1;
                result = data.Select(x => x.ToViewModel(pos++));
            }

            return result;
        }


        public static IEnumerable<HitDataViewModel> FixOrdinal(this IEnumerable<HitDataViewModel> data)
        {
            var result = data;
            uint pos = 1;

            if (result != null)
            {
                foreach (var item in result)
                    item.Ordinal = pos++;
            }

            return result;
        }
    }
}
