using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

namespace ShopLocal.SmartCircular.NAnt
{
    // https://github.com/ssg/Eksi.Tasks

    /// <summary>
    /// This is the base task to derive all "minimizers" from.
    /// </summary>
    [TaskName("minify")]
    public class MinifyTask : Task
    {
        [TaskAttribute("outputdir", Required = false)]
        public string OutputDir { get; set; }

        [TaskAttribute("outputfile", Required = true)]
        public string OutputFile { get; set; }

        [BuildElement("fileset", Required = true)]
        public FileSet FileSet { get; set; }


        protected override void ExecuteTask()
        {
            //Debugger.Launch();            

            Dictionary<string, string> filecontent = new Dictionary<string, string>();

            OutputDir = OutputDir ?? Path.GetDirectoryName(OutputFile);
            OutputDir = Path.Combine(this.Project.BaseDirectory, OutputDir);
                        
            if (!Directory.Exists(OutputDir) && OutputDir != string.Empty)
            {
                Directory.CreateDirectory(OutputDir);
            }

            foreach (var fileName in FileSet.FileNames)
            {                
                string content = readfile(FileSet.BaseDirectory + fileName);
                filecontent.Add(fileName, content);                
            }

            string minified = Minify(filecontent);
            savefile(Path.Combine(OutputDir, OutputFile), minified);
        }

        protected void savefile(string file, string filecontent)
        {
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(filecontent);
                }
            }
        }

        protected string readfile(string file)
        {
            string content = null;

            if (!File.Exists(file))
            {
                Log(Level.Error, string.Format("File not found: {0}", file));
                throw new BuildException(string.Format("File not found: {0}", file));
            }

            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            { 
                using(StreamReader sr = new StreamReader(fs))
                {
                    content = sr.ReadToEnd();
                }
            }

            return content;
        }

        protected virtual string Minify(Dictionary<string, string> fileContents)
        {
            throw new BuildException("Cannot call base Minify method. You must use a concrete task that inherits MinifyTask, such as YUIMinify");
        }
    }
}
