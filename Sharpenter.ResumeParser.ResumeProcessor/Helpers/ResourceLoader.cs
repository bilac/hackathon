using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Sharpenter.ResumeParser.ResumeProcessor.Helpers
{
    public class ResourceLoader : IResourceLoader
    {                
        public HashSet<string> LoadIntoHashSet(Assembly assembly, string resourceName, char delimiter)
        {           
            var lines = Load(assembly, resourceName, delimiter);
            return new HashSet<string>(lines, StringComparer.InvariantCultureIgnoreCase);
        }
        public Random random = new Random((int)DateTime.Now.Ticks);
        public string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
        public IEnumerable<string> Load(Assembly assembly, string resourceName, char delimiter)
        {            
            var fullResourcePath = string.Format("Sharpenter.ResumeParser.ResumeProcessor.Data.{0}", resourceName);
            var resources = assembly.GetManifestResourceNames();
            using (var stream = assembly.GetManifestResourceStream(fullResourcePath))
            {
                using (var reader = new StreamReader(stream))
                {
                    var text = reader.ReadToEnd();
                    return text.Split(delimiter);                    
                }
            }
        }
    }
}
