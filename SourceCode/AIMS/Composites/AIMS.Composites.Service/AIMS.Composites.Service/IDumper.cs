using System;
using System.Diagnostics;

namespace AIMS.Composites.Service
{
    public interface IDumper
    {
        void Indent();
        void WriteLine(String message, Boolean addTimestamp = false);
        void WriteLine(String message, Stopwatch stopwatch);
        void Write(String message);
        void Unindent();
    }
}