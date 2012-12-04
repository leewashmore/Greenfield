using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingBpt.ChangingTtbpt
{
	public interface IChange
	{
		void Accept(IChangeResolver resolver);
	}

	public interface IChangeResolver
	{
		void Resolve(DeleteChange change);
		void Resolve(InsertChange change);
		void Resolve(UpdateChange change);
	}
}
