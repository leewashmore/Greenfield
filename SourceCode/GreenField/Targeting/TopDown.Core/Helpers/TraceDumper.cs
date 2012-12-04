using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core
{
    public class TraceDumper : IDumpWriter
    {
        public void WriteLine(String line)
        {
            Trace.WriteLine(line);
        }
        public void Indent()
        {
            Trace.Indent();
        }
        public void Unindent()
        {
            Trace.Unindent();
        }
    }
}
