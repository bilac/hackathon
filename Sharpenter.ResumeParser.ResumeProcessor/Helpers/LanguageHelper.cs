using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NTextCat;
using System.IO;

namespace Sharpenter.ResumeParser.ResumeProcessor.Helpers
{
    public static class LanguageHelper
    {

        private static RankedLanguageIdentifier _identifier;

        public static string IdentifyLanguage(string text)
        {
            if (_identifier == null)
            {
                var file = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Sharpenter.ResumeParser.ResumeProcessor\Data\Wiki82.profile.xml"));
                if (!file.Exists)
                {
                    throw new FileNotFoundException("Could not find LanguageModels/Core14.profile.xml to detect the language");
                }
                using (var readStream = File.OpenRead(file.FullName))
                {
                    var factory = new RankedLanguageIdentifierFactory();
                    _identifier = factory.Load(readStream);
                }
            }
            var languages = _identifier.Identify(text);
            var mostCertainLanguage = languages.FirstOrDefault();
            if (mostCertainLanguage != null)
            {
                
                return mostCertainLanguage.Item1.Iso639_3=="simple" ? "en":mostCertainLanguage.Item1.Iso639_3;
            }
            return null;
        }
       
    }
}
