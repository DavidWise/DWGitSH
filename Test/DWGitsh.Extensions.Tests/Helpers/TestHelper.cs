using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DWGitsh.Extensions.Tests.Helpers
{
    public class TestHelper
    {
        private static Assembly _assembly;
        private static string[] _embeddedResourceNames;

        static TestHelper()
        {
            _assembly = Assembly.GetExecutingAssembly();
            _embeddedResourceNames = _assembly.GetManifestResourceNames();
        }


        // replaces the expected slashes with the dots used in the path for resource names
        private static string ConvertToResourceNameFormat(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Resource name must have a value");

            var result = name.Replace('/', '.');
            result = result.Replace('\\', '.');
            return result;
        }


        /// <summary>
        /// Loads the specified test data from the embedded resources in the test library
        /// </summary>
        /// <param name="name">the name of the resource to load.  It only needs to be the last part of the name, not necessarily the full path</param>
        /// <returns>the resource if found</returns>
        public static string GetTestData(string name)
        {
            string result = null;
            var updatedName = ConvertToResourceNameFormat(name);

            var resourceName = _embeddedResourceNames.Single(x => x.EndsWith(updatedName, StringComparison.InvariantCultureIgnoreCase));

            using (Stream stream = _assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }

            return result;
        }
    }
}
