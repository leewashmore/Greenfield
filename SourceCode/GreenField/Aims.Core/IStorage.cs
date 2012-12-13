using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Core
{
	/// <summary>
	/// Something that knowns how to store and retreive an object by a key.
	/// </summary>
	public interface IStorage<TValue>
	{
		TValue this[String key] { get; set; }
    }
}
