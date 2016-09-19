using System;

namespace Sharpenter.ResumeParser.Model
{
    public class Period
    {
        private DateTime dateTime1;
        private DateTime dateTime2;

        public string Start { get; private set; }
        public string End { get; private set; }

        public Period(string start, string end)
        {
            Start = start;
            End = end;
        }

        public Period(DateTime dateTime1, DateTime dateTime2)
        {
            this.dateTime1 = dateTime1;
            this.dateTime2 = dateTime2;
        }
    }
}
