using System;

namespace AIMS.Composites.Service
{
    public interface IDumper
    {
        void Indent();
        void WriteLine(String message, Boolean addTimestamp = false);
        void Write(String message);
        void Unindent();
    }
}