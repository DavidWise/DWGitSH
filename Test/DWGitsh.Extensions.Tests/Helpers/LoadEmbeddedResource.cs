using System.IO;
using System.Linq;
using System.Reflection;

namespace DWGitsh.Extensions.Tests.Helpers
{
    public class EmbeddedResourceLoader
    {
        private static string[] embeddedResourceNames;
        private static Assembly assembly = null;

        static EmbeddedResourceLoader()
        {
            assembly = Assembly.GetExecutingAssembly();
            embeddedResourceNames = assembly.GetManifestResourceNames();
        }

        public static string ReadAllText(string name)
        {
            string result = null;

            var resourceName = embeddedResourceNames.First(x=> x.ToLower().EndsWith(name.ToLower()));

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
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
