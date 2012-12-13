using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Core;

namespace TopDown.Core
{
	public class Monitor : IMonitor
	{
		public Stack<String> context;

		public Monitor()
		{
			this.context = new Stack<String>();
		}

		protected void Scope(String message)
		{
			this.context.Push(message);
		}

		protected void Unscope()
		{
			this.context.Pop();
		}

		private void RegisterError(Exception exception)
		{
			this.DumpError(exception);
#warning Left undone.
			// do nothing, report
		}

		private void DumpError(Exception exception)
		{
			var context = new Stack<String>(this.context.ToList());
			this.DumpError(exception, context);
		}

		private void DumpError(Exception exception, Stack<String> context)
		{
			if (context.Any())
			{
                var popped = context.Pop();
				Trace.WriteLine(popped);
				Trace.Indent();
				this.DumpError(exception, context);
				Trace.Unindent();
			}
			else
			{
				Trace.WriteLine(exception.Message);
			}
		}


		public TValue DefaultIfFails<TValue>(String message, Func<TValue> handler)
		{
			this.Scope(message);
			try
			{
				var result = handler();
				return result;
			}
			catch (Exception exception)
			{
				this.RegisterError(exception);
				return default(TValue);
			}
			finally
			{
				this.Unscope();
			}

		}

        public void SwallowIfFails(String message, Action handler)
        {
            this.Scope(message);
            try
            {
                handler();
            }
            catch (Exception exception)
            {
                this.RegisterError(exception);
            }
            finally
            {
                this.Unscope();
            }
        }
    }
}
