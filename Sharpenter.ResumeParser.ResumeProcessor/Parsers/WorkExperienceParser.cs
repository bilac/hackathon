using System;
using System.Linq;
using System.Text.RegularExpressions;
using Sharpenter.ResumeParser.Model;
using Sharpenter.ResumeParser.Model.Models;
using Sharpenter.ResumeParser.ResumeProcessor.Helpers;
using System.Collections.Generic;
using System.Reflection;

namespace Sharpenter.ResumeParser.ResumeProcessor.Parsers
{
    public class WorkExperienceParser : IParser
    {
        private static readonly Regex SplitByWhiteSpaceRegex = new Regex(@"\s+", RegexOptions.Compiled);
        private readonly List<string> _jobLookUp;
        private readonly List<string> _countryLookUp;
        private readonly List<string> _usStatesLookUp;

        public WorkExperienceParser(IResourceLoader resourceLoader)
        {
            var assembly = Assembly.GetExecutingAssembly();

            _jobLookUp = new List<string>(resourceLoader.Load(assembly, "JobTitles.txt", ','));
            _countryLookUp = new List<string>(resourceLoader.Load(assembly, "Countries.txt", '|'));
            _usStatesLookUp = new List<string>(resourceLoader.Load(assembly, "USStates.txt", ','));
        }

        public void Parse(Section section, Resume resume)
        {

            resume.Positions = new List<Position>();

            var i = 0;
            //List<int> advanceParse = new List<int>();
            Position currentPosition = null;
            while (i < section.Content.Count)
            {

                var line = section.Content[i];
                var title = FindJobTitle(line);
                var company = FindJobCompany(line);
                resume.HomePhone += company;
                // ko company, ko title
                if (string.IsNullOrWhiteSpace(title) && string.IsNullOrEmpty(company))
                {
                    if (currentPosition != null)
                    {
                        var startAndEndDate = DateHelper.ParseStartAndEndDate(line);
                        if (startAndEndDate != null)
                        {
                            currentPosition.StartDate = startAndEndDate.Start;
                            currentPosition.EndDate = startAndEndDate.End;
                        }
                        else
                        {
                            currentPosition.Description.Add(line);
                        }
                    }
                }
                else
                {
                    // ko company
                    if (string.IsNullOrEmpty(company))
                    {
                        if (currentPosition == null || !string.IsNullOrEmpty(currentPosition.Employeer))
                        {
                            currentPosition = new Position
                            {
                                Employeer = title
                            };
                            resume.Positions.Add(currentPosition);
                        }
                        else
                            currentPosition.Employeer = title;
                    }
                    else
                    {

                        if (string.IsNullOrEmpty(title))
                        {

                           
                            if (currentPosition == null || !string.IsNullOrEmpty(currentPosition.Location))
                            {
                                currentPosition = new Position
                                {
                                    Location = company
                                };
                                resume.Positions.Add(currentPosition);
                            }
                            else
                                currentPosition.Location = company;

                            var startAndEndDate = DateHelper.ParseStartAndEndDate(line);
                            if (startAndEndDate != null)
                            {
                                currentPosition.StartDate = startAndEndDate.Start;
                                currentPosition.EndDate = startAndEndDate.End;
                            }
                        }

                    }


                }

                i++;
            }
        }



        private string FindJobTitle(string line)
        {
            var elements = SplitByWhiteSpaceRegex.Split(line);
            if (elements.Length > 6)
            {
                return string.Empty;
            }

            return _jobLookUp.FirstOrDefault(job => line.IndexOf(job, StringComparison.InvariantCultureIgnoreCase) > -1);
        }

        private string FindJobCompany(string line)
        {
            var words = SplitByWhiteSpaceRegex.Split(line.Trim());
            string country = null;
            foreach (var word in words)
            {
                if (line.IndexOf("công ty",StringComparison.InvariantCultureIgnoreCase)>-1|| line.IndexOf("ltd", StringComparison.InvariantCultureIgnoreCase) > -1)
                {
                    country = word;
                    break;
                }

            }
            if (country == null)
            {
                return string.Empty;
            }
            else
            {
                while (!string.IsNullOrEmpty(DateHelper.StringDate(line)))
                {
                    line = line.Replace(DateHelper.StringDate(line), "").Trim();
                }
                return StringHelper.RemoveSpecialCharacters(line);
            }
        }
    }
}
