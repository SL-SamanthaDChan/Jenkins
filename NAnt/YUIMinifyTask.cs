using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

using Yahoo.Yui.Compressor;

namespace ShopLocal.SmartCircular.NAnt
{
    [TaskName("yuijsminify")]
    public class YUIJSMinifyTask : MinifyTask
    {
        protected override string Minify(Dictionary<string, string> fileContents)
        {
            StringBuilder sb = new StringBuilder();

            foreach(var js in fileContents)
            {
                try
                {
                    var c = new JavaScriptCompressor(js.Value, false);
                    sb.Append(c.Compress());
                }
                catch (Exception)
                {
                    Log(Level.Error, string.Format("Failed to compress {0}", js.Key));
                    throw new BuildException(string.Format("Failed to compress {0}", js.Key));
                }                
            }

            return sb.ToString();
        }
    }

    [TaskName("yuicssminify")]
    public class YUICSSMinifyTask : MinifyTask
    {
        protected override string Minify(Dictionary<string, string> fileContents)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var css in fileContents)
            {
                sb.Append(CssCompressor.Compress(css.Value));
            }

            return sb.ToString();
        }
    }
}
