using Newtonsoft.Json.Linq;
using Sharpenter.ResumeParser.Model;
using Sharpenter.ResumeParser.Model.Models;
using Sharpenter.ResumeParser.ResumeProcessor.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Sharpenter.ResumeParser.ResumeProcessor.Parsers
{
    public class PersonalParser : IParser
    {
        private static readonly Regex EmailRegex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", RegexOptions.IgnoreCase);
        private static readonly Regex PhoneRegex = new Regex(@"\(?([0-9]{3,5})\)?[-. ]?([0-9]{3,5})[-. ]?([0-9]{3,5})?", RegexOptions.IgnoreCase);
        private static readonly Regex SocialProfileRegex = new Regex(@"(http(s)?:\/\/)?([\w]+\.)?(linkedin\.com|facebook\.com|github\.com|stackoverflow\.com|bitbucket\.org|sourceforge\.net|(\w+\.)?codeplex\.com|code\.google\.com).*?(?=\s)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex SplitByWhiteSpaceRegex = new Regex(@"\s+|,", RegexOptions.Compiled);
        private readonly HashSet<string> _firstNameLookUp;
        private readonly SortedSet<string> _firstName;
        private readonly List<string> _countryLookUp;
        private readonly List<string> _vnProvideLookup;

        public PersonalParser(IResourceLoader resourceLoader)
        {
            var assembly = Assembly.GetExecutingAssembly();

            _firstNameLookUp = resourceLoader.LoadIntoHashSet(assembly, "FirstName.txt", ',');
            _countryLookUp = new List<string>(resourceLoader.Load(assembly, "Countries.txt", '|'));
            //  _vnProvideLookup = new List<string>(resourceLoader.Load(assembly, "VietnamProvides.txt", '|'));
        }

        public void Parse(Section section, Resume resume)
        {
            var firstNameFound = false;
            var addressFound = false;
            var genderFound = false;
            var emailFound = false;
            var phoneFound = false;
            var dobFound = false;
            var NationalityFound = false;
            var RelocationFound = false;
            var Marital_Status_Found = false;
            foreach (var line in section.Content)
            {
                firstNameFound = ExtractFirstAndLastName(resume, firstNameFound, line);
                genderFound = ExtractGender(resume, genderFound, line);
                dobFound = ExtractDOB(resume, dobFound, line);
                NationalityFound = ExtractNationality(resume, NationalityFound, line);
                addressFound = ExtractAddress(resume, addressFound, line);
                emailFound = ExtractEmail(resume, emailFound, line);
                phoneFound = ExtractPhone(resume, phoneFound, line);
                Marital_Status_Found = ExtractMarital(resume, Marital_Status_Found, line);
                ExtractSocialProfiles(resume, line);
            }
            foreach (var line in section.Content)
            {
                firstNameFound = exTractEnd(resume, firstNameFound, line);
            }
            splitAddress(resume);
        }

        private void splitAddress(Resume resume)
        {
            if (!string.IsNullOrEmpty(resume.AddressFull))
            {
                var country =
               _countryLookUp.FirstOrDefault(c => resume.AddressFull.IndexOf(c.Trim(), StringComparison.InvariantCultureIgnoreCase) > -1);
                if (country != null)
                {
                    resume.County_State = country;
                    resume.StreetAddress = resume.AddressFull.Substring(0,resume.AddressFull.IndexOf(country.Trim()));
                }
            }
        }

        private bool ExtractMarital(Resume resume, bool marital_Status_Found, string line)
        {
            if (marital_Status_Found) return marital_Status_Found;
            // tiếng anh
            if (line.IndexOf("marital", StringComparison.InvariantCultureIgnoreCase) > -1 || line.IndexOf("status", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                if (line.IndexOf("married", StringComparison.InvariantCultureIgnoreCase) > -1) resume.Marital_status = "Married";
                if (line.IndexOf("widowed", StringComparison.InvariantCultureIgnoreCase) > -1) resume.Marital_status = "Widowed";
                if (line.IndexOf("separated", StringComparison.InvariantCultureIgnoreCase) > -1) resume.Marital_status = "Separated";
                if (line.IndexOf("divorced", StringComparison.InvariantCultureIgnoreCase) > -1) resume.Marital_status = "Divorced";
                if (line.IndexOf("single", StringComparison.InvariantCultureIgnoreCase) > -1) resume.Marital_status = "Single";
            }
            // tiếng việt
            if (line.IndexOf("hôn nhân", StringComparison.InvariantCultureIgnoreCase) > -1 || line.IndexOf("tình trạng", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                if (line.IndexOf("đã kết hôn", StringComparison.InvariantCultureIgnoreCase) > -1) resume.Marital_status = "đã kết hôn";
                if (line.IndexOf("góa chồng", StringComparison.InvariantCultureIgnoreCase) > -1) resume.Marital_status = "góa chồng";
                if (line.IndexOf("ly thân", StringComparison.InvariantCultureIgnoreCase) > -1) resume.Marital_status = "ly thân";
                if (line.IndexOf("đã ly dị", StringComparison.InvariantCultureIgnoreCase) > -1) resume.Marital_status = "đã ly dị";
                if (line.IndexOf("độc thân", StringComparison.InvariantCultureIgnoreCase) > -1) resume.Marital_status = "độc thân";
            }
            return false;
        }

        private bool ExtractNationality(Resume resume, bool NationalityFound, string line)
        {
            if (NationalityFound) return NationalityFound;
            if (line.IndexOf("nationality", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                int index = line.IndexOf("nationality", StringComparison.InvariantCultureIgnoreCase) + 11;
                resume.Nationality = StringHelper.RemoveSpecialCharacters(line.Substring(index).Trim());
                NationalityFound = true;
            }
            if (line.IndexOf("quốc tịch", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                int index = line.IndexOf("quốc tịch", StringComparison.InvariantCultureIgnoreCase) + 9;
                resume.Nationality = StringHelper.RemoveSpecialCharacters(line.Substring(index).Trim());
                NationalityFound = true;
            }
            return NationalityFound;
        }

        private bool ExtractDOB(Resume resume, bool dobFound, string line)
        {
            if (dobFound) return dobFound;
            if (line.IndexOf("birth", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                int index = line.IndexOf("birth", StringComparison.InvariantCultureIgnoreCase) + 5;
                resume.DOB = line.Substring(index).Trim();
                resume.DOB = resume.DOB.Substring(Regex.Match(resume.DOB, @"[A-z0-9a-z]").Index);

                //  resume.DOB = DateHelper.StringDate(resume.DOB);
                dobFound = true;
            }
            if (line.IndexOf(" sinh", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                int index = line.IndexOf(" sinh", StringComparison.InvariantCultureIgnoreCase) + 5;
                resume.DOB = line.Substring(index).Trim();
                resume.DOB = resume.DOB.Substring(Regex.Match(resume.DOB, @"[A-z0-9a-z]").Index);
                dobFound = true;
            }
            return dobFound;
        }

        private bool ExtractAddress(Resume resume, bool addressFound, string line)
        {
            if (line.IndexOf("địa chỉ tạm trú", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                addressFound = true;
                line = line.Substring(line.IndexOf("địa chỉ tạm trú", StringComparison.InvariantCultureIgnoreCase) + 15);
                line = StringHelper.RemoveSpecialCharacters(line);
                resume.AddressFull = line;
                return addressFound;
            }
            if (addressFound) return addressFound;

            // english
            if (line.IndexOf("address", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                addressFound = true;
                line = line.Replace("Address", "");
                line = line.Replace("address", "");
                line = StringHelper.RemoveSpecialCharacters(line);
                resume.AddressFull = line;
                return addressFound;
            }
            if (line.IndexOf("city", StringComparison.InvariantCultureIgnoreCase) > -1 || line.IndexOf("street", StringComparison.InvariantCultureIgnoreCase) > -1 || line.IndexOf("provide", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                addressFound = true;
                resume.AddressFull = line.Replace("Address", "");

                return addressFound;
            }
            // vietnamese
           
            if (line.IndexOf("địa chỉ", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                addressFound = true;
                line = line.Substring(line.IndexOf("địa chỉ", StringComparison.InvariantCultureIgnoreCase) + 7);
                line = StringHelper.RemoveSpecialCharacters(line);
                resume.AddressFull = line;
                return addressFound;
            }
            var country =
                _countryLookUp.FirstOrDefault(
                    c => line.IndexOf(c.Trim(), StringComparison.InvariantCultureIgnoreCase) > -1);
            if (country != null)
            {
                resume.AddressFull = line;
             //   resume.Country = line.Substring(line.IndexOf(country.Trim()), country.Trim().Length);

            }
            //Assume address is in one line and ending with country name
            //Working backward to the beginning of the line to get the address

            return addressFound;
        }

        private void ExtractSocialProfiles(Resume resume, string line)
        {
            var socialProfileMatches = SocialProfileRegex.Matches(line);
            foreach (Match socialProfileMatch in socialProfileMatches)
            {
                resume.SocialProfiles.Add(socialProfileMatch.Value);
            }
        }

        private bool ExtractPhone(Resume resume, bool phoneFound, string line)
        {
            if (phoneFound) return phoneFound;

            var phoneMatch = PhoneRegex.Match(line);
            if (!phoneMatch.Success) { phoneFound = false; }
            else
            {
                //english
                if (line.IndexOf("mobile", StringComparison.InvariantCultureIgnoreCase) > -1) resume.MobilePhone = phoneMatch.Value;
                else if (line.IndexOf("home", StringComparison.InvariantCultureIgnoreCase) > -1) resume.HomePhone = phoneMatch.Value;
                else if (line.IndexOf("work", StringComparison.InvariantCultureIgnoreCase) > -1) resume.WorkPhone = phoneMatch.Value;
                else if (line.IndexOf("fax", StringComparison.InvariantCultureIgnoreCase) > -1) resume.Fax = phoneMatch.Value;
                else if (line.IndexOf("di động", StringComparison.InvariantCultureIgnoreCase) > -1 || line.IndexOf("dd", StringComparison.InvariantCultureIgnoreCase) > -1) resume.MobilePhone = phoneMatch.Value;
                else if (line.IndexOf("nhà", StringComparison.InvariantCultureIgnoreCase) > -1) resume.HomePhone = phoneMatch.Value;
                else if (line.IndexOf("việc", StringComparison.InvariantCultureIgnoreCase) > -1) resume.WorkPhone = phoneMatch.Value;
                else if (line.IndexOf("fax", StringComparison.InvariantCultureIgnoreCase) > -1) resume.Fax = phoneMatch.Value;
                else { resume.MobilePhone = phoneMatch.Value; }
            }

            return phoneFound;
        }

        private bool ExtractGender(Resume resume, bool genderFound, string line)
        {
            if (genderFound) return genderFound;

            if (line.IndexOf("male", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                resume.Gender = "male";

                genderFound = true;
            }
            if (line.IndexOf("nam", StringComparison.InvariantCultureIgnoreCase) > -1 && line.IndexOf("giới", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                resume.Gender = "nam";

                genderFound = true;
            }
            if (line.IndexOf("female", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                resume.Gender = "female";

                genderFound = true;
            }
            if (line.IndexOf("nữ", StringComparison.InvariantCultureIgnoreCase) > -1 && line.IndexOf("giới", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                resume.Gender = "nữ";

                genderFound = true;
            }

            return genderFound;
        }



        private bool ExtractFirstAndLastName(Resume resume, bool firstNameFound, string line)
        {


            if (firstNameFound) return firstNameFound;
            //  resume.Salary_expected += line;
            if (line.IndexOf("name", StringComparison.InvariantCultureIgnoreCase) > -1)
            {

                var index = line.IndexOf("name", StringComparison.InvariantCultureIgnoreCase) + 4;
                var linex = SplitByWhiteSpaceRegex.Split(StringHelper.RemoveSpecialCharacters(line.Substring(index)).Trim());
                resume.FirstName = linex[0];
                resume.LastName = string.Join(" ", linex.Skip(1).Take(linex.Length - 1).ToArray());
                firstNameFound = true;

            }
            else if (line.IndexOf("tên", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                var index = line.IndexOf("tên", StringComparison.InvariantCultureIgnoreCase) + 3;
                var linex = SplitByWhiteSpaceRegex.Split(StringHelper.RemoveSpecialCharacters(line.Substring(index)).Trim());
                resume.FirstName = linex[0];
                resume.LastName = string.Join(" ", linex.Skip(1).Take(linex.Length - 1).ToArray());
                firstNameFound = true;

            }
            else
            {

            }

            return firstNameFound;
        }
        private bool exTractEnd(Resume resume, bool firstNameFound, string line)
        {
            if (firstNameFound) return firstNameFound;
            var words = SplitByWhiteSpaceRegex.Split(line);
            for (var i = 0; i < words.Length; i++)
            {
                var word = words[i].Trim();
                String output = String.Empty, gender = String.Empty;
                Stream streamOut;



                if (!firstNameFound && _firstNameLookUp.Contains(word) && !resume.AddressFull.Contains(word))
                {
                    resume.FirstName = word;

                    //Consider the rest of the line as part of last name
                    resume.LastName = string.Join(" ", words.Skip(i + 1));

                    firstNameFound = true;
                }
                else if (!firstNameFound)
                {
                    //System.Net.WebRequest request = System.Net.WebRequest.Create(String.Format("https://api.genderize.io/?name={0}", word));
                    //request.Method = "GET";


                    //System.Net.WebResponse response = request.GetResponse();
                    //streamOut = response.GetResponseStream();
                    //output = new StreamReader(streamOut).ReadToEnd();
                    //Object obj = Newtonsoft.Json.JsonConvert.DeserializeObject(output);



                    //JObject jObject = JObject.Parse(output);

                    //gender = jObject.GetValue("gender", StringComparison.OrdinalIgnoreCase).ToString();
                    //resume.Gender = gender;

                    //if (!String.IsNullOrEmpty(resume.Gender))
                    //{
                    //    resume.Gender = gender;
                    //    resume.FirstName = jObject.GetValue("name", StringComparison.OrdinalIgnoreCase).ToString();
                    //    resume.LastName = string.Join(" ", words.Skip(i + 1));
                    //    firstNameFound = true;
                    //    break;
                    //}
                }
            }
            return firstNameFound;
        }

        private bool ExtractEmail(Resume resume, bool emailFound, string line)
        {
            if (emailFound) return emailFound;

            var emailMatch = EmailRegex.Match(line);
            if (!emailMatch.Success)
            {
                emailFound = false;
            }
            else
            {
                resume.EmailAddress = emailMatch.Value;

                emailFound = true;
            }

            return emailFound;
        }
    }
}
