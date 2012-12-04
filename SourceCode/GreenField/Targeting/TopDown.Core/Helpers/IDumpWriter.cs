using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core
{
    public interface IDumpWriter
    {
        void WriteLine(String line);
        void Indent();
        void Unindent();
    }
}
