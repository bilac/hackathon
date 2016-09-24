using System;
using System.IO;
using Sharpenter.ResumeParser.Model;
using Sharpenter.ResumeParser.ResumeProcessor.Parsers;
using Sharpenter.ResumeParser.ResumeProcessor.Helpers;
using Sharpenter.ResumeParser.InputReader.Plain;
using Sharpenter.ResumeParser.Model.Exceptions;
using System.Linq;
using System.Collections.Generic;
using System.Net;

namespace Sharpenter.ResumeParser.ResumeProcessor
{

    public class ResumeProcessor
    {
        private readonly IOutputFormatter _outputFormatter;
        private readonly IInputReader _inputReaders;
        public ResumeProcessor()
        {
            _inputReaders = new StringInputReader() as IInputReader;
        }
        public ResumeProcessor(IOutputFormatter outputFormatter)
        {
            if (outputFormatter == null)
            {
                throw new ArgumentNullException("outputFormatter");
            }

            _outputFormatter = outputFormatter;
            // IInputReaderFactory inputReaderFactory = new InputReaderFactory(new ConfigFileApplicationSettingsAdapter());
            //_inputReaders = inputReaderFactory.LoadInputReaders();

            _inputReaders = new StringInputReader() as IInputReader;

        }

        public static Resume resume;
        public string Process(string location)
        {
            resume = new Resume();
            //try
            //{
            var rawInput = location.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None); ;


            //using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
            //{
            //    string querytemp = rawInput.Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur);
            //    if (querytemp.Length > 11) querytemp = querytemp.Substring(0, 10);
            //    string htmlCode = client.DownloadString("http://ws.detectlanguage.com/0.2/detect?q=" + querytemp + "&key=3580bdeda1fa9ec0d1985d6c6b432334");
            //    htmlCode = htmlCode.Substring(htmlCode.IndexOf("language") + 11);
            //    htmlCode = htmlCode.Substring(0, htmlCode.IndexOf(",") - 1);
            //    ResumeProcessor.resume.resumeLanguage = htmlCode;
            //}
            string checktext = rawInput.Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur);
            resume.resumeLanguage = LanguageHelper.IdentifyLanguage(checktext);

            var sectionExtractor = new SectionExtractor();
            var sections = sectionExtractor.ExtractFrom(rawInput);

            string temp = "";
            foreach (var item in sections)
            {
                temp += "-------------------------------------\r\n";
                temp += item.Type.ToString() + "\r\n";
                foreach (var item1 in item.Content)
                {
                    temp += item1 + "\r\n";
                }
            }
           // return temp;

            IResourceLoader resourceLoader = new CachedResourceLoader(new ResourceLoader());
            var resumeBuilder = new ResumeBuilder(resourceLoader);
            var resume1 = resumeBuilder.Build(sections);

            var formatted = _outputFormatter.Format(resume1);

            return formatted;
            //}
            //catch (IOException ex)
            //{
            //    //throw new ResumeParserException(ex.Data.ToString());
            //    //throw new ResumeParserException("There's a problem accessing the file, it might still being opened by other application", ex);
            //}
        }
    }
}
