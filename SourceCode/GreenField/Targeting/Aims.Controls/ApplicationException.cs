using System;

namespace Aims.Controls
{
    public class ApplicationException : Exception
    {
        public ApplicationException() { }
        public ApplicationException(String message) : base(message) { }
        public ApplicationException(String message, Exception inner) : base(message, inner) { }
    }
}
