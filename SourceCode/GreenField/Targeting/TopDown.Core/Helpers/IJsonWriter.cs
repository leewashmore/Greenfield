using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core
{
    public interface IJsonWriter : IDisposable
    {
        void Write(String value, String name);
        void Write(String name, Action handler);
        void WriteArray<TValue>(IEnumerable<TValue> values, String name, Action<TValue> handler);
        void WriteArray<TValue>(IEnumerable<TValue> values, Action<TValue> handler);
        void Write(Action handler);
        void Write(Int32 value, String name);
        void Write(Decimal value, String name);
        void Write(Decimal? value, String name);
        void Write(Decimal value);
        void Write(Decimal? value);
        void Write(String value);
        void Write(Boolean value, String name);
        void Write(Boolean value);
		void Write(DateTime value, String name);
        void WriteNull(String name);
        void WriteNull();
    }
}
