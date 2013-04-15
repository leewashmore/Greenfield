using System;
using System.Diagnostics;
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
            var indent = new String(' ', depth * 4);
            Console.WriteLine(indent + message + " ..." + (stopwatch.ElapsedMilliseconds / 1000.00).ToString() + " seconds.");
        }

        public void WriteLine(String message, Boolean addTimestamp = false)
        {
            var indent = new String(' ', depth*4);
            Console.WriteLine(indent + message +
                              (addTimestamp ? " [at " + DateTime.Now.ToString("MM/dd/yyyy h:mm:ss.fffff tt") + "]" : ""));
        }

        public void Write(String message)
        {
            var indent = new String(' ', depth * 4);
            Console.Write(indent + message +
                              (false ? " (at " + DateTime.Now.ToString("MM/dd/yyyy h:mm:ss.fffff tt") + ")" : ""));
        }

        public void Unindent()
        {
            if (--depth < 0)
            {
                depth = 0;
            }
        }
    }
}