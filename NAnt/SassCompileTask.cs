using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

using SassAndCoffee.Ruby.Sass;

namespace ShopLocal.SmartCircular.NAnt
{
	[TaskName( "sasscompile" )]
	public class CompileSassTask : Task
    {
		[TaskAttribute( "compress" )]
		public bool Compress { get; set; }

        [TaskAttribute("outputdir", Required = true)]
        public string OutputDir { get; set; }

        [BuildElement("fileset", Required = true)]
        public FileSet FileSet { get; set; }

		protected override void ExecuteTask() {
			SassCompiler compiler = new SassCompiler();
			Regex extensionRegex = new Regex( ".s(a|c)ss$" );
            bool compressFlag = Compress || false;

            OutputDir = Path.Combine(this.Project.BaseDirectory, OutputDir);
            if (!Directory.Exists(OutputDir) && OutputDir != string.Empty)
            {
                Directory.CreateDirectory(OutputDir);
            }

			foreach ( string filename in FileSet.FileNames ) {
				string targetFilename = extensionRegex.IsMatch( filename ) ? extensionRegex.Replace( filename, ".css" ) : ( filename + ".css" );
				try {
					this.Log( Level.Info, "sass compiling " + filename );
					if ( !File.Exists( filename ) ) {
						throw new Exception( "Can't sass compile because file doesn't exist: " + filename );
					}
					string output = compiler.Compile( filename, compressFlag, dependentFileList: null );
					savefile(Path.Combine(OutputDir, targetFilename), output );
				} catch ( Exception ex ) {
					File.WriteAllText( targetFilename, string.Format( "Error sass compiling {0}: {1}", filename, ex.Message ) );
					throw new BuildException( "Error sass compiling " + filename + ": " + ex.Message, ex );
				}
			}
			this.Log( Level.Info, "sass compiling all files complete" );

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
        
	}
}