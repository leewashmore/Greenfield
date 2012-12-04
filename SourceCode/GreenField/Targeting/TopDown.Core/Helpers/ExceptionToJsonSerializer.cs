using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core
{
    public class ExceptionToJsonSerializerOptions
    {
        [DebuggerStepThrough]
        public ExceptionToJsonSerializerOptions()
        {
        }

        [DebuggerStepThrough]
        public ExceptionToJsonSerializerOptions(Boolean includeStackTrace, Boolean includeType, Boolean includeInnerExceptions)
        {
            this.IncludeStackTrace = includeStackTrace;
            this.IncludeType = includeType;
            this.IncludeInnerExceptions = includeInnerExceptions;
        }

        public Boolean IncludeStackTrace { get; set; }
        public Boolean IncludeType { get; set; }
        public Boolean IncludeInnerExceptions { get; set; }
    }


    public class ExceptionToJsonSerializer
    {
        public String RenderToJson(Exception exception, ExceptionToJsonSerializerOptions options)
        {
            var builder = new StringBuilder();
            using (var writer = new JsonWriter(builder.ToJsonTextWriter()))
            {
                writer.Write(delegate
                {
                    this.SerializeException(exception, options, writer);
                });
            }
            return builder.ToString();
        }

        public void SerializeException(Exception exception, ExceptionToJsonSerializerOptions options, IJsonWriter writer)
        {
            writer.Write(exception.Message, "message");

            if (options.IncludeType)
            {
                writer.Write(exception.GetType().Name, "type");
            }

            if (options.IncludeStackTrace)
            {
                var lines = (exception.StackTrace ?? String.Empty).Split('\n').Select(x => x.Trim()).ToArray();
                writer.WriteArray(lines, "stack", line =>
                {
                    writer.Write(line);
                });
            }

            if (options.IncludeInnerExceptions)
            {
                if (exception.InnerException != null)
                {
                    writer.Write("inner", delegate
                    {
                        this.SerializeException(exception.InnerException, options, writer);
                    });
                }
                else
                {
                    writer.WriteNull("inner");
                }
            }
        }
    }
}
