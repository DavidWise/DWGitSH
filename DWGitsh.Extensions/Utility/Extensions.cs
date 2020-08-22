using StaticAbstraction;
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


        /// <summary>
        /// Experimental - this seems to be a way to detect if the app was launched using the Windows Terminal instead of the PowerShell command line
        /// </summary>
        /// <param name="environment"></param>
        /// <returns>true if it could detect that the command is running in Windows Terminal, false if not</returns>
        public static bool IsWindowsTerminal(this IEnvironment environment)
        {
            var isWinTerm = false;

            if (environment != null)
            {
                var winTermVar = environment.GetEnvironmentVariable("WSLENV");
                if (!string.IsNullOrEmpty(winTermVar)) isWinTerm = true;
            }

            return isWinTerm;
        }
    }
}
 