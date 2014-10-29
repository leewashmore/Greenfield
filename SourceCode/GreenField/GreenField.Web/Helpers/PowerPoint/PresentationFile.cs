using System;
using System.Collections.Generic;

namespace GreenField.Web.Helpers
{
    public class PresentationFile
    {
        public long PresentationId { get; set; }
        public Byte[] FileStream { get; set; }
    }
}