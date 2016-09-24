using System;
using System.Windows.Forms;
using Sharpenter.ResumeParser.OutputFormatter.Json;
using Sharpenter.ResumeParser.ResumeProcessor;
using Sharpenter.ResumeParser.ResumeProcessor.Helpers;
using IFilterTextReader;
using System.IO;
using System.Diagnostics;
using iTextSharp.text.pdf;
using System.Text;
using iTextSharp.text.pdf.parser;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            //String resumeText = richTextBox1.Text;

            //var processor = new ResumeProcessor(new JsonOutputFormatter());

            //String output = processor.Process(resumeText);

            //richTextBox1.Text = output;
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtFolderSave.Text = openFileDialog1.FileName;
            }

            string raw;
            try
            {
                TextReader reader = new FilterReader(txtFolderSave.Text);
                
                using (reader)
                {
                    raw = reader.ReadToEnd();
                }
            }
            catch
            {
                using (PdfReader reader = new PdfReader(txtFolderSave.Text))
                {
                    StringBuilder sb = new StringBuilder();

                    for (int page = 0; page < reader.NumberOfPages; page++)
                    {
                        string text = PdfTextExtractor.GetTextFromPage(reader, page + 1, new SimpleTextExtractionStrategy());
                        if (!string.IsNullOrWhiteSpace(text))
                            sb.Append(text);
                    }
                    raw = sb.ToString();
                }
            }
            var processor = new ResumeProcessor(new JsonOutputFormatter());
            string filename = StringHelper.RandomString(6) + ".json";
            //MessageBox.Show(processor.Process(raw));
            File.WriteAllText(filename,  processor.Process(raw));
            Process.Start("notepad.exe", filename);

        }

    
    }
}
