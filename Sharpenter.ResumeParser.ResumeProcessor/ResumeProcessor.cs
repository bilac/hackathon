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
    public class Detection
    {
        public string language { get; set; }
        public bool isReliable { get; set; }
        public float confidence { get; set; }
    }

    public class ResultData
    {
        public List<Detection> detections { get; set; }
    }

    public class Result
    {
        public ResultData data { get; set; }
    }

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

        public static Resume resume = new Resume();
        public string Process(string location)
        {

            try
            {
                var rawInput = location.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None); ;


                using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
                {
                    string querytemp = rawInput.Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur);
                    if (querytemp.Length > 11) querytemp = querytemp.Substring(0, 10);
                    string htmlCode = client.DownloadString("http://ws.detectlanguage.com/0.2/detect?q=" + querytemp + "&key=3580bdeda1fa9ec0d1985d6c6b432334");
                    htmlCode = htmlCode.Substring(htmlCode.IndexOf("language") + 11);
                    htmlCode = htmlCode.Substring(0, htmlCode.IndexOf(",") - 1);
                    ResumeProcessor.resume.resumeLanguage = htmlCode;
                }
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
                //return temp;

                IResourceLoader resourceLoader = new CachedResourceLoader(new ResourceLoader());
                var resumeBuilder = new ResumeBuilder(resourceLoader);
                var resume = resumeBuilder.Build(sections);

                var formatted = _outputFormatter.Format(resume);

                return formatted;
            }
            catch (IOException ex)
            {
                throw new ResumeParserException("There's a problem accessing the file, it might still being opened by other application", ex);
            }
        }
    }
}
