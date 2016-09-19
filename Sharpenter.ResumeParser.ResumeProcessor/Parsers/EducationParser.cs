using System;
using System.Collections.Generic;
using System.Linq;
using Sharpenter.ResumeParser.Model;
using Sharpenter.ResumeParser.Model.Models;
using Sharpenter.ResumeParser.ResumeProcessor.Helpers;

namespace Sharpenter.ResumeParser.ResumeProcessor.Parsers
{
    public class EducationParser : IParser
    {
        private static readonly List<string> _schoolLookUp =
            new List<string>
            {
                "trung tâm",
                "trường",
                "đại học",
                "cao đẳng",
                "trung cấp",
                "phòng",
                "viện",
                "cơ sở",
                "school",
                "academy",
                "department",
                "faculty",
                "institute",
                "institution",
                "university",
                "college",
                "seminary"
            };
        private static readonly List<string> _courseLookUp = new List<string>
        {
            "bachelor",
            "diploma",
            "master",
            "masters",
            "doctor",
            "phd",
            "cử nhân",
            "bằng",
            "sĩ",
            "viên",

        };
        public void Parse(Section section, Resume resume)
        {
            
            if (resume.Educations != null && resume.Educations.Count>0) return;
           
            resume.Educations = new List<Education>();
            Education currentEducation = null;
            if (resume.resumeLanguage == "vi")
            {
                foreach (var item in section.Content)
                {
                    currentEducation = null;
                    var line = item;
                    var school = VIParseSchool(line);
                    if (string.IsNullOrWhiteSpace(school))
                    {
                        if (currentEducation != null)
                        {
                            var startAndEndDate = DateHelper.ParseStartAndEndDate(line);
                            if (startAndEndDate != null)
                            {
                                currentEducation.StartDate = startAndEndDate.Start;
                                currentEducation.EndDate = startAndEndDate.End;
                            }
                            else
                            {
                                currentEducation.StartDate = DateHelper.StringDate(line);
                                currentEducation.EndDate = DateHelper.StringDate(line);

                            }


                            var course =
                                _courseLookUp.FirstOrDefault(
                                    c => line.IndexOf(c, StringComparison.InvariantCultureIgnoreCase) > -1);
                            if (!string.IsNullOrWhiteSpace(course))
                            {
                                currentEducation.Degree = line;
                            }
                        }
                    }
                    else
                    {
                        if (currentEducation != null && string.IsNullOrWhiteSpace(currentEducation.Degree))
                        {
                            currentEducation.School += ", " + school;
                        }
                        else
                        {
                            currentEducation = new Education
                            {
                                School = school
                            };

                            var startAndEndDate = DateHelper.ParseStartAndEndDate(line);
                            if (startAndEndDate != null)
                            {
                                currentEducation.StartDate = startAndEndDate.Start;
                                currentEducation.EndDate = startAndEndDate.End;
                            }
                            else
                            {
                                currentEducation.StartDate = DateHelper.StringDate(line);
                                currentEducation.EndDate = DateHelper.StringDate(line);

                            }


                            resume.Educations.Add(currentEducation);
                        
                        }
                    }

                }
                return;
            }

            var i = 0;
             
            while (i < section.Content.Count)
            {
                currentEducation = null;
                var line = StringHelper.preserveAlphaNumeric(section.Content[i]);
                var school = ParseSchool(line);

                if (string.IsNullOrWhiteSpace(school))
                {
                    if (currentEducation != null)
                    {
                        var startAndEndDate = DateHelper.ParseStartAndEndDate(line);
                        if (startAndEndDate != null)
                        {
                            currentEducation.StartDate = startAndEndDate.Start;
                            currentEducation.EndDate = startAndEndDate.End;
                        }
                        else
                        {
                            currentEducation.StartDate = DateHelper.StringDate(line);
                            currentEducation.EndDate = DateHelper.StringDate(line);

                        }


                        var course =
                            _courseLookUp.FirstOrDefault(
                                c => line.IndexOf(c, StringComparison.InvariantCultureIgnoreCase) > -1);
                        if (!string.IsNullOrWhiteSpace(course))
                        {
                            currentEducation.Degree = line;
                        }
                    }
                }
                else
                {
                    try
                    {
                        school = school.Replace(DateHelper.StringDate(school), "");
                        school = school.Replace(DateHelper.StringDate(school), "");
                       
                    }
                    catch
                    {

                    }
                    school = StringHelper.RemoveSpecialCharacters(school).Trim();
                    if (currentEducation != null && string.IsNullOrWhiteSpace(currentEducation.Degree))
                    {
                        currentEducation.School += ", " + school;
                    }
                    else
                    {
                        currentEducation = new Education
                        {
                            School = school
                        };

                        var startAndEndDate = DateHelper.ParseStartAndEndDate(line);
                        if (startAndEndDate != null)
                        {
                            currentEducation.StartDate = startAndEndDate.Start;
                            currentEducation.EndDate = startAndEndDate.End;
                        }
                        else
                        {
                            currentEducation.StartDate = DateHelper.StringDate(line);
                            currentEducation.EndDate = DateHelper.StringDate(line);

                        }

                        resume.Educations.Add(currentEducation);
                    }                                                            
                }

                i++;
            }
        }

        private string VIParseSchool(string line)
        {

            var school = _schoolLookUp.FirstOrDefault(s => line.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) > -1);
            if (string.IsNullOrWhiteSpace(school))
            {
                return string.Empty;
            }

            var startOfSchoolIndex = line.IndexOf(school, StringComparison.InvariantCultureIgnoreCase);

            return line.Substring(startOfSchoolIndex).Trim();
        }

        private static string ParseSchool(string line)
        {
  
            var school = _schoolLookUp.FirstOrDefault(s => line.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) > -1);
            if (string.IsNullOrWhiteSpace(school))
            {
                return string.Empty;
            }

            var endOfSchoolIndex = line.IndexOf('\t');
            if (endOfSchoolIndex == -1)
            {
                endOfSchoolIndex = line.Length - 1;
            }

            return line.Substring(0, endOfSchoolIndex + 1).Trim() ;
        }

  
    }
}
