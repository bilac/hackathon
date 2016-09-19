using System.Collections.Generic;
using System.Linq;
using Sharpenter.ResumeParser.Model.Models;

namespace Sharpenter.ResumeParser.ResumeProcessor
{
    public class SectionMatchingService
    {
        private readonly Dictionary<SectionType, List<string>> _keyWordRegistry = new Dictionary<SectionType, List<string>>
        {                        
            {SectionType.Education, new List<string> {"education", "study", "school","degree","institution", "academic", "qualification", "học vấn", "học tập"}},
            {SectionType.Courses, new List<string> {"coursework", "course","khóa học","mong muốn"}},
            {SectionType.Summary, new List<string> {"summary","profile", "cá nhân", "tóm tắt","lý lịch"}},
            {SectionType.WorkExperience, new List<string> {"experience", "work", "employment", "experiences", "quá trình làm việc","kinh nghiệm","công việc đã làm","nghề nghiệp"}},
            {SectionType.Projects, new List<string> {"project","academic projects","projects","dự án"}},
            {SectionType.Skills, new List<string> {"skill", "ability", "tool","skills","kỹ năng","khả năng"}},
            {SectionType.Awards, new List<string> {"award", "certification", "certificate","Certifications","giải thưởng","các danh hiệu","chứng nhận"}}
        };

        public SectionType FindSectionTypeMatching(string input)
        {
            return
                (from sectionType in _keyWordRegistry
                 where sectionType.Value.Any(input.Contains)
                 select sectionType.Key).FirstOrDefault();
        }
    }
}
