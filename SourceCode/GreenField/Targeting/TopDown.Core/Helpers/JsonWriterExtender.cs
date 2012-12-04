using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace TopDown.Core
{
	public static class JsonWriterExtender
	{
        public static JsonTextWriter ToJsonTextWriter(this StringBuilder builder)
        {
            var writer = new JsonTextWriter(new StringWriter(builder));
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 4;
            writer.IndentChar = ' ';
            return writer;
        }
    }

}
