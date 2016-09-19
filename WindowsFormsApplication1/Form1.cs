using System;
using System.Windows.Forms;
using Sharpenter.ResumeParser.OutputFormatter.Json;
using Sharpenter.ResumeParser.ResumeProcessor;
using Sharpenter.ResumeParser.ResumeProcessor.Helpers;
using IFilterTextReader;
using System.IO;
using System.Diagnostics;

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
            TextReader reader = new FilterReader(txtFolderSave.Text);
            string raw;
            using (reader)
            {
               raw = reader.ReadToEnd();
            }
            var processor = new ResumeProcessor(new JsonOutputFormatter());
            string filename = StringHelper.RandomString(6) + ".json";
            //MessageBox.Show(processor.Process(raw));
            File.WriteAllText(filename,  processor.Process(raw));
            Process.Start("notepad.exe", filename);

        }

        private void button2_Click(object sender, EventArgs e)
        {
         
        }

        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
