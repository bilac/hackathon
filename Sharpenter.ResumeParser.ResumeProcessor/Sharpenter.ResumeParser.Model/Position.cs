using System.Collections.Generic;

namespace Sharpenter.ResumeParser.Model
{
    public class Position
    {
        public string Employeer { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        //      public string Company { get; set; }        
        public string Location { set; get; }
        public string Position_held { set; get; }

        public List<string> Description { get; set; }


        public Position()
        {
            Description = new List<string>();
        }
    }
}