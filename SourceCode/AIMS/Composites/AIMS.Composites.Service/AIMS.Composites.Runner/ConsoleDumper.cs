using System;
using System.Diagnostics;
using System.Globalization;
using AIMS.Composites.Service;

namespace AIMS.Composites.Runner
{
    internal class ConsoleDumper : IDumper
    {
        private Int32 depth;

        public ConsoleDumper()
        {
            depth = 0;
        }

        public void Indent()
        {
            depth++;
        }

        public void WriteLine(String message, Stopwatch stopwatch)
        {
            stopwatch.Stop();
            var indent = new String(' ', depth*4);
            Console.WriteLine("{0}{1} ...{2} seconds.", indent, message,
                              (stopwatch.ElapsedMilliseconds/1000.00).ToString(CultureInfo.InvariantCulture));
        }

        public void WriteLine(String message, Boolean addTimestamp = false)
        {
            var indent = new String(' ', depth*4);
            Console.WriteLine("{0}{1}{2}", indent, message,
                              (addTimestamp ? " [at " + DateTime.Now.ToString("MM/dd/yyyy h:mm:ss.fffff tt") + "]" : ""));
        }

        public void Write(String message)
        {
            Console.Write("{0}", message);
        }

        public void Write(String message, Stopwatch stopwatch)
        {
            Console.Write("{0} ...{1} seconds.", message,
                          (stopwatch.ElapsedMilliseconds/1000.00).ToString(CultureInfo.InvariantCulture));
        }

        public void Unindent()
        {
            if (--depth < 0)
                depth = 0;
        }
    }
}