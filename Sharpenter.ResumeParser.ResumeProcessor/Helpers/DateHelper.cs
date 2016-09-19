using System.Text.RegularExpressions;
using Sharpenter.ResumeParser.Model;

namespace Sharpenter.ResumeParser.ResumeProcessor.Helpers
{
    public class DateHelper
    {
        private const string ShortMonth = "Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec|Tháng|Năm";
        private const string FullMonth = "January|February|March|April|May|June|July|August|September|October|November|December|T";
        private static readonly Regex StartAndEndDateRegex =
            new Regex(
                string.Format(
                    @"(?<Start>({0}|{1}|\d{{1,2}}|.)[/\s-–](20)?\d{{2}})[/\s-–— ]+(?<End>({0}|{1}|\d{{1,2}}|.)[/\s-– ](20)?\d{{2}}|Current|Now|Present|Hiện tại|Nay|Bây giờ|nay|hiện tại)",
                    ShortMonth, FullMonth), RegexOptions.Compiled);
        private static readonly Regex StartAndEndDateRegex_Years =
         new Regex(
             string.Format(
                   @"(?<Start>(20)?\d{{2}})[/\s-–— ]+(?<End>(20)?\d{{2}}|Current|Now|Present|Hiện tại|Nay|Bây giờ|nay|hiện tại)",
                 ShortMonth, FullMonth), RegexOptions.Compiled);
        private static readonly Regex StartAndEndDateRegex_1Years =
       new Regex(
           string.Format(
                 @"\d{{4}}",
               ShortMonth, FullMonth), RegexOptions.Compiled);
        public static Period ParseStartAndEndDate(string input)
        {
            var match = StartAndEndDateRegex.Match(input);
            if (!match.Success) match = StartAndEndDateRegex_Years.Match(input);
            if (match.Success)
            {
                var startDate = match.Groups["Start"].Value;
                var endDate = match.Groups["End"].Value;

                return new Period(startDate, endDate);
            }

            return null;
        }
        public static string StringDate(string input)
        {
            var match = StartAndEndDateRegex.Match(input);
            if (!match.Success) match = StartAndEndDateRegex_Years.Match(input);
            if(!match.Success) match = StartAndEndDateRegex_1Years.Match(input);
            if (match.Success) return match.Value;
            return null;
        }
    }
}
