using System;

namespace DWGitsh.Extensions.Utility
{
    public static class UtilityExtensions
    {
        public static bool EqualsCI(this string value, string compare)
        {
            if (value == null && compare == null) return true;
            if (value == string.Empty && compare == string.Empty) return true;

            // at this point if either is null they are not equal
            if (value == null || compare == null) return false;

            return (string.Compare(value, compare, StringComparison.InvariantCultureIgnoreCase)== 0);
        }

        public static bool IsSameFolder(this string value, string compare)
        {
            // folders cannot be null or empty
            if (value == null || compare == null) return false;
            if (value == string.Empty || compare == string.Empty) return true;

            var valueTrim = value.TrimEnd('/', '\\');
            var compareTrim = compare.TrimEnd('/', '\\');
            return valueTrim.EqualsCI(compareTrim);
        }
    }
}
 