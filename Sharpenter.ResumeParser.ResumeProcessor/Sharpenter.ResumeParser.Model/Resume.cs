using System.Collections.Generic;

namespace Sharpenter.ResumeParser.Model
{
    public class Resume
    {
        public string resumeLanguage { set; get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Salutation { get; set; }
        public string DOB { set; get; }
        public string Nationality { set; get; }
        public string StreetAddress { get; set; }
        public string Town { get; set; }
        public string County_State {set; get; }
        public string Country { set; get; }
        public string PostCode { set; get; }
        public string Gender { get; set; }
        public string EmailAddress { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string WorkPhone { get; set; }
        public string Fax { set; get; }
        public string AddressFull { get; set; }
        public string Position_expected { set; get; }
        public string Salary_expected { set; get; }
        public string Relocation { set; get; }
        public string Location_expected { set; get; }
        public string Marital_status { set; get; }

        public List<string> Skills { get; set; }

        public List<Position> Positions { get; set; }
        public List<Project> Projects { get; set; }
        public List<string> SocialProfiles { get; set; }
        public List<Education> Educations { get; set; }
        public List<string> Courses { get; set; }
        public List<string> Awards { get; set; }

        public Resume()
        {
            Skills = new List<string>();
            Positions = new List<Position>();
            Projects = new List<Project>();
            SocialProfiles = new List<string>();
            Educations = new List<Education>();
            Courses = new List<string>();
            Awards = new List<string>();
        }
    }
}
