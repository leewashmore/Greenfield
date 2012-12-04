using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core
{
	public class DumpWriterRecorder : IDumpWriter
	{
		private interface IRecord
		{
			void PlayOn(IDumpWriter writer);
		}

		private class WriteLineRecord : IRecord
		{
			private String line;

			public WriteLineRecord(String line)
			{
				this.line = line;
			}

			public void PlayOn(IDumpWriter writer)
			{
				writer.WriteLine(this.line);
			}
		}

		private class IndentRecord : IRecord
		{
			public void PlayOn(IDumpWriter writer)
			{
				writer.Indent();
			}
		}

		private class UnindenRecord : IRecord
		{
			public void PlayOn(IDumpWriter writer)
			{
				writer.Unindent();
			}
		}

		private List<IRecord> records;

		public DumpWriterRecorder()
		{
			this.records = new List<IRecord>();
		}

		public void WriteLine(String line)
		{
			this.records.Add(new WriteLineRecord(line));
		}

		public void Indent()
		{
			this.records.Add(new IndentRecord());
		}

		public void Unindent()
		{
			this.records.Add(new UnindenRecord());
		}

		public void PlaybackTo(IDumpWriter writer)
		{
			foreach (var record in this.records)
			{
				record.PlayOn(writer);
			}
		}
	}
}
